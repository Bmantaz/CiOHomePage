using CiOHomePage.Data;
using CiOHomePage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CiOHomePage.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CalendarController(AppDbContext db) : ControllerBase
{
 private readonly AppDbContext _db = db;

 [HttpGet]
 public async Task<ActionResult<IEnumerable<CalendarEvent>>> Get()
 {
 var uid = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
 var list = await _db.CalendarEvents.AsNoTracking()
 .Where(e => e.IsBandWide || e.CreatedByUserId == uid)
 .OrderBy(e => e.StartUtc)
 .ToListAsync();
 return Ok(list);
 }

 [HttpPost]
 public async Task<ActionResult<CalendarEvent>> Create(CalendarEvent evt)
 {
 var uid = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
 evt.CreatedByUserId = uid;
 _db.CalendarEvents.Add(evt);
 await _db.SaveChangesAsync();
 return CreatedAtAction(nameof(GetById), new { id = evt.Id }, evt);
 }

 [HttpGet("{id}")]
 public async Task<ActionResult<CalendarEvent>> GetById(Guid id)
 {
 var evt = await _db.CalendarEvents.FindAsync(id);
 if (evt is null) return NotFound();
 var uid = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
 if (!evt.IsBandWide && evt.CreatedByUserId != uid) return Forbid();
 return Ok(evt);
 }

 [HttpPut("{id}")]
 public async Task<IActionResult> Update(Guid id, CalendarEvent update)
 {
 if (id != update.Id) return BadRequest();
 var evt = await _db.CalendarEvents.FindAsync(id);
 if (evt is null) return NotFound();
 var uid = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
 if (evt.CreatedByUserId != uid) return Forbid();
 evt.Title = update.Title;
 evt.Type = update.Type;
 evt.StartUtc = update.StartUtc;
 evt.EndUtc = update.EndUtc;
 evt.Location = update.Location;
 evt.Notes = update.Notes;
 evt.IsBandWide = update.IsBandWide;
 await _db.SaveChangesAsync();
 return NoContent();
 }

 [HttpDelete("{id}")]
 public async Task<IActionResult> Delete(Guid id)
 {
 var evt = await _db.CalendarEvents.FindAsync(id);
 if (evt is null) return NotFound();
 var uid = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
 if (evt.CreatedByUserId != uid) return Forbid();
 _db.CalendarEvents.Remove(evt);
 await _db.SaveChangesAsync();
 return NoContent();
 }
}
