using VehicleBlackBox.Api.Services;

namespace VehicleBlackBox.Api.Tests.Services;

public class TelemetryMqttValidatorTests
{
    [Fact]
    public void PayloadValido_DeveSerAceito()
    {
        var payload = """
        {
          "deviceId": "VBB-001",
          "vehicleId": "CAR-001",
          "timestamp": "2026-09-30T04:00:00Z",
          "speed": 80,
          "rpm": 3000
        }
        """;

        var result = TelemetryMqttValidator.TryParse(
            "vehicle/CAR-001/telemetry",
            payload,
            out var telemetry,
            out var error);

        Assert.True(result);
        Assert.NotNull(telemetry);
        Assert.Equal("CAR-001", telemetry.VehicleId);
        Assert.Equal("VBB-001", telemetry.DeviceId);
        Assert.Equal(string.Empty, error);
    }

    [Fact]
    public void JsonInvalido_DeveSerRejeitado()
    {
        var result = TelemetryMqttValidator.TryParse(
            "vehicle/CAR-001/telemetry",
            "{json-invalido",
            out var telemetry,
            out var error);

        Assert.False(result);
        Assert.Null(telemetry);
        Assert.Equal("JSON MQTT inválido.", error);
    }

    [Fact]
    public void DeviceIdAusente_DeveSerRejeitado()
    {
        var payload = """
        {
          "vehicleId": "CAR-001",
          "speed": 80
        }
        """;

        var result = TelemetryMqttValidator.TryParse(
            "vehicle/CAR-001/telemetry",
            payload,
            out _,
            out var error);

        Assert.False(result);
        Assert.Contains("deviceId ausente", error);
    }

    [Fact]
    public void VehicleIdDivergente_DoTopico_DeveSerRejeitado()
    {
        var payload = """
        {
          "deviceId": "VBB-001",
          "vehicleId": "CAR-001"
        }
        """;

        var result = TelemetryMqttValidator.TryParse(
            "vehicle/CAR-002/telemetry",
            payload,
            out _,
            out var error);

        Assert.False(result);
        Assert.Equal(
            "VehicleId do tópico difere do payload.",
            error);
    }
}
