using CiOHomePage.Server.Data;
using CiOHomePage.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CiOHomePage.Server.Extensions;

public static class ServiceCollectionExtensions
{
 public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
 {
 var cs = config.GetConnectionString("Default") ?? "Data Source=app.db";
 services.AddDbContext<AppDbContext>(o => o.UseSqlite(cs));
 services.AddIdentityCore<IdentityUser>(options =>
 {
 options.Password.RequireNonAlphanumeric = false;
 options.Password.RequireDigit = false;
 options.Password.RequiredLength =6;
 options.Password.RequireUppercase = false;
 options.Password.RequireLowercase = false;
 })
 .AddRoles<IdentityRole>()
 .AddEntityFrameworkStores<AppDbContext>()
 .AddSignInManager();

 var key = config["Jwt:Key"] ?? "dev_secret_change_me";
 var issuer = config["Jwt:Issuer"] ?? "CiOHomePage";
 var audience = config["Jwt:Audience"] ?? "CiOHomePageClient";
 services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(o =>
 {
 o.TokenValidationParameters = new TokenValidationParameters
 {
 ValidateIssuerSigningKey = true,
 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
 ValidateIssuer = true,
 ValidIssuer = issuer,
 ValidateAudience = true,
 ValidAudience = audience,
 ValidateLifetime = true,
 ClockSkew = TimeSpan.FromMinutes(2)
 };
 });
 services.AddAuthorization();

 services.AddScoped<IEmailSender, SmtpEmailSender>();
 return services;
 }
}
