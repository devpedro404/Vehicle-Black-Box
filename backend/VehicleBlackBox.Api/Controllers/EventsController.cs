using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleBlackBox.Api.Data;
using VehicleBlackBox.Api.Models;

namespace VehicleBlackBox.Api.Controllers;

[ApiController]
[Route("api")]
public class EventsController : ControllerBase
{
    private readonly VehicleBlackBoxContext _context;

    public EventsController(VehicleBlackBoxContext context)
    {
        _context = context;
    }

    [HttpGet("vehicles/{vehicleId}/events")]
    public async Task<ActionResult<IEnumerable<Event>>> GetEventsByVehicle(string vehicleId)
    {
        var events = await _context.Events
            .Where(e => e.VehicleId == vehicleId)
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync();

        return Ok(events);
    }

    [HttpGet("events/{id}")]
    public async Task<ActionResult<Event>> GetEventById(int id)
    {
        var ev = await _context.Events.FindAsync(id);

        if (ev is null)
            return NotFound();

        return Ok(ev);
    }

    [HttpPost("events")]
    public async Task<ActionResult<Event>> CreateEvent(Event newEvent)
    {
        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEventById), new { id = newEvent.Id }, newEvent);
    }
}