using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShopAPI.BL.CustomClasses;
using ShopAPI.BL.DTOs.User;
using ShopAPI.DAL.Data.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShopAPI.BL.Managers.User;

public class UserManager : IUserManager
{
    public IMapper Mapper { get; }
    public IConfiguration Configuration { get; }
    public UserManager<ApplicationUser> Manager { get; }
    public UserManager(UserManager<ApplicationUser> manager, IMapper mapper, IConfiguration configuration)
    {
        Manager = manager;
        Mapper = mapper;
        Configuration = configuration;
    }

    public async Task<UserReadDTO> Add(NewUserDTO user, string Role)
    {
        return await Register(user, Role);
    }

    public async Task<string> Login(UserCredinals credinals)
    {
        var user = await Manager.FindByEmailAsync(credinals.Email);
        if (user == null)
        {
            throw new StatusCodeEx(401, "Email Was Not Found");
        }
        var result = await Manager.CheckPasswordAsync(user, credinals.Password);
        if (result == false)
        {
            throw new StatusCodeEx(401, "Password Is Incorrect");
        }
        var UserClaims = await Manager.GetClaimsAsync(user);
        var Token = GetToken(UserClaims);
        return Token;
    }
    async Task<UserReadDTO> Register(NewUserDTO newUser, string Role)
    {
        var User = Mapper.Map<ApplicationUser>(newUser);
        var Result = await Manager.CreateAsync(User, newUser.Password);
        if (!Result.Succeeded)
        {
            string ErrorMessage = "";
            foreach (var item in Result.Errors)
            {
                ErrorMessage += $"{item.Description} ";
            }
            throw new StatusCodeEx(500, ErrorMessage);
        }
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier,User.UserName!),
            new Claim(ClaimTypes.Sid,User.Id!),
            new Claim(ClaimTypes.Role,Role),
        };
        Result = await Manager.AddClaimsAsync(User, claims);
        if (!Result.Succeeded)
        {
            string ErrorMessage = "";
            foreach (var item in Result.Errors)
            {
                ErrorMessage += $"{item.Description} ";
            }
            throw new StatusCodeEx(500, ErrorMessage);
        }
        return Mapper.Map<UserReadDTO>(User);
    }
    string GetToken(IList<Claim>? claims)
    {
        var SecurityKey = new SymmetricSecurityKey(
                                    Encoding.ASCII.GetBytes(
                                        Configuration["Authentication:SecurityKey"]!));

        var singingCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            signingCredentials: singingCredentials,
            claims: claims,
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddDays(1));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<UserReadDTO> GetById(string id, IEnumerable<Claim> claims)
    {
        if (!CheckClaims(id, claims)) throw new StatusCodeEx(401, "Token Id Doesn't Equal Paramter Id");
        var user = await Manager.FindByIdAsync(id);
        if (user == null)
        {
            throw new StatusCodeEx(404);
        }
        return Mapper.Map<UserReadDTO>(user);
    }

    public async Task Delete(string id, IEnumerable<Claim> claims)
    {
        if (!CheckClaims(id, claims)) throw new StatusCodeEx(401, "Token Id Doesn't Equal Paramter Id");
        var User = await Manager.FindByIdAsync(id);
        if (User == null)
        {
            throw new StatusCodeEx(404);
        }
        var result = await Manager.DeleteAsync(User);
        if (!result.Succeeded)
        {
            string ErrorMessage = "";
            foreach (var item in result.Errors)
            {
                ErrorMessage += $"{item.Description} \n";
            }
            throw new StatusCodeEx(500, ErrorMessage);
        }
    }

    public async Task Update(string id, UpdatedUserDTO updatedUser, IEnumerable<Claim> claims)
    {
        if (!CheckClaims(id, claims)) throw new StatusCodeEx(401, "Token Id Doesn't Equal Paramter Id");
        if (id != updatedUser.Id) throw new StatusCodeEx(400, "Paramter Id Not Equal User Id");
        var user = await Manager.FindByIdAsync(id);
        if (user == null) throw new StatusCodeEx(404);
        user.PhoneNumber = updatedUser.PhoneNumber;
        user.UserName = updatedUser.UserName;
        user.Email = updatedUser.Email;
        user.FirstName = updatedUser.FirstName;
        user.LastName = updatedUser.LastName;
        user.City = updatedUser.City;
        user.Governorate = updatedUser.Governorate;
        if (updatedUser.NewPassword != null && updatedUser.OldPassword != null)
        {
            var result = await Manager.ChangePasswordAsync(user, updatedUser.OldPassword, updatedUser.NewPassword);
            string ErrorMessage = "";
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ErrorMessage += $"{item.Description} \n";
                }
                throw new StatusCodeEx(400, ErrorMessage);
            }
        }
        var updateResult = await Manager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            string ErrorMessage = "";
            foreach (var item in updateResult.Errors)
            {
                ErrorMessage += $"{item.Description}\n";
            }
            throw new StatusCodeEx(500, ErrorMessage);
        }
    }

    public async Task<IEnumerable<UserReadDTO>> GetAll()
    {
        var users = await Manager.Users.ToListAsync();
        return Mapper.Map<IEnumerable<UserReadDTO>>(users);
    }


    bool CheckClaims(string id, IEnumerable<Claim> claims)
    {
        var Role = claims.FirstOrDefault(C => C.Type == ClaimTypes.Role)?.Value;
        var userId = claims.FirstOrDefault(C => C.Type == ClaimTypes.Sid)?.Value;
        if (Role != "Admin" && userId != id)
        {
            return false;
        }
        return true;
    }
}
