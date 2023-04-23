using Microsoft.AspNetCore.Identity;

namespace ShopAPI.DAL.Data.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string City { set; get; } = string.Empty;
    public string Governorate { set; get; } = string.Empty;
    public virtual ICollection<Order>? Orders { get; set; }
}

