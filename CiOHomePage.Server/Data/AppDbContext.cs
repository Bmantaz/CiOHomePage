using CiOHomePage.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CiOHomePage.Server.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
 public DbSet<CalendarEvent> CalendarEvents => Set<CalendarEvent>();
 public DbSet<EventRsvp> EventRsvps => Set<EventRsvp>();
 public DbSet<MerchSale> MerchSales => Set<MerchSale>();

 protected override void OnModelCreating(ModelBuilder builder)
 {
 base.OnModelCreating(builder);
 builder.Entity<CalendarEvent>()
 .HasIndex(e => e.StartUtc);
 builder.Entity<EventRsvp>()
 .HasIndex(r => new { r.EventId, r.UserId })
 .IsUnique();
 }
}
