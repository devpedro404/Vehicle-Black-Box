namespace VehicleBlackBox.Api.Models;

public class MqttOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 1883;
    public string ClientId { get; set; } = "vehicle-black-box-backend";
    public string Topic { get; set; } = "vehicle/+/telemetry";
}
