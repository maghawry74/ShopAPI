using ShopAPI.DAL.Data.Context;
using ShopAPI.DAL.Data.Models;

namespace ShopAPI.DAL.Repository;

public class OrderProductsRepository : IOrderProductsRepository
{
    public OrderProductsRepository(ShopDbContext context)
    {
        Context = context;
    }

    public ShopDbContext Context { get; }

    public async Task Add(OrderProducts orderProducts)
    {
        await Context.Set<OrderProducts>().AddAsync(orderProducts);
    }

    public async Task SaveChanges()
    {
        await Context.SaveChangesAsync();
    }
}
