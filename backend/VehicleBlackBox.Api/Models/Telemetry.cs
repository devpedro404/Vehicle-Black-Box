namespace VehicleBlackBox.Api.Models;

public class Telemetry
{
    public int Id { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string VehicleId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }

    public double Speed { get; set; }
    public int Rpm { get; set; }
    public double EngineTemperature { get; set; }

    public double LongitudinalAcceleration { get; set; }
    public double LateralAcceleration { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
