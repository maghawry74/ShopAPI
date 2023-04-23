using ShopAPI.BL.DTOs;
using ShopAPI.DAL.Data.Models;

namespace ShopAPI.BL.Managers;

public interface IProductManager
{
    public Task<ProductReadDTO> AddAsync(NewProductDTO newProduct);
    public Task<ProductReadDTO?> GetByConditionAsync(int id);
    public Task<IEnumerable<ProductReadDTO>> GetAllAsync(int skip, int take);
    public Task DeleteAsync(int id);
    public Task UpdateAsync(int id, ProductUpdateDTO product);
    public Task<IEnumerable<Product>> GetRange(int[] ids);
}
