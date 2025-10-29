using CiOHomePage.Server.Data;
using CiOHomePage.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CiOHomePage.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MerchController(AppDbContext db) : ControllerBase
{
 private readonly AppDbContext _db = db;

 [HttpGet]
 public async Task<ActionResult<IEnumerable<MerchSale>>> GetAll()
 {
 var items = await _db.MerchSales.AsNoTracking().OrderByDescending(x => x.DateSold).ToListAsync();
 return Ok(items);
 }

 [HttpGet("summary")]
 public async Task<ActionResult<object>> Summary()
 {
 var items = await _db.MerchSales.AsNoTracking().ToListAsync();
 var revenue = items.Sum(x => x.Quantity * x.SalePrice);
 var top = items
 .GroupBy(x => x.ItemName)
 .Select(g => new { Item = g.Key, Qty = g.Sum(x => x.Quantity) })
 .OrderByDescending(x => x.Qty)
 .Take(5)
 .ToList();
 return Ok(new { revenue, top });
 }

 [HttpPost]
 public async Task<ActionResult<MerchSale>> Create(MerchSale sale)
 {
 _db.MerchSales.Add(sale);
 await _db.SaveChangesAsync();
 return CreatedAtAction(nameof(GetById), new { id = sale.Id }, sale);
 }

 [HttpGet("{id}")]
 public async Task<ActionResult<MerchSale>> GetById(int id)
 {
 var sale = await _db.MerchSales.FindAsync(id);
 return sale is null ? NotFound() : Ok(sale);
 }

 [HttpPut("{id}")]
 public async Task<IActionResult> Update(int id, MerchSale sale)
 {
 if (id != sale.Id) return BadRequest();
 var existing = await _db.MerchSales.FindAsync(id);
 if (existing is null) return NotFound();
 existing.ItemName = sale.ItemName;
 existing.Category = sale.Category;
 existing.Quantity = sale.Quantity;
 existing.SalePrice = sale.SalePrice;
 existing.DateSold = sale.DateSold;
 await _db.SaveChangesAsync();
 return NoContent();
 }

 [HttpDelete("{id}")]
 public async Task<IActionResult> Delete(int id)
 {
 var existing = await _db.MerchSales.FindAsync(id);
 if (existing is null) return NotFound();
 _db.MerchSales.Remove(existing);
 await _db.SaveChangesAsync();
 return NoContent();
 }
}
