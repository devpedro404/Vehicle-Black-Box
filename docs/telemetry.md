# Vehicle Black Box — Telemetry

Este documento define o contrato atual de telemetria usado pelo projeto.

## Campos atuais

| Campo | Tipo | Unidade | Exemplo |
|---|---|---|---|
| deviceId | string | - | VBB-001 |
| vehicleId | string | - | CAR-001 |
| timestamp | datetime | UTC | 2026-09-24T18:30:10Z |
| speed | double | km/h | 82 |
| rpm | int | RPM | 3100 |
| engineTemperature | double | °C | 89 |
| longitudinalAcceleration | double | m/s² | -7.2 |
| lateralAcceleration | double | m/s² | 1.8 |
| latitude | double | graus | -3.1190 |
| longitude | double | graus | -60.0217 |

## Modelo atual no backend

`Telemetry` contém:

- `Id`
- `DeviceId`
- `VehicleId`
- `Timestamp`
- `Speed`
- `Rpm`
- `EngineTemperature`
- `LongitudinalAcceleration`
- `LateralAcceleration`
- `Latitude`
- `Longitude`

## Exemplo JSON

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

## Dados de Device para evolução futura

Os campos abaixo já foram citados como visão de Device, mas não fazem parte do modelo `Telemetry` atual:

| Campo | Descrição |
|---|---|
| connectionStatus | Estado da conexão |
| softwareVersion | Versão do software embarcado |

Decisão atual:

- `DeviceId` permanece em `Telemetry`;
- `connectionStatus` e `softwareVersion` ficam para futura entidade/módulo de Device.

## Observação de timestamp

O contrato usa UTC e os exemplos usam o sufixo `Z`.

A persistência/leitura de UTC no SQLite está registrada como dívida técnica para revisão futura.