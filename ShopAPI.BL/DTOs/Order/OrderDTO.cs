using DataAnnotationsExtensions;
using ShopAPI.BL.DTOs.User;
using System.ComponentModel.DataAnnotations;

namespace ShopAPI.BL.DTOs;

public class OrderReadDTO
{
    public int Id { get; set; }
    public UserReadDTO User { get; set; }
    public List<OrderProductReadDTO> Products { get; set; }
    public bool IsBeingShipped { get; set; }
    public bool Delivered { get; set; }
    public float Price { get; set; }

}

public class NewOrderDTO
{
    [Required]
    public string UserId { get; set; } = string.Empty;
    [Required]
    [Min(1)]
    public float Price { get; set; }
    [ArrayLength(1, ErrorMessage = "Minimun Order Products is 1 ")]
    public IEnumerable<OrderProductDTO> Products { get; set; }
}
public class UpdatedOrderDTO
{
    [Required]
    public int OrderId { get; set; }
    public float Price { get; set; }
    [ArrayLength(1, ErrorMessage = "Minimun Order Products is 1 ")]
    public IEnumerable<OrderProductDTO> Products { get; set; }
}
