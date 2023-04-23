using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ShopAPI.BL.CustomClasses;
using ShopAPI.BL.DTOs;
using ShopAPI.BL.DTOs.User;
using ShopAPI.BL.Managers.User;
using ShopAPI.DAL.Data.Models;
using System.Security.Claims;

namespace ShopAPI.BL.Managers;

public class OrderManager : IOrderManager
{
    public IOrderRepository OrderRepository { get; }
    public IMapper Mapper { get; }
    public UserManager<ApplicationUser> UserManager { get; }
    public IProductManager ProductManager { get; }

    public OrderManager(IOrderRepository orderRepository, IMapper mapper, UserManager<ApplicationUser> userManager, IProductManager productManager)
    {
        OrderRepository = orderRepository;
        Mapper = mapper;
        UserManager = userManager;
        ProductManager = productManager;
    }

    public async Task<OrderReadDTO> Add(NewOrderDTO newOrder)
    {
        var user = await UserManager.FindByIdAsync(newOrder.UserId);
        if (user == null)
            throw new StatusCodeEx(400, "User Not Founded");
        var ProductsIds = newOrder.Products.Select(P => P.ProductId).ToArray();
        var orderProducts = await ProductManager.GetRange(ProductsIds);
        if (orderProducts.Count() != ProductsIds.Length)
            throw new StatusCodeEx(400, "Some Products Not Founded");
        var order = new Order()
        {
            Price = newOrder.Price,
            UserId = user.Id,
        };
        foreach (var product in newOrder.Products)
        {
            order.Products.Add(new OrderProducts()
            {
                ProductId = product.ProductId,
                OrderId = order.Id,
                Quantity = product.Quantity,
            });
        }
        await OrderRepository.Add(order);
        await OrderRepository.SaveChanges();
        var ProductInOrder = Mapper.Map<IEnumerable<OrderProductReadDTO>>(orderProducts).ToList();

        for (int i = 0; i < ProductInOrder.Count(); i++)
        {
            ProductInOrder[i].Quantity = newOrder.Products.ToList()[i].Quantity;
        }
        return new OrderReadDTO()
        {
            Id = order.Id,
            Price = order.Price,
            User = Mapper.Map<UserReadDTO>(user),
            Products = ProductInOrder
        };
    }

    public async Task<IEnumerable<OrderReadDTO>> GetAll(string state)
    {
        IEnumerable<Order> ordersFromDb = new List<Order>();
        if (state.ToLower() == "shipped")
            ordersFromDb = await OrderRepository.GetAllbyCondition(O => O.IsBeingShipped == true);
        if (state.ToLower() == "delivered")
            ordersFromDb = await OrderRepository.GetAllbyCondition(O => O.Delivered == true);
        if (state.ToLower() == "new")
            ordersFromDb = await OrderRepository.GetAllbyCondition(O => (O.IsBeingShipped == false && O.Delivered == false));

        var ordersToRead = new List<OrderReadDTO>();
        foreach (var order in ordersFromDb)
        {
            var orderProducts = new List<OrderProductReadDTO>();
            foreach (var item in order.Products)
            {
                var product = Mapper.Map<OrderProductReadDTO>(item.Product);
                product.Quantity = item.Quantity;
                orderProducts.Add(product);
            }
            ordersToRead.Add(new OrderReadDTO()
            {
                Id = order.Id,
                Price = order.Price,
                IsBeingShipped = order.IsBeingShipped,
                Delivered = order.Delivered,
                User = Mapper.Map<UserReadDTO>(order.User),
                Products = orderProducts
            });
        }
        return ordersToRead;
    }

    public async Task<OrderReadDTO> GetById(int id, IEnumerable<Claim> claims)
    {

        var orderFromDb = await OrderRepository.GetByCondition(O => O.Id == id);
        if (orderFromDb == null)
            throw new StatusCodeEx(404, "Order Not Founded");

        if (!CheckClaims(orderFromDb.UserId, claims))
            throw new StatusCodeEx(400, "This Order doesn't belong to Logged User");

        var orderProducts = new List<OrderProductReadDTO>();
        foreach (var item in orderFromDb.Products)
        {
            var product = Mapper.Map<OrderProductReadDTO>(item.Product);
            product.Quantity = item.Quantity;
            orderProducts.Add(product);
        }

        return new OrderReadDTO()
        {
            Id = orderFromDb.Id,
            Price = orderFromDb.Price,
            IsBeingShipped = orderFromDb.IsBeingShipped,
            Delivered = orderFromDb.Delivered,
            User = Mapper.Map<UserReadDTO>(orderFromDb.User),
            Products = orderProducts
        };
    }

    public async Task Delete(int id, IEnumerable<Claim> claims)
    {
        var LoggedUserId = claims.FirstOrDefault(C => C.Type == ClaimTypes.Sid)?.Value;
        var order = await OrderRepository.GetByCondition(O => O.Id == id);
        if (order == null)
            throw new StatusCodeEx(404, "Order Not Founded");
        if (order.IsBeingShipped)
            throw new StatusCodeEx(400, "Can't Cancel Order As It's Being Shipped");
        if (order.Delivered)
            throw new StatusCodeEx(400, "Can't Cancel Order As It's Been Delivered");
        if (!CheckClaims(order.UserId, claims))
            throw new StatusCodeEx(400, "This Order doesn't belong to Logged User");
        OrderRepository.Delete(order);
        await OrderRepository.SaveChanges();
    }

    public async Task ChangeOrderStatue(int id, string Statue)
    {
        var order = await OrderRepository.GetByCondition(O => O.Id == id, true);
        if (order == null)
            throw new StatusCodeEx(404, "Order Not Founded");
        if (Statue.ToLower() == "shipped")
            order.IsBeingShipped = true;
        if (Statue.ToLower() == "completed")
        {
            order.Delivered = true;
            order.IsBeingShipped = false;
        }
        await OrderRepository.SaveChanges();
    }

    public async Task ChangeOrderProducts(int id, UpdatedOrderDTO updatedOrder, IEnumerable<Claim> claims)
    {
        if (id != updatedOrder.OrderId)
            throw new StatusCodeEx(400, "Paramter Id Doesn't Equal Updated Order");

        var order = await OrderRepository.GetByCondition(O => O.Id == id, true);
        if (order == null)
            throw new StatusCodeEx(404, "Order Not Founded");

        if (order.IsBeingShipped)
            throw new StatusCodeEx(400, "Can't Change Order As It's Being Shipped");

        if (order.Delivered)
            throw new StatusCodeEx(400, "Can't Change Order As It's Been Delivered");

        if (!CheckClaims(order.UserId, claims))
            throw new StatusCodeEx(400, "This Order doesn't belong to Logged User");

        var ProductsIds = updatedOrder.Products.Select(P => P.ProductId).ToArray();
        var orderProducts = await ProductManager.GetRange(ProductsIds);
        if (orderProducts.Count() != ProductsIds.Length)
            throw new StatusCodeEx(400, "Some Products Not Founded");

        ICollection<OrderProducts> newProduct = new List<OrderProducts>();
        foreach (var product in updatedOrder.Products)
        {
            newProduct.Add(new OrderProducts()
            {
                ProductId = product.ProductId,
                OrderId = order.Id,
                Quantity = product.Quantity,
            });
        }
        order.Products = newProduct;
        await OrderRepository.SaveChanges();
    }


    bool CheckClaims(string id, IEnumerable<Claim> claims)
    {
        var Role = claims.FirstOrDefault(C => C.Type == ClaimTypes.Role)?.Value;
        var userId = claims.FirstOrDefault(C => C.Type == ClaimTypes.Sid)?.Value;
        if (Role != "Admin" && userId != id)
        {
            return false;
        }
        return true;
    }
}
