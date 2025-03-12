using System.ComponentModel.DataAnnotations;

namespace warehouse_service.Repository.Entities;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
}