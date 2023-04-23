using System.ComponentModel.DataAnnotations;

namespace ShopAPI.BL.DTOs.User;

public class UserReadDTO
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string City { set; get; }
    public string Governorate { set; get; }
}
public class NewUserDTO
{
    [Required]
    public string UserName { get; set; }
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required]
    [RegularExpression(@"^01[1|2|5|0]\d{8}")]
    public string PhoneNumber { get; set; }
    [Required]
    [MinLength(5)]
    public string FirstName { get; set; }
    [Required]
    [MinLength(5)]
    public string LastName { get; set; }
    [Required]
    public string City { set; get; }
    [Required]
    public string Governorate { set; get; }
    [Required]
    public string Password { set; get; }
}
public class UserCredinals
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
public class UpdatedUserDTO
{
    public string Id { get; set; }
    public string UserName { get; set; }
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [RegularExpression(@"^01[1|2|5|0]\d{8}")]
    public string PhoneNumber { get; set; }
    [MinLength(5)]
    public string FirstName { get; set; }
    [MinLength(5)]
    public string LastName { get; set; }
    public string City { set; get; }
    public string Governorate { set; get; }
    public string? OldPassword { set; get; }
    public string? NewPassword { set; get; }
}