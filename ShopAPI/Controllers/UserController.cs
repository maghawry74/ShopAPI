using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopAPI.BL.DTOs.User;
using ShopAPI.BL.Managers.User;

namespace ShopAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    public IUserManager Manager { get; }
    public IMapper Mapper { get; }
    public IConfiguration Configuration { get; }

    public UserController(IUserManager manager, IMapper mapper, IConfiguration configuration)
    {
        Manager = manager;
        Mapper = mapper;
        Configuration = configuration;
    }
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAll()
    {
        var users = await Manager.GetAll();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var LoggedClaims = User.Claims;
        var user = await Manager.GetById(id, LoggedClaims);
        return Ok(user);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UpdatedUserDTO updatedUser)
    {
        var LoggedClaims = User.Claims;

        await Manager.Update(id, updatedUser, LoggedClaims);
        return NoContent();
    }
    [HttpPost]
    [Route("Account/User")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUser([FromBody] NewUserDTO newUser)
    {
        var AddedUser = await Manager.Add(newUser, "User");
        return CreatedAtAction(nameof(Get), new { AddedUser.Id }, AddedUser);
    }
    [HttpPost]
    [Route("Account/Admin")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAdmin([FromBody] NewUserDTO newUser)
    {
        var AddedUser = await Manager.Add(newUser, "Admin");
        return CreatedAtAction(nameof(Get), new { AddedUser.Id }, AddedUser);
    }
    [HttpPost]
    [Route("Login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(UserCredinals credinals)
    {
        string Token = await Manager.Login(credinals);
        return Ok(new { Token });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var LoggedClaims = User.Claims;
        await Manager.Delete(id, LoggedClaims);
        return NoContent();
    }

}
