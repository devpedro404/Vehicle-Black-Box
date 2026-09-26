using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleBlackBox.Api.Data;
using VehicleBlackBox.Api.Models;

namespace VehicleBlackBox.Api.Controllers;

[ApiController]
[Route("api/vehicles")]
public class VehiclesController : ControllerBase
{
    private readonly VehicleBlackBoxContext _context;

    public VehiclesController(VehicleBlackBoxContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehicles()
    {
        var vehicles = await _context.Vehicles
            .OrderBy(v => v.Id)
            .ToListAsync();

        return Ok(vehicles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Vehicle>> GetVehicleById(string id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);

        if (vehicle is null)
            return NotFound();

        return Ok(vehicle);
    }

    [HttpGet("{id}/telemetry/latest")]
    public async Task<ActionResult<Telemetry>> GetLatestTelemetry(string id)
    {
        var vehicleExists = await _context.Vehicles
            .AnyAsync(v => v.Id == id);

        if (!vehicleExists)
            return NotFound();

        var telemetry = await _context.Telemetries
            .Where(t => t.VehicleId == id)
            .OrderByDescending(t => t.Timestamp)
            .ThenByDescending(t => t.Id)
            .FirstOrDefaultAsync();

        if (telemetry is null)
            return NotFound();

        return Ok(telemetry);
    }
}
