using CiOHomePage.Client.Pages;
using CiOHomePage.Components;
using CiOHomePage.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Razor + WASM
builder.Services.AddRazorComponents()
 .AddInteractiveWebAssemblyComponents();

// API + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext (SQLite)
var cs = builder.Configuration.GetConnectionString("Default") ?? "Data Source=app.db";
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite(cs));

// Identity
builder.Services.AddIdentityCore<IdentityUser>(options =>
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

// JWT
var key = builder.Configuration["Jwt:Key"] ?? "dev_secret_change_me";
var issuer = builder.Configuration["Jwt:Issuer"] ?? "CiOHomePage";
var audience = builder.Configuration["Jwt:Audience"] ?? "CiOHomePageClient";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

builder.Services.AddAuthorization();

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Create DB
using (var scope = app.Services.CreateScope())
{
 var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
 db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
 app.UseWebAssemblyDebugging();
}
else
{
 app.UseExceptionHandler("/Error", createScopeForErrors: true);
 app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
 .AddInteractiveWebAssemblyRenderMode()
 .AddAdditionalAssemblies(typeof(CiOHomePage.Client._Imports).Assembly);

app.Run();
