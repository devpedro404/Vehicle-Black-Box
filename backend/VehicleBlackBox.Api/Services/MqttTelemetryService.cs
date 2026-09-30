using MQTTnet;

namespace VehicleBlackBox.Api.Services;

public class MqttTelemetryService : BackgroundService
{
    private readonly ILogger<MqttTelemetryService> _logger;
    private IMqttClient? _mqttClient;

    public MqttTelemetryService(ILogger<MqttTelemetryService> logger)
    {
        _logger = logger;
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

        _mqttClient.DisconnectedAsync += async e =>
        {
            _logger.LogWarning("MQTT desconectado do broker.");

            if (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(5),
                    stoppingToken);
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
