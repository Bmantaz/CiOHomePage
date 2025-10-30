using CiOHomePage.Server.Data;
using CiOHomePage.Server.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS (restrict to configured client origins)
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
 options.AddPolicy("ClientPolicy", policy =>
 {
 policy.WithOrigins(allowedOrigins)
 .AllowAnyHeader()
 .AllowAnyMethod();
 });
});

var app = builder.Build();

// Apply database migrations
using (var scope = app.Services.CreateScope())
{
 var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
 db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
 app.UseSwagger();
 app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("ClientPolicy");
app.UseAuthentication();
app.UseAuthorization();

// Redirect root to Swagger so hitting http://localhost:PORT shows UI
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.MapControllers();

app.Run();
