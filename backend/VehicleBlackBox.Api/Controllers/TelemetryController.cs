using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleBlackBox.Api.Data;
using VehicleBlackBox.Api.Models;

namespace VehicleBlackBox.Api.Controllers;

[ApiController]
[Route("api")]
public class TelemetryController : ControllerBase
{
    private readonly VehicleBlackBoxContext _context;

    public TelemetryController(VehicleBlackBoxContext context)
    {
        _context = context;
    }

    [HttpPost("telemetry")]
    public async Task<ActionResult<Telemetry>> CreateTelemetry(Telemetry telemetry)
    {
        if (string.IsNullOrWhiteSpace(telemetry.VehicleId))
            return BadRequest("vehicleId is required.");

        if (string.IsNullOrWhiteSpace(telemetry.DeviceId))
            return BadRequest("deviceId is required.");

        var vehicleExists = await _context.Vehicles
            .AnyAsync(v => v.Id == telemetry.VehicleId);

        if (!vehicleExists)
        {
            _context.Vehicles.Add(new Vehicle
            {
                Id = telemetry.VehicleId
            });
        }

        _context.Telemetries.Add(telemetry);
        await _context.SaveChangesAsync();

        return StatusCode(201, telemetry);
    }
}
