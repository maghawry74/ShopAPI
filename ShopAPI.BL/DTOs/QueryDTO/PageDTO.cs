using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations;

namespace ShopAPI.BL.DTOs.QueryDTO;

public class PageDTO
{
    [Required]
    [Min(1)]
    public int Skip { get; set; }
    [Required]
    [Min(1)]
    public int Take { get; set; }
}
