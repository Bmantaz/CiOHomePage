using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CiOHomePage.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
 [HttpGet("auth/google")]
 [AllowAnonymous]
 public IActionResult GoogleAuth()
 {
 // TODO: Implement Google OAuth2.0 authorization code flow
 return Problem("Google Drive OAuth not configured. Add client id/secret and implement flow.", statusCode:501);
 }

 [HttpGet("auth/dropbox")]
 [AllowAnonymous]
 public IActionResult DropboxAuth()
 {
 // TODO: Implement Dropbox OAuth2.0 authorization code flow
 return Problem("Dropbox OAuth not configured. Add app key/secret and implement flow.", statusCode:501);
 }

 public record FileItem(string Id, string Name, string? MimeType, long? Size, DateTime? ModifiedUtc);

 [HttpGet("list")]
 public ActionResult<IEnumerable<FileItem>> List([FromQuery] string provider)
 {
 // TODO: Use provider argument and user tokens to query cloud provider.
 // For scaffolding, return a sample payload so the client UI works end to end.
 var sample = new[]
 {
 new FileItem("1","Sample.txt","text/plain",1234, DateTime.UtcNow.AddDays(-1)),
 new FileItem("2","RehearsalNotes.md","text/markdown",4096, DateTime.UtcNow.AddDays(-3)),
 };
 return Ok(sample);
 }
}
