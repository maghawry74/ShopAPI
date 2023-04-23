using DataAnnotationsExtensions;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ShopAPI.BL.DTOs;

public class NewProductDTO
{
    [Required]
    public string ProductName { get; set; }
    [Required]
    [MaxLength(100)]
    [MinLength(10)]
    public string Description { get; set; }
    [Required]
    [Min(1)]
    public float Price { get; set; }
    [Required]
    public string Category { get; set; }
    [Required]
    [Min(1)]
    public int Amount { get; set; }
    [Required]
    public IFormFile ImageFile { get; set; }
}

public class ProductReadDTO
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public float Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public int Amount { get; set; }
    public string Image { get; set; } = string.Empty;
}
public class ProductUpdateDTO
{
    [Required]
    public int Id { get; set; }
    public string ProductName { get; set; }
    [Required]
    [MaxLength(100)]
    [MinLength(10)]
    public string Description { get; set; }
    [Required]
    [Min(1)]
    public float Price { get; set; }
    [Required]
    public string Category { get; set; }
    [Required]
    [Min(1)]
    public int Amount { get; set; }

    public IFormFile? ImageFile { get; set; }
}

public class OrderProductDTO
{
    public int ProductId { get; set; }
    [Min(1)]
    public int Quantity { get; set; }
}

public class OrderProductReadDTO : ProductReadDTO
{
    public int Quantity { set; get; }
}