using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CiOHomePage.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration config) : ControllerBase
{
 private readonly UserManager<IdentityUser> _userManager = userManager;
 private readonly SignInManager<IdentityUser> _signInManager = signInManager;
 private readonly IConfiguration _config = config;

 [HttpPost("register")]
 public async Task<IActionResult> Register(RegisterRequest request)
 {
 var user = new IdentityUser { UserName = request.Username, Email = request.Email };
 var result = await _userManager.CreateAsync(user, request.Password);
 if (!result.Succeeded)
 {
 return BadRequest(result.Errors);
 }
 // Optionally add a default role using Identity Roles rather than raw claim
 // await _userManager.AddToRoleAsync(user, "BandMember");
 return Ok();
 }

 [HttpPost("login")]
 public async Task<IActionResult> Login(LoginRequest request)
 {
 var user = await _userManager.FindByNameAsync(request.Username);
 if (user == null) return Unauthorized();
 var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
 if (!result.Succeeded) return Unauthorized();
 var token = await GenerateJwtAsync(user);
 return Ok(new { token });
 }

 private async Task<string> GenerateJwtAsync(IdentityUser user)
 {
 var key = _config["Jwt:Key"] ?? throw new InvalidOperationException("Configuration 'Jwt:Key' is required.");
 var issuer = _config["Jwt:Issuer"] ?? throw new InvalidOperationException("Configuration 'Jwt:Issuer' is required.");
 var audience = _config["Jwt:Audience"] ?? throw new InvalidOperationException("Configuration 'Jwt:Audience' is required.");
 var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
 var claims = new List<Claim>
 {
 new(ClaimTypes.NameIdentifier, user.Id),
 new(ClaimTypes.Name, user.UserName ?? user.Id)
 };
 var roles = await _userManager.GetRolesAsync(user);
 foreach (var role in roles)
 {
 claims.Add(new Claim(ClaimTypes.Role, role));
 }
 var token = new JwtSecurityToken(
 issuer: issuer,
 audience: audience,
 claims: claims,
 expires: DateTime.UtcNow.AddHours(12),
 signingCredentials: creds
 );
 return new JwtSecurityTokenHandler().WriteToken(token);
 }

 public sealed record RegisterRequest(string Username, string Email, string Password);
 public sealed record LoginRequest(string Username, string Password);
}
