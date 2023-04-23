using ShopAPI.BL.DTOs;
using System.Security.Claims;

namespace ShopAPI.BL.Managers;

public interface IOrderManager
{
    public Task<OrderReadDTO> Add(NewOrderDTO newOrder);
    public Task<IEnumerable<OrderReadDTO>> GetAll(string state);
    public Task<OrderReadDTO> GetById(int id, IEnumerable<Claim> claims);
    public Task Delete(int id, IEnumerable<Claim> claims);
    public Task ChangeOrderStatue(int id, string Statue);
    public Task ChangeOrderProducts(int id, UpdatedOrderDTO updatedOrder, IEnumerable<Claim> claims);

}
