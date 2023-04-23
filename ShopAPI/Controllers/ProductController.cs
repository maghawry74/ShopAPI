using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopAPI.ActionFilters;
using ShopAPI.BL.DTOs;
using ShopAPI.BL.DTOs.QueryDTO;
using ShopAPI.BL.Managers;

namespace ShopAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "AdminOnly")]
public class ProductController : ControllerBase
{
    public IProductManager Manager { get; }

    public ProductController(IProductManager manager)
    {
        Manager = manager;
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] PageDTO Page)
    {
        var products = await Manager.GetAllAsync(Page.Skip, Page.Take);
        return Ok(products);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(int id)
    {
        var product = await Manager.GetByConditionAsync(id);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpPost]
    [FilterImages]

    public async Task<IActionResult> Post([FromForm] NewProductDTO newProduct)
    {
        var addedProduct = await Manager.AddAsync(newProduct);
        return CreatedAtAction(nameof(Get), new { addedProduct.Id }, addedProduct);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromForm] ProductUpdateDTO product)
    {
        if (id != product.Id) return BadRequest("Paramter Id Not Equal Product Id");
        await Manager.UpdateAsync(id, product);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await Manager.DeleteAsync(id);
        return NoContent();
    }
}
