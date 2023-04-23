

using ShopAPI.DAL.Data.Models;
using ShopAPI.DAL.Repository;

public interface IProductRepository : IRepository<Product>
{
    public Task<IEnumerable<Product>> GetRange(int[] ids);
    public Task<IEnumerable<Product>> GetPage(int skip, int take, bool IsTrackable = false);
}
