using ShopAPI.DAL.Data.Models;

namespace ShopAPI.DAL.Repository;

public interface IOrderProductsRepository
{
    public Task Add(OrderProducts orderProducts);
    public Task SaveChanges();
}
