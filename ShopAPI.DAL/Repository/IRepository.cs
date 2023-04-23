using System.Linq.Expressions;

namespace ShopAPI.DAL.Repository;

public interface IRepository<T> where T : class
{
    public Task<IEnumerable<T>> GetAll(bool IsTrackable = false);
    public Task<T?> GetByCondition(Expression<Func<T, bool>> condition, bool IsTrackable = false);
    public Task Add(T entity);
    public void Update(T entity);
    public void Delete(T entity);
    public Task SaveChanges();
}
