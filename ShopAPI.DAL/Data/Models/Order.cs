using System.ComponentModel;

namespace ShopAPI.DAL.Data.Models;

public class Order
{
    public int Id { get; set; }
    public float Price { get; set; }
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser? User { get; set; }
    [DefaultValue(false)]
    public bool IsBeingShipped { get; set; }
    [DefaultValue(false)]
    public bool Delivered { get; set; }
    public virtual ICollection<OrderProducts> Products { get; set; } = new HashSet<OrderProducts>();
}
