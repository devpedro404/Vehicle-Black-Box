#include "../include/mqtt_client.h"

#include <mosquitto.h>
#include <cjson/cJSON.h>

#include <iostream>
#include <ctime>
#include <cstdio>

namespace
{
    void onConnect(struct mosquitto* /*mosq*/, void* userdata, int rc)
    {
        bool* connected = static_cast<bool*>(userdata);
        if (rc == 0)
        {
            *connected = true;
            std::cout << "[MQTT] Conectado ao broker.\n";
        }
        else
        {
            *connected = false;
            std::cout << "[MQTT] Falha ao conectar. Codigo: " << rc << "\n";
        }
    }

    void onDisconnect(struct mosquitto* /*mosq*/, void* userdata, int /*rc*/)
    {
        bool* connected = static_cast<bool*>(userdata);
        *connected = false;
        std::cout << "[MQTT] Desconectado do broker.\n";
    }
}

MqttClient::MqttClient(const MqttConfig& config)
    : config_(config), mosq_(nullptr), connected_(false)
{
    mosquitto_lib_init();
    mosq_ = mosquitto_new(config_.clientId.c_str(), true, &connected_);

    if (mosq_ != nullptr)
    {
        mosquitto_connect_callback_set(mosq_, onConnect);
        mosquitto_disconnect_callback_set(mosq_, onDisconnect);
    }
}

MqttClient::~MqttClient()
{
    if (mosq_ != nullptr)
    {
        mosquitto_loop_stop(mosq_, true);
        mosquitto_destroy(mosq_);
    }
    mosquitto_lib_cleanup();
}

bool MqttClient::connect()
{
    if (mosq_ == nullptr)
    {
        return false;
    }

    int rc = mosquitto_connect(mosq_, config_.host.c_str(), config_.port, config_.keepaliveSeconds);
    if (rc != MOSQ_ERR_SUCCESS)
    {
        std::cout << "[MQTT] Erro ao conectar. Codigo: " << rc << "\n";
        connected_ = false;
        return false;
    }

    mosquitto_loop_start(mosq_);
    return true;
}

bool MqttClient::ensureConnected()
{
    if (connected_)
    {
        return true;
    }

    if (mosq_ == nullptr)
    {
        return false;
    }

    std::cout << "[MQTT] Tentando reconectar...\n";
    int rc = mosquitto_reconnect(mosq_);

    return rc == MOSQ_ERR_SUCCESS;
}

bool MqttClient::publish(const std::string& topic, const std::string& payload, int qos, bool retain)
{
    if (mosq_ == nullptr || !connected_)
    {
        return false;
    }

    int rc = mosquitto_publish(
        mosq_,
        nullptr,
        topic.c_str(),
        static_cast<int>(payload.size()),
        payload.c_str(),
        qos,
        retain
    );

    return rc == MOSQ_ERR_SUCCESS;
}

bool MqttClient::isConnected() const
{
    return connected_;
}


std::string buildTelemetryTopic(const std::string& vehicleId)
{
    return "vehicle/" + vehicleId + "/telemetry";
}

std::string currentTimestampUtc()
{
    std::time_t now = std::time(nullptr);
    std::tm utcTime{};

#if defined(_WIN32)
    gmtime_s(&utcTime, &now);
#else
    gmtime_r(&now, &utcTime);
#endif

    char buffer[32];
    std::strftime(buffer, sizeof(buffer), "%Y-%m-%dT%H:%M:%SZ", &utcTime);

    return std::string(buffer);
}

std::string serializeTelemetryToJson(const TelemetryData& telemetry)
{
    cJSON* root = cJSON_CreateObject();

    cJSON_AddStringToObject(root, "deviceId", telemetry.deviceId.c_str());
    cJSON_AddStringToObject(root, "vehicleId", telemetry.vehicleId.c_str());
    cJSON_AddStringToObject(root, "timestamp", telemetry.timestamp.c_str());
    cJSON_AddNumberToObject(root, "speed", telemetry.speed);
    cJSON_AddNumberToObject(root, "rpm", telemetry.rpm);
    cJSON_AddNumberToObject(root, "engineTemperature", telemetry.engineTemperature);
    cJSON_AddNumberToObject(root, "longitudinalAcceleration", telemetry.longitudinalAcceleration);
    cJSON_AddNumberToObject(root, "lateralAcceleration", telemetry.lateralAcceleration);
    cJSON_AddNumberToObject(root, "latitude", telemetry.latitude);
    cJSON_AddNumberToObject(root, "longitude", telemetry.longitude);

    char* rendered = cJSON_PrintUnformatted(root);
    std::string result(rendered);

    cJSON_free(rendered);
    cJSON_Delete(root);

    return result;
}