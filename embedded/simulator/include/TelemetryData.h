#ifndef TELEMETRY_DATA_H
#define TELEMETRY_DATA_H

#include <string>

struct TelemetryData
{
    std::string deviceId;
    std::string vehicleId;
    std::string timestamp;

    double speed;
    int rpm;
    double engineTemperature;

    double longitudinalAcceleration;
    double lateralAcceleration;

    double latitude;
    double longitude;
};

#endif
