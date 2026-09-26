# Vehicle Black Box — MQTT

## Status

Planejado para Sprint 2.

A arquitetura e a divisão de tarefas da Sprint 2 ainda serão fechadas antes da implementação.

## Objetivo

Usar MQTT como camada de comunicação entre o simulador/embedded em C++ e o backend em C#.

Direção planejada:

```text
C++ Simulator
      ↓
MQTT Broker
      ↓
ASP.NET Core
      ↓
SQLite
      ↓
React
```

## Broker de desenvolvimento

A intenção atual é começar com um broker MQTT local.

Docker poderá ser considerado depois, sem tornar o broker parte da regra de negócio.

## Topics planejados

### Telemetria

```text
vehicle/{vehicleId}/telemetry
```

Exemplo:

```text
vehicle/CAR-001/telemetry
```

### Eventos

```text
vehicle/{vehicleId}/events
```

Exemplo:

```text
vehicle/CAR-001/events
```

### Status do dispositivo

Reservado para evolução futura:

```text
vehicle/{vehicleId}/status
```

Exemplo:

```text
vehicle/CAR-001/status
```

## Payload atual de telemetria

O payload MQTT de telemetria deve preservar o contrato já utilizado pela API:

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
```

## Regras arquiteturais

- MQTT é transporte, não banco;
- MQTT não deve conter regra de persistência;
- o backend continua responsável por validação e persistência;
- o frontend não consome MQTT diretamente neste estágio;
- a implementação final deve ser documentada em `docs/sprints/sprint-02.md`.