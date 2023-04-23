using ShopAPI.BL.DTOs.User;
using System.Security.Claims;

namespace ShopAPI.BL.Managers.User;

public interface IUserManager
{
    public Task<UserReadDTO> Add(NewUserDTO user, string Role);
    public Task<string> Login(UserCredinals credinals);
    public Task<UserReadDTO> GetById(string id, IEnumerable<Claim> claims);
    public Task<IEnumerable<UserReadDTO>> GetAll();

    public Task Delete(string id, IEnumerable<Claim> claims);

    public Task Update(string id, UpdatedUserDTO updatedUser, IEnumerable<Claim> claims);
}
