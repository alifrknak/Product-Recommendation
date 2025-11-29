namespace ProductRecommendation.Models;

using Microsoft.Data.SqlTypes;
using System.ComponentModel.DataAnnotations.Schema;


public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    [Column(TypeName = "vector(100)")]
    public SqlVector<float> Embedding { get; set; }
}
