using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleBlackBox.Api.Controllers;
using VehicleBlackBox.Api.Data;
using VehicleBlackBox.Api.Models;

namespace VehicleBlackBox.Api.Tests.Controllers;

public class TelemetryControllerTests
{
    private static VehicleBlackBoxContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<VehicleBlackBoxContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VehicleBlackBoxContext(options);
    }

    [Fact]
    public async Task CreateTelemetry_ValidTelemetry_CreatesVehicleAndTelemetry()
    {
        using var context = CreateContext();
        var controller = new TelemetryController(context);

        var telemetry = new Telemetry
        {
            DeviceId = "VBB-001",
            VehicleId = "CAR-001",
            Timestamp = DateTime.UtcNow,
            Speed = 82,
            Rpm = 3100,
            EngineTemperature = 89,
            LongitudinalAcceleration = -7.2,
            LateralAcceleration = 1.8,
            Latitude = -3.119,
            Longitude = -60.0217
        };

        var result = await controller.CreateTelemetry(telemetry);

        var createdResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);

        var savedTelemetry = await context.Telemetries.SingleAsync();
        Assert.Equal("VBB-001", savedTelemetry.DeviceId);
        Assert.Equal("CAR-001", savedTelemetry.VehicleId);

        var vehicle = await context.Vehicles.SingleAsync();
        Assert.Equal("CAR-001", vehicle.Id);
    }

    [Fact]
    public async Task CreateTelemetry_ExistingVehicle_DoesNotDuplicateVehicle()
    {
        using var context = CreateContext();
        context.Vehicles.Add(new Vehicle { Id = "CAR-001" });
        await context.SaveChangesAsync();

        var controller = new TelemetryController(context);

        var telemetry = new Telemetry
        {
            DeviceId = "VBB-001",
            VehicleId = "CAR-001",
            Timestamp = DateTime.UtcNow
        };

        var result = await controller.CreateTelemetry(telemetry);

        var createdResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal(1, await context.Vehicles.CountAsync());
        Assert.Equal(1, await context.Telemetries.CountAsync());
    }

    [Fact]
    public async Task CreateTelemetry_MissingVehicleId_ReturnsBadRequest()
    {
        using var context = CreateContext();
        var controller = new TelemetryController(context);

        var telemetry = new Telemetry
        {
            DeviceId = "VBB-001",
            VehicleId = ""
        };

        var result = await controller.CreateTelemetry(telemetry);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Empty(context.Telemetries);
    }

    [Fact]
    public async Task CreateTelemetry_MissingDeviceId_ReturnsBadRequest()
    {
        using var context = CreateContext();
        var controller = new TelemetryController(context);

        var telemetry = new Telemetry
        {
            DeviceId = "",
            VehicleId = "CAR-001"
        };

        var result = await controller.CreateTelemetry(telemetry);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Empty(context.Telemetries);
    }
}
