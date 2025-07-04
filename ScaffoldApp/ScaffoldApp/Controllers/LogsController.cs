using Microsoft.AspNetCore.Mvc;
using ScaffoldApp.DbConfigurations;
using ScaffoldApp.Models;

namespace ScaffoldApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public LogsController(AppDbContext context)
    {
        _context = context;
    }
    [HttpPost]
    public async Task<ActionResult<LogEntry>> PostLog([FromBody] LogEntry logEntry)
    {
        _context.LogEntries.Add(logEntry);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLog), new { id = logEntry.Id }, logEntry);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LogEntry>> GetLog(int id)
    {
        var log = await _context.LogEntries.FindAsync(id);
        if (log == null) return NotFound();
        return Ok(log);
    }
    [HttpGet("ping")]
    public IActionResult Ping([FromServices] ILogger<LogsController> log)
    {
        log.LogInformation("Teste de log em /ping");
        return Ok("pong");
    }
}