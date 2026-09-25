using Microsoft.EntityFrameworkCore;
using VehicleBlackBox.Api.Models;

namespace VehicleBlackBox.Api.Data;

public class VehicleBlackBoxContext : DbContext
{
    public VehicleBlackBoxContext(DbContextOptions<VehicleBlackBoxContext> options)
        : base(options) {   }

        public DbSet<Event> Events => Set<Event>();

        // Continues with DbSet Vehicle and DbSet Telemetry
}