using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleBlackBox.Api.Controllers;
using VehicleBlackBox.Api.Data;
using VehicleBlackBox.Api.Models;

namespace VehicleBlackBox.Api.Tests.Controllers;

public class VehiclesControllerTests
{
    private static VehicleBlackBoxContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<VehicleBlackBoxContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VehicleBlackBoxContext(options);
    }

    [Fact]
    public async Task GetVehicles_ReturnsVehiclesOrderedById()
    {
        using var context = CreateContext();

        context.Vehicles.AddRange(
            new Vehicle { Id = "CAR-002" },
            new Vehicle { Id = "CAR-001" }
        );

        await context.SaveChangesAsync();

        var controller = new VehiclesController(context);

        var result = await controller.GetVehicles();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var vehicles = Assert.IsAssignableFrom<IEnumerable<Vehicle>>(okResult.Value)
            .ToList();

        Assert.Equal(2, vehicles.Count);
        Assert.Equal("CAR-001", vehicles[0].Id);
        Assert.Equal("CAR-002", vehicles[1].Id);
    }

    [Fact]
    public async Task GetVehicleById_ExistingVehicle_ReturnsVehicle()
    {
        using var context = CreateContext();

        context.Vehicles.Add(new Vehicle { Id = "CAR-001" });
        await context.SaveChangesAsync();

        var controller = new VehiclesController(context);

        var result = await controller.GetVehicleById("CAR-001");

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var vehicle = Assert.IsType<Vehicle>(okResult.Value);

        Assert.Equal("CAR-001", vehicle.Id);
    }

    [Fact]
    public async Task GetVehicleById_NonExistingVehicle_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = new VehiclesController(context);

        var result = await controller.GetVehicleById("CAR-999");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetLatestTelemetry_ReturnsMostRecentTelemetry()
    {
        using var context = CreateContext();

        context.Vehicles.Add(new Vehicle { Id = "CAR-001" });

        context.Telemetries.AddRange(
            new Telemetry
            {
                Id = 1,
                DeviceId = "VBB-001",
                VehicleId = "CAR-001",
                Timestamp = new DateTime(2026, 9, 26, 10, 0, 0, DateTimeKind.Utc),
                Speed = 50
            },
            new Telemetry
            {
                Id = 2,
                DeviceId = "VBB-001",
                VehicleId = "CAR-001",
                Timestamp = new DateTime(2026, 9, 26, 11, 0, 0, DateTimeKind.Utc),
                Speed = 82
            }
        );

        await context.SaveChangesAsync();

        var controller = new VehiclesController(context);

        var result = await controller.GetLatestTelemetry("CAR-001");

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var telemetry = Assert.IsType<Telemetry>(okResult.Value);

        Assert.Equal(2, telemetry.Id);
        Assert.Equal(82, telemetry.Speed);
    }

    [Fact]
    public async Task GetLatestTelemetry_NonExistingVehicle_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = new VehiclesController(context);

        var result = await controller.GetLatestTelemetry("CAR-999");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetLatestTelemetry_VehicleWithoutTelemetry_ReturnsNotFound()
    {
        using var context = CreateContext();

        context.Vehicles.Add(new Vehicle { Id = "CAR-001" });
        await context.SaveChangesAsync();

        var controller = new VehiclesController(context);

        var result = await controller.GetLatestTelemetry("CAR-001");

        Assert.IsType<NotFoundResult>(result.Result);
    }
}
