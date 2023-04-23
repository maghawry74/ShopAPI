using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopAPI.ActionFilters;
using ShopAPI.BL.DTOs;
using ShopAPI.BL.Managers;


namespace ShopAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    public IOrderManager OrderManager { get; }

    public OrderController(IOrderManager orderManager)
    {
        OrderManager = orderManager;
    }
    [HttpGet]
    [AllowedQueries("state", "shipped", "delivered", "new")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAll([FromQuery] string state)
    {
        return Ok(await OrderManager.GetAll(state));
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        var LoggedClaims = User.Claims;
        return Ok(await OrderManager.GetById(id, LoggedClaims));
    }

    [HttpPost]
    [Authorize(Policy = "UsersOnly")]
    public async Task<IActionResult> Post([FromBody] NewOrderDTO order)
    {
        var Addedorder = await OrderManager.Add(order);
        return CreatedAtAction(nameof(Get), new { Addedorder.Id }, Addedorder);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "UsersOnly")]
    public async Task<IActionResult> ChangeOrderProducts(int id, [FromBody] UpdatedOrderDTO UpdatedOrder)
    {
        var LoggedClaims = User.Claims;
        await OrderManager.ChangeOrderProducts(id, UpdatedOrder, LoggedClaims);
        return NoContent();
    }

    [HttpPatch("{id}")]
    [AllowedQueries("statue", "shipped", "completed")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ChangeOrderStatue(int id, [FromQuery] string Statue)
    {
        await OrderManager.ChangeOrderStatue(id, Statue);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "UsersOnly")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var LoggedClaims = User.Claims;
        await OrderManager.Delete(id, LoggedClaims);
        return NoContent();
    }
}
