using CiOHomePage.Server.Data;
using CiOHomePage.Server.Models;
using CiOHomePage.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CiOHomePage.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CalendarController(AppDbContext db, IEmailSender emailSender, UserManager<IdentityUser> userManager) : ControllerBase
{
 private readonly AppDbContext _db = db;
 private readonly IEmailSender _email = emailSender;
 private readonly UserManager<IdentityUser> _users = userManager;

 [HttpGet]
 public async Task<ActionResult<IEnumerable<CalendarEvent>>> Get()
 {
 var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
 var query = _db.CalendarEvents.AsNoTracking().Include(e => e.Rsvps).Where(e => e.IsBandWide || e.CreatedByUserId == uid);
 var list = await query.OrderBy(e => e.StartUtc).ToListAsync();
 return Ok(list);
 }

 [HttpPost]
 public async Task<ActionResult<CalendarEvent>> Create(CalendarEvent evt)
 {
 var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
 evt.CreatedByUserId ??= uid ?? string.Empty;
 _db.CalendarEvents.Add(evt);
 await _db.SaveChangesAsync();

 // Notify other band members by email (best-effort)
 var allUsers = _users.Users.ToList();
 foreach (var u in allUsers.Where(u => u.Id != uid && !string.IsNullOrWhiteSpace(u.Email)))
 {
 var subject = $"New {evt.Category} - {evt.Title}";
 var baseUrl = $"{Request.Scheme}://{Request.Host}";
 var accept = $"{baseUrl}/api/calendar/{evt.Id}/rsvp?status=attending";
 var decline = $"{baseUrl}/api/calendar/{evt.Id}/rsvp?status=not";
 var body = $"Event: {evt.Title}\nCategory: {evt.Category}\nWhen: {evt.StartUtc:u} - {evt.EndUtc:u}\nLocation: {evt.Location}\nNotes: {evt.Notes}\n\nRSVP: Attending {accept} | Not Attending {decline}";
 await _email.SendAsync(u.Email!, subject, body);
 }

 return CreatedAtAction(nameof(GetById), new { id = evt.Id }, evt);
 }

 [HttpGet("{id}")]
 public async Task<ActionResult<CalendarEvent>> GetById(Guid id)
 {
 var evt = await _db.CalendarEvents.Include(e => e.Rsvps).FirstOrDefaultAsync(e => e.Id == id);
 if (evt == null) return NotFound();
 var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
 if (!evt.IsBandWide && evt.CreatedByUserId != uid) return Forbid();
 return Ok(evt);
 }

 [HttpPut("{id}")]
 public async Task<IActionResult> Update(Guid id, CalendarEvent update)
 {
 if (id != update.Id) return BadRequest();
 var evt = await _db.CalendarEvents.FindAsync(id);
 if (evt == null) return NotFound();
 var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
 if (evt.CreatedByUserId != uid) return Forbid();
 evt.Title = update.Title;
 evt.Type = update.Type;
 evt.StartUtc = update.StartUtc;
 evt.EndUtc = update.EndUtc;
 evt.Location = update.Location;
 evt.Notes = update.Notes;
 evt.IsBandWide = update.IsBandWide;
 evt.Category = update.Category;
 await _db.SaveChangesAsync();
 return NoContent();
 }

 [HttpDelete("{id}")]
 public async Task<IActionResult> Delete(Guid id)
 {
 var evt = await _db.CalendarEvents.FindAsync(id);
 if (evt == null) return NotFound();
 var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
 if (evt.CreatedByUserId != uid) return Forbid();
 _db.CalendarEvents.Remove(evt);
 await _db.SaveChangesAsync();
 return NoContent();
 }

 // RSVP endpoints via email links (no auth required)
 [AllowAnonymous]
 [HttpGet("{id}/rsvp")]
 public async Task<IActionResult> Rsvp(Guid id, [FromQuery] string status)
 {
 var evt = await _db.CalendarEvents.FindAsync(id);
 if (evt == null) return NotFound();
 // For simplicity, record anonymous RSVP. In real app, map token -> user.
 var userId = "anonymous";
 var rsvp = await _db.EventRsvps.FirstOrDefaultAsync(r => r.EventId == id && r.UserId == userId);
 var parsed = status?.StartsWith("att", StringComparison.OrdinalIgnoreCase) == true ? RsvpStatus.Attending : RsvpStatus.NotAttending;
 if (rsvp == null)
 {
 _db.EventRsvps.Add(new EventRsvp { EventId = id, UserId = userId, Status = parsed });
 }
 else
 {
 rsvp.Status = parsed;
 rsvp.RespondedUtc = DateTime.UtcNow;
 }
 await _db.SaveChangesAsync();
 return Content($"Thanks! RSVP recorded as {parsed} for event {evt.Title}", "text/plain");
 }
}
