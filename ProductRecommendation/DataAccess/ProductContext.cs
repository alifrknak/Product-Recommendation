

namespace ProductRecommendation.DataAccess;

using Microsoft.EntityFrameworkCore;
using ProductRecommendation.Models;

public class ProductContext : DbContext
{
    public ProductContext(DbContextOptions<ProductContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
}
