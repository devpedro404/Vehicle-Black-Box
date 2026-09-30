using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using VehicleBlackBox.Api.Data;
using VehicleBlackBox.Api.Models;

namespace VehicleBlackBox.Api.Services;

public class MqttTelemetryService : BackgroundService
{
    private readonly ILogger<MqttTelemetryService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private IMqttClient? _mqttClient;

    public MqttTelemetryService(
        ILogger<MqttTelemetryService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new MqttClientFactory();
        _mqttClient = factory.CreateMqttClient();

        _mqttClient.ConnectedAsync += async e =>
        {
            _logger.LogInformation("MQTT conectado ao broker.");

            var subscribeOptions = factory
                .CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => f.WithTopic("vehicle/+/telemetry"))
                .Build();

            await _mqttClient.SubscribeAsync(
                subscribeOptions,
                stoppingToken);

            _logger.LogInformation(
                "MQTT inscrito no tópico vehicle/+/telemetry");
        };

        _mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            try
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = e.ApplicationMessage.ConvertPayloadToString();

                var telemetry = JsonSerializer.Deserialize<Telemetry>(
                    payload,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (telemetry is null)
                {
                    _logger.LogWarning("Payload MQTT vazio ou inválido.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(telemetry.VehicleId) ||
                    string.IsNullOrWhiteSpace(telemetry.DeviceId))
                {
                    _logger.LogWarning(
                        "Telemetry inválida: vehicleId ou deviceId ausente.");
                    return;
                }

                var topicParts = topic.Split('/');

                if (topicParts.Length != 3 ||
                    topicParts[0] != "vehicle" ||
                    topicParts[2] != "telemetry")
                {
                    _logger.LogWarning(
                        "Topic MQTT inválido: {Topic}",
                        topic);
                    return;
                }

                var vehicleIdFromTopic = topicParts[1];

                if (!string.Equals(
                    vehicleIdFromTopic,
                    telemetry.VehicleId,
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning(
                        "VehicleId do tópico difere do payload.");
                    return;
                }

                await using var scope = _scopeFactory.CreateAsyncScope();

                var db = scope.ServiceProvider
                    .GetRequiredService<VehicleBlackBoxContext>();

                var vehicleExists = await db.Vehicles
                    .AnyAsync(
                        v => v.Id == telemetry.VehicleId,
                        stoppingToken);

                if (!vehicleExists)
                {
                    db.Vehicles.Add(new Vehicle
                    {
                        Id = telemetry.VehicleId
                    });
                }

                db.Telemetries.Add(telemetry);

                await db.SaveChangesAsync(stoppingToken);

                _logger.LogInformation(
                    "Telemetry persistida via MQTT para {VehicleId}.",
                    telemetry.VehicleId);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(
                    ex,
                    "JSON MQTT inválido. Mensagem descartada.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao processar mensagem MQTT.");
            }
        };

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1883)
            .WithClientId("vehicle-black-box-backend")
            .Build();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!_mqttClient.IsConnected)
                {
                    _logger.LogInformation(
                        "Tentando conectar ao broker MQTT...");

                    await _mqttClient.ConnectAsync(
                        options,
                        stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Não foi possível conectar ao broker MQTT.");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(5),
                stoppingToken);
        }
    }
}
