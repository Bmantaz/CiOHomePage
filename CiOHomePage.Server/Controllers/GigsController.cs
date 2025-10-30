using CiOHomePage.Server.Data;
using CiOHomePage.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CiOHomePage.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GigsController(AppDbContext db) : ControllerBase
{
 private readonly AppDbContext _db = db;

 [HttpGet]
 public async Task<ActionResult<IEnumerable<Gig>>> Get()
 {
 var today = DateTime.UtcNow.Date;
 var gigs = await _db.Gigs
 .Where(g => g.Date.Date >= today)
 .OrderBy(g => g.Date)
 .ToListAsync();
 return Ok(gigs);
 }

 [Authorize(Roles = "BandMember")]
 [HttpPost]
 public async Task<ActionResult<Gig>> Create([FromBody] Gig gig)
 {
 if (!ModelState.IsValid) return ValidationProblem(ModelState);
 _db.Gigs.Add(gig);
 await _db.SaveChangesAsync();
 return CreatedAtAction(nameof(Get), new { id = gig.Id }, gig);
 }
}
