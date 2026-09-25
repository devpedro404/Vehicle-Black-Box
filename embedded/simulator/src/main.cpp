#include <iostream>
#include "../include/TelemetryData.h"

int main()
{
    TelemetryData telemetry;

    telemetry.deviceId = "VBB-001";
    telemetry.vehicleId = "CAR-001";

    telemetry.speed = 82.0;
    telemetry.rpm = 3100;
    telemetry.engineTemperature = 89.0;

    telemetry.longitudinalAcceleration = -0.5;
    telemetry.lateralAcceleration = 0.2;

    telemetry.latitude = -3.119027;
    telemetry.longitude = -60.021731;

    std::cout << "Vehicle Black Box Simulator\n\n";

    std::cout << "Vehicle: "
              << telemetry.vehicleId
              << "\n";

    std::cout << "Speed: "
              << telemetry.speed
              << " km/h\n";

    std::cout << "RPM: "
              << telemetry.rpm
              << "\n";

    std::cout << "Engine temperature: "
              << telemetry.engineTemperature
              << " C\n";

    std::cout << "Acceleration: "
              << telemetry.longitudinalAcceleration
              << " m/s2\n";

    std::cout << "GPS: "
              << telemetry.latitude
              << ", "
              << telemetry.longitude
              << "\n";

    return 0;
}
