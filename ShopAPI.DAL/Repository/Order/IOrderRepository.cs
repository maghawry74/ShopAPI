
using ShopAPI.DAL.Data.Models;
using ShopAPI.DAL.Repository;
using System.Linq.Expressions;

public interface IOrderRepository : IRepository<Order>
{
    public Task<IEnumerable<Order>> GetAllbyCondition(Expression<Func<Order, bool>> condition, bool IsTrackable = false);
}
