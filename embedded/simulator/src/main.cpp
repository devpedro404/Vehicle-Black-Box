#include <iostream>
#include <vector>
#include <thread>
#include <chrono>
#include <iomanip>

#include "../include/TelemetryData.h"
#include "../include/mqtt_client.h"

double kmhToMs(double speedKmh)
{
    return speedKmh / 3.6;
}

double calculateAcceleration(
    double previousSpeed,
    double currentSpeed,
    double deltaTimeSeconds)
{
    double previousMs = kmhToMs(previousSpeed);
    double currentMs = kmhToMs(currentSpeed);

    return (currentMs - previousMs) / deltaTimeSeconds;
}

int main()
{
    std::cout << "Vehicle Black Box Simulator\n\n";

    MqttConfig mqttConfig;
    mqttConfig.host = "localhost";
    mqttConfig.port = 1883;
    mqttConfig.clientId = "vehicle-black-box-simulator";

    MqttClient mqttClient(mqttConfig);
    mqttClient.connect();

    TelemetryData telemetry;

    telemetry.deviceId = "VBB-001";
    telemetry.vehicleId = "CAR-001";

    telemetry.engineTemperature = 89.0;
    telemetry.lateralAcceleration = 0.2;

    telemetry.latitude = -3.119027;
    telemetry.longitude = -60.021731;

    std::string topic = buildTelemetryTopic(telemetry.vehicleId);

    std::vector<double> speeds =
    {
        60, 68, 75, 82, 80, 78, 55, 40, 31
    };

    double previousSpeed = speeds[0];

    for (size_t i = 0; i < speeds.size(); i++)
    {
        telemetry.speed = speeds[i];
        telemetry.timestamp = currentTimestampUtc();

        if (i == 0)
        {
            telemetry.longitudinalAcceleration = 0.0;
        }
        else
        {
            telemetry.longitudinalAcceleration =
                calculateAcceleration(previousSpeed, telemetry.speed, 1.0);
        }

        telemetry.rpm = 900 + static_cast<int>(telemetry.speed * 30);

        std::cout
            << "Speed: " << telemetry.speed << " km/h"
            << " | RPM: " << telemetry.rpm
            << " | Acceleration: " << std::fixed << std::setprecision(2)
            << telemetry.longitudinalAcceleration << " m/s2"
            << "\n";

        if (telemetry.longitudinalAcceleration <= -5.0)
        {
            std::cout << ">>> HARD_BRAKING DETECTED <<<\n";
        }

        if (mqttClient.ensureConnected())
        {
            std::string payload = serializeTelemetryToJson(telemetry);
            bool published = mqttClient.publish(topic, payload, 0, false);

            if (published)
            {
                std::cout << "[MQTT] Publicado em " << topic << "\n";
            }
            else
            {
                std::cout << "[MQTT] Falha ao publicar (sera tentado novamente).\n";
            }
        }
        else
        {
            std::cout << "[MQTT] Sem conexao com o broker. Publicacao pulada.\n";
        }

        previousSpeed = telemetry.speed;

        telemetry.latitude += 0.00001;
        telemetry.longitude += 0.00001;

        std::this_thread::sleep_for(std::chrono::seconds(1));
    }

    return 0;
}