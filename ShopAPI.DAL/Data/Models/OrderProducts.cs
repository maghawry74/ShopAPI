using Microsoft.EntityFrameworkCore;

namespace ShopAPI.DAL.Data.Models;

[PrimaryKey(nameof(OrderId), nameof(ProductId))]
public class OrderProducts
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }
    public virtual Order? Order { get; set; }
    public int Quantity { get; set; }
}
