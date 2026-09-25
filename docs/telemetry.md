# Vehicle Black Box — Telemetry

Este documento define os dados que serão coletados pelo sistema.

## Telemetria inicial

| Campo | Tipo | Unidade | Exemplo |
|---|---|---|---|
| vehicleId | string | - | CAR-001 |
| timestamp | datetime | UTC | 2026-09-24T18:30:10Z |
| speed | double | km/h | 82 |
| rpm | int | RPM | 3100 |
| engineTemperature | double | °C | 89 |
| longitudinalAcceleration | double | m/s² | -7.2 |
| lateralAcceleration | double | m/s² | 1.8 |
| latitude | double | graus | -3.1190 |
| longitude | double | graus | -60.0217 |

## Dados do dispositivo

| Campo | Descrição |
|---|---|
| deviceId | Identificador do dispositivo |
| vehicleId | Identificador do veículo |
| timestamp | Data e hora da leitura |
| connectionStatus | Estado da conexão |
| softwareVersion | Versão do software embarcado |

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
