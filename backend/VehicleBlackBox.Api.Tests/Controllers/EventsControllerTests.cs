using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleBlackBox.Api.Controllers;
using VehicleBlackBox.Api.Data;
using VehicleBlackBox.Api.Models;
using Xunit;

namespace VehicleBlackBox.Api.Tests.Controllers;

public class EventsControllerTests
{
    private static VehicleBlackBoxContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<VehicleBlackBoxContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VehicleBlackBoxContext(options);
    }

    [Fact]
    public async Task GetEventsByVehicle_ReturnsOnlyEventsForThatVehicle()
    {
        using var context = CreateContext();
        context.Events.AddRange(
            new Event { Id = 1, VehicleId = "CAR-001", EventType = "HARD_BRAKING", Timestamp = DateTime.UtcNow, Speed = 82, Acceleration = -7.2 },
            new Event { Id = 2, VehicleId = "CAR-002", EventType = "HARD_BRAKING", Timestamp = DateTime.UtcNow, Speed = 60, Acceleration = -5.0 }
        );
        await context.SaveChangesAsync();

        var controller = new EventsController(context);
        var result = await controller.GetEventsByVehicle("CAR-001");

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var events = Assert.IsAssignableFrom<IEnumerable<Event>>(okResult.Value);
        Assert.Single(events);
        Assert.Equal("CAR-001", events.First().VehicleId);
    }

    [Fact]
    public async Task GetEventById_ExistingId_ReturnsEvent()
    {
        using var context = CreateContext();
        context.Events.Add(new Event
        {
            Id = 1,
            VehicleId = "CAR-001",
            EventType = "HARD_BRAKING",
            Timestamp = DateTime.UtcNow,
            Speed = 82,
            Acceleration = -7.2
        });
        await context.SaveChangesAsync();

        var controller = new EventsController(context);
        var result = await controller.GetEventById(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var ev = Assert.IsType<Event>(okResult.Value);
        Assert.Equal("HARD_BRAKING", ev.EventType);
    }

    [Fact]
    public async Task GetEventById_NonExistingId_ReturnsNotFound()
    {
        using var context = CreateContext();
        var controller = new EventsController(context);

        var result = await controller.GetEventById(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }
}