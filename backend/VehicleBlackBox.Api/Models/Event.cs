namespace VehicleBlackBox.Api.Models;

public class Event
{
    public int Id { get; set; }
    public string VehicleId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public double Speed { get; set; }
    public double Acceleration { get; set; }
}