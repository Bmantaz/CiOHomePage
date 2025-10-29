using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CiOHomePage.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
 UserManager<IdentityUser> userManager,
 SignInManager<IdentityUser> signInManager,
 IConfiguration config) : ControllerBase
{
 private readonly UserManager<IdentityUser> _userManager = userManager;
 private readonly SignInManager<IdentityUser> _signInManager = signInManager;
 private readonly IConfiguration _config = config;

 [HttpPost("register")]
 public async Task<IActionResult> Register(RegisterRequest request)
 {
 var user = new IdentityUser { UserName = request.Username, Email = request.Email };
 var result = await _userManager.CreateAsync(user, request.Password);
 if (!result.Succeeded) return BadRequest(result.Errors);
 await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "BandMember"));
 return Ok();
 }

 [HttpPost("login")]
 public async Task<IActionResult> Login(LoginRequest request)
 {
 var user = await _userManager.FindByNameAsync(request.Username);
 if (user == null) return Unauthorized();
 var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
 if (!result.Succeeded) return Unauthorized();
 var token = GenerateJwt(user);
 return Ok(new { token });
 }

 private string GenerateJwt(IdentityUser user)
 {
 var key = _config["Jwt:Key"] ?? "dev_secret_change_me";
 var issuer = _config["Jwt:Issuer"] ?? "CiOHomePage";
 var audience = _config["Jwt:Audience"] ?? "CiOHomePageClient";
 var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
 var claims = new List<Claim>
 {
 new(ClaimTypes.NameIdentifier, user.Id),
 new(ClaimTypes.Name, user.UserName ?? user.Id),
 new(ClaimTypes.Role, "BandMember")
 };
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
