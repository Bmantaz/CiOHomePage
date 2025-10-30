using CiOHomePage.Server.Data;
using CiOHomePage.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

const string ClientAppPolicy = "ClientApp";

var builder = WebApplication.CreateBuilder(args);

// CORS: allow only the development WASM client origin
builder.Services.AddCors(options =>
{
 options.AddPolicy(name: ClientAppPolicy, policy =>
 {
 policy.WithOrigins(
 "http://localhost",
 "https://localhost")
 .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
 .AllowAnyHeader()
 .AllowAnyMethod();
 });
});

// DbContext (SQLite)
var cs = builder.Configuration.GetConnectionString("Default") ?? "Data Source=app.db";
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(cs));

// Identity + Roles
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
.AddDefaultTokenProviders();

// JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
 options.TokenValidationParameters = new()
 {
 ValidateIssuer = true,
 ValidateAudience = true,
 ValidateLifetime = true,
 ValidateIssuerSigningKey = true,
 ValidAudience = builder.Configuration["Jwt:Audience"],
 ValidIssuer = builder.Configuration["Jwt:Issuer"],
 IssuerSigningKey = new SymmetricSecurityKey(
 Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]
 ?? throw new InvalidOperationException("JWT Key not found"))
 )
 };
 });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

var app = builder.Build();

// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
 var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
 try
 {
 db.Database.Migrate();
 }
 catch (SqliteException ex) when (
 app.Environment.IsDevelopment() &&
 ex.SqliteErrorCode ==1 &&
 ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
 {
 // Dev-only fallback when an existing SQLite DB has tables but no migration history
 Console.Error.WriteLine("[Dev] Migration failed due to existing tables without migration history. Falling back to EnsureCreated().");
 db.Database.EnsureCreated();
 }
}

if (app.Environment.IsDevelopment())
{
 app.UseSwagger();
 app.UseSwaggerUI();
}
else
{
 app.UseExceptionHandler("/Error");
 app.UseHsts();
}

// In dev we can run HTTP only to avoid cert issues
// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors(ClientAppPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Helpful dev default: redirect root to Swagger so the browser shows a page
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.Run();