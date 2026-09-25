# Vehicle Black Box — MQTT

## Objetivo

MQTT será utilizado para comunicação entre o software
embarcado em C++ e o backend em C#.

Arquitetura:

C++
 ↓
MQTT Broker
 ↓
C# Backend

## Broker inicial

Durante o desenvolvimento será utilizado um broker MQTT local.

Posteriormente poderá ser utilizado Docker.

## Topics

### Telemetria

vehicle/{vehicleId}/telemetry

Exemplo:

vehicle/CAR-001/telemetry

### Eventos

vehicle/{vehicleId}/events

Exemplo:

vehicle/CAR-001/events

### Status do dispositivo

vehicle/{vehicleId}/status

Exemplo:

vehicle/CAR-001/status

## Payload de telemetria

```json
{
  "deviceId": "VBB-001",
  "vehicleId": "CAR-001",
  "timestamp": "2026-09-24T18:30:10Z",
  "speed": 82,
  "rpm": 3100,
  "engineTemperature": 89,
  "longitudinalAcceleration": -7.2,
  "lateralAcceleration": 1.8,
  "latitude": -3.1190,
  "longitude": -60.0217
}
