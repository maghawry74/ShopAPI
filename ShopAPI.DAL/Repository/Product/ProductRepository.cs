using Microsoft.EntityFrameworkCore;
using ShopAPI.DAL.Data.Context;
using ShopAPI.DAL.Data.Models;
using System.Linq.Expressions;

public class ProductRepository : IProductRepository
{
    public ProductRepository(ShopDbContext context)
    {
        Context = context;
    }

    public ShopDbContext Context { get; }

    public async Task Add(Product entity)
    {
        await Context.Products.AddAsync(entity);
    }

    public void Delete(Product entity)
    {
        Context.Remove(entity);
    }

    public async Task<IEnumerable<Product>> GetAll(bool IsTrackable = false)
    {
        return IsTrackable ?
            await Context.Set<Product>().ToListAsync()
            :
            await Context.Set<Product>().AsNoTracking().ToListAsync();
    }

    public async Task<Product?> GetByCondition(Expression<Func<Product, bool>> condition, bool IsTrackable = false)
    {
        return IsTrackable ?
            await Context.Set<Product>().FirstOrDefaultAsync(condition)
            :
            await Context.Set<Product>().AsNoTracking().FirstOrDefaultAsync(condition);
    }

    public async Task<IEnumerable<Product>> GetPage(int skip, int take, bool IsTrackable = false)
    {
        return IsTrackable ?
            await Context.Set<Product>().Skip(skip).Take(take).ToListAsync()
            :
            await Context.Set<Product>().Skip(skip).Take(take).AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetRange(int[] ids)
    {
        return await Context.Set<Product>().Where(P => ids.Contains(P.Id)).ToListAsync();
    }

    public async Task SaveChanges()
    {
        await Context.SaveChangesAsync();
    }

    public void Update(Product entity)
    {

    }
}
