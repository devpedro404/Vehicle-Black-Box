using System.Text.Json;
using VehicleBlackBox.Api.Models;

namespace VehicleBlackBox.Api.Services;

public static class TelemetryMqttValidator
{
    public static bool TryParse(
        string topic,
        string payload,
        out Telemetry? telemetry,
        out string error)
    {
        telemetry = null;
        error = string.Empty;

        try
        {
            telemetry = JsonSerializer.Deserialize<Telemetry>(
                payload,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch (JsonException)
        {
            error = "JSON MQTT inválido.";
            return false;
        }

        if (telemetry is null)
        {
            error = "Payload MQTT vazio ou inválido.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(telemetry.VehicleId) ||
            string.IsNullOrWhiteSpace(telemetry.DeviceId))
        {
            error = "Telemetry inválida: vehicleId ou deviceId ausente.";
            return false;
        }

        var topicParts = topic.Split('/');

        if (topicParts.Length != 3 ||
            topicParts[0] != "vehicle" ||
            topicParts[2] != "telemetry")
        {
            error = $"Topic MQTT inválido: {topic}";
            return false;
        }

        var vehicleIdFromTopic = topicParts[1];

        if (!string.Equals(
            vehicleIdFromTopic,
            telemetry.VehicleId,
            StringComparison.OrdinalIgnoreCase))
        {
            error = "VehicleId do tópico difere do payload.";
            return false;
        }

        return true;
    }
}
