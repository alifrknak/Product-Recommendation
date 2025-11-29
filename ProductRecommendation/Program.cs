
using Microsoft.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using ProductRecommendation.DataAccess;
using ProductRecommendation.Models;

namespace ProductRecommendation;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Configuration.AddEnvironmentVariables();

        builder.Services.AddDbContext<ProductContext>(options =>
           options.UseSqlServer(Environment.GetEnvironmentVariable("ConnectionStrings")));


        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.Services.CreateScope().ServiceProvider.GetRequiredService<ProductContext>().Database.Migrate();



        app.MapGet("/api/products", async (ProductContext db) =>
        {
            return Results.Ok(await db.Products.ToListAsync());
        });



        app.MapPost("/api/products", async (AddProductModel request, ProductContext db) =>
        {
            ArgumentNullException.ThrowIfNull(request.Name);
            ArgumentNullException.ThrowIfNull(request.Description);

            Embedding<float> embedding = await EmbeddingGenerator.GenerateEmbedding(request.Description);

            db.Products.Add(new Product
            {
                Name = request.Name,
                Description = request.Description,
                Embedding = new SqlVector<float>(embedding.Vector)
            });

            await db.SaveChangesAsync();

            return Results.Ok(new
            {
                Message = "Product created",
            });
        });



        app.MapGet("/api/products/{id}/recommendations", async (int id, ProductContext db) =>
        {
            // Týklanan ürünü veritabanýndan al
            var clickedProduct = await db.Products.FindAsync(id);

            if (clickedProduct == null)
            {
                return Results.NotFound(new { Message = "Product not found" });
            }

            SqlVector<float> targetVector = clickedProduct.Embedding;

            var similarProducts = await db.Products
                .Where(p => p.Id != id) // ayný ürünü hariç tut
                .OrderBy(p => EF.Functions.VectorDistance(
                    "cosine", // cosine benzerlik metriðini kullan
                    p.Embedding,
                    targetVector
                )) // benzerlik sýrasýna göre sýrala
                .Take(3) // en benzer 3 ürünü al
                .ToListAsync();

            return Results.Ok(similarProducts);
        });


        app.MapControllers();

        app.Run();
    }
}
