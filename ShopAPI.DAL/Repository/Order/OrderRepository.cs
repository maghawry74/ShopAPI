using Microsoft.EntityFrameworkCore;
using ShopAPI.DAL.Data.Context;
using ShopAPI.DAL.Data.Models;
using System.Linq.Expressions;

namespace ShopAPI.DAL.Repository;

public class OrderRepository : IOrderRepository
{
    public OrderRepository(ShopDbContext context)
    {
        Context = context;
    }

    public ShopDbContext Context { get; }

    public async Task Add(Order entity)
    {
        await Context.Set<Order>().AddAsync(entity);
    }

    public void Delete(Order entity)
    {
        Context.Set<Order>().Remove(entity);
    }

    public async Task<IEnumerable<Order>> GetAll(bool IsTrackable = false)
    {
        return IsTrackable ?
            await Context.Set<Order>().Include(O => O.User).Include(O => O.Products).ThenInclude(OP => OP.Product).ToListAsync()
            :
            await Context.Set<Order>().Include(O => O.User).Include(O => O.Products).ThenInclude(OP => OP.Product).AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllbyCondition(Expression<Func<Order, bool>> condition, bool IsTrackable = false)
    {
        return IsTrackable ?
    await Context.Set<Order>().Include(O => O.User).Include(O => O.Products).ThenInclude(OP => OP.Product).Where(condition).ToListAsync()
    :
    await Context.Set<Order>().Include(O => O.User).Include(O => O.Products).ThenInclude(OP => OP.Product).Where(condition).AsNoTracking().ToListAsync();
    }

    public async Task<Order?> GetByCondition(Expression<Func<Order, bool>> condition, bool IsTrackable = false)
    {
        return IsTrackable ?
            await Context.Set<Order>().Include(O => O.User).Include(O => O.Products).ThenInclude(OP => OP.Product).FirstOrDefaultAsync(condition)
            :
            await Context.Set<Order>().Include(O => O.User).Include(O => O.Products).ThenInclude(OP => OP.Product).AsNoTracking().FirstOrDefaultAsync(condition);
    }

    public async Task SaveChanges()
    {
        await Context.SaveChangesAsync();
    }

    public void Update(Order entity)
    {

    }
}
