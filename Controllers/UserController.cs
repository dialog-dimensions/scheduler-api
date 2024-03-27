using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchedulerApi.DAL;
using SchedulerApi.Models.DTOs;
using SchedulerApi.Models.ViewModels.Account;
using SchedulerApi.Services.JWT;

namespace SchedulerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApiDbContext _context;
    private readonly IJwtGenerator _jwtGenerator;

    public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager, ApiDbContext context, IJwtGenerator jwtGenerator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _context = context;
        _jwtGenerator = jwtGenerator;
    }

    [HttpPost("/api/[controller]/Register")]
    public async Task<IActionResult> RegisterAsync(RegisterModel model)
    {
        var employee = await _context.Employees.FindAsync(int.Parse(model.Id!));
        if (employee is null) return Unauthorized(new { Message = "Must be an employee to register." });

        var role = employee.Role;
        if (!await _roleManager.RoleExistsAsync(role))
            return BadRequest(new { Message = "Employee role is not a legal role." });

        var user = new IdentityUser
        {
            Id = model.Id!,
            UserName = model.Id,
            PhoneNumber = model.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, model.Password!);
        if (!result.Succeeded)
        {
            Console.WriteLine("Error creating user");
            foreach (var error in result.Errors)
            {
                Console.WriteLine(error.Description);
            }

            return Problem(string.Join("\n", result.Errors.Select(err => err.Description)));
        }

        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded) return Problem("Role assignment failed.");

        return Ok(new { Message = "Account created successfully." });
    }

    [HttpPost("/api/[controller]/Login")]
    public async Task<ActionResult<string>> LoginAsync(LoginModel model)
    {
        var user = await _userManager.FindByIdAsync(model.Id!);
        if (user is null) return Unauthorized("Username or password isn't correct.");

        var result = await _signInManager.PasswordSignInAsync(user, model.Password!, false, false);
        if (!result.Succeeded) return Unauthorized(new { Message = "Username or password isn't correct." });

        var token = await _jwtGenerator.Generate(user);
        return token;
    }

    [HttpPost("/api/[controller]/Logout")]
    [Authorize]
    public async Task<IActionResult> LogoutAsync()
    {
        if (!_signInManager.IsSignedIn(User)) return Unauthorized("Requester already logged out.");
        await _signInManager.SignOutAsync();
        return NoContent();
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersAsync()
    {
        var employees = await _userManager.GetUsersInRoleAsync("Employee");
        var managers = await _userManager.GetUsersInRoleAsync("Manager");
        var admins = await _userManager.GetUsersInRoleAsync("Admin");

        var allUsers = new List<IdentityUser>();
        allUsers.AddRange(employees);
        allUsers.AddRange(managers);
        allUsers.AddRange(admins);

        return allUsers.Select(UserDto.FromEntity).ToList();
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetUserAsync(string id)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var loggedInUserId))
            return Unauthorized(new { Message = "Invalid token." });

        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (userRole is not ("Employee" or "Manager" or "Admin"))
            return Unauthorized(new { Message = "Invalid token." });

        if (userRole is "Employee" & loggedInUserId.ToString() != id) return Forbid();

        var user = await _userManager.FindByIdAsync(id);
        
        return user is null 
            ? NotFound(new { Message = "User not found in database.", Id = id }) 
            : UserDto.FromEntity(user);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return BadRequest("User already does not exist.");

        var roles = await _userManager.GetRolesAsync(user);
        var rolesResult = await _userManager.RemoveFromRolesAsync(user, roles);
        if (!rolesResult.Succeeded) return Problem("User roles removal failed.");
        
        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded ? NoContent() : Problem("Delete failed.");
    }
}
