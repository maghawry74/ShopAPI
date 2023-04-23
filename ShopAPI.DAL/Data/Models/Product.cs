namespace ShopAPI.DAL.Data.Models;

public class Product
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public float Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public int Amount { get; set; }
    public string Image { get; set; } = string.Empty;
    public virtual ICollection<OrderProducts>? Orders { get; set; }
}
