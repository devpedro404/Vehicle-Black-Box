#ifndef MQTT_CLIENT_H
#define MQTT_CLIENT_H

#include <string>
#include "TelemetryData.h"

struct MqttConfig
{
    std::string host = "localhost";
    int port = 1883;
    std::string clientId = "vehicle-black-box-simulator";
    int keepaliveSeconds = 60;
};

class MqttClient
{
public:
    explicit MqttClient(const MqttConfig& config);
    ~MqttClient();

    MqttClient(const MqttClient&) = delete;
    MqttClient& operator=(const MqttClient&) = delete;

    bool connect();
    bool ensureConnected();
    bool publish(const std::string& topic, const std::string& payload, int qos = 0, bool retain = false);
    bool isConnected() const;

private:
    MqttConfig config_;
    struct mosquitto* mosq_;
    bool connected_;
};

// P2.2 - helpers de contrato/serializacao
std::string buildTelemetryTopic(const std::string& vehicleId);
std::string currentTimestampUtc();
std::string serializeTelemetryToJson(const TelemetryData& telemetry);

#endif