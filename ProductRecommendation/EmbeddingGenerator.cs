using GeminiDotnet.Extensions.AI;
using Microsoft.Extensions.AI;

namespace ProductRecommendation;

public class EmbeddingGenerator
{

    public static async Task<Embedding<float>> GenerateEmbedding(string input)
    {
        var model = "gemini-embedding-001";
        
        IEmbeddingGenerator<string, Embedding<float>> generator =
      new GeminiEmbeddingGenerator(new GeminiDotnet.GeminiClientOptions
      {
          ApiKey = Environment.GetEnvironmentVariable("APIKEY")!,
          ModelId = model

      });

        Embedding<float> embedding = await generator.GenerateAsync(input,
            new EmbeddingGenerationOptions
            {
                Dimensions = 100
                // 100 olarak ayarladýk çünkü çok büyük vektörler performans sorunlarýna yol açabilir. bu  sadece bir örnek proje.
            });

        return embedding;
    }
}