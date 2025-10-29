using CiOHomePage.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CiOHomePage.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
 : IdentityDbContext<IdentityUser>(options)
{
 public DbSet<CalendarEvent> CalendarEvents => Set<CalendarEvent>();

 protected override void OnModelCreating(ModelBuilder builder)
 {
 base.OnModelCreating(builder);
 builder.Entity<CalendarEvent>()
 .HasIndex(e => e.StartUtc);
 }
}
