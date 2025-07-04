using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ScaffoldApp.DbConfigurations;
using ScaffoldApp.Models;

namespace ScaffoldApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private static readonly ActivitySource MySource = new ActivitySource("scaffoldApp.Api.Manual");
    private readonly AppDbContext _context;
    private readonly ILogger<LogsController> _log;
    public LogsController(AppDbContext context, ILogger<LogsController> log)
    {
        _context = context;
        _log = log;
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
    
    [HttpGet("trace")]
    public IActionResult Trace()
    {
        using var activity = MySource.StartActivity("ManualOperation");
        _log.LogInformation("Iniciando operação manual…");

        // aqui seu código “pesado”
        Thread.Sleep(200);

        _log.LogInformation("Operação manual concluída.");
        return Ok("traced");
    }
}