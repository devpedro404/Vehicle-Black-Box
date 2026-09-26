# Vehicle Black Box — Architecture

## Objetivo

Construir um sistema de caixa-preta veicular capaz de coletar, processar, armazenar e visualizar dados de condução sem acoplar o Core a um modelo específico de veículo.

## Arquitetura modular do produto

```text
CARRO
  ↓
ADAPTADOR DO VEÍCULO
  ↓
MÓDULO DE INTERFACE
  ↓
CORE VEHICLE BLACK BOX
  ↓
COMUNICAÇÃO
  ↓
BACKEND C#
  ↓
BANCO
  ↓
FRONTEND REACT
```

## Responsabilidades

### Adaptador do veículo

Responsável por diferenças físicas:

- conector;
- pinagem;
- alimentação;
- chicote;
- acesso OBD/CAN quando aplicável.

### Módulo de interface

Responsável por conversar com a eletrônica do veículo:

- CAN;
- OBD-II;
- outros barramentos/protocolos futuros.

### Core Vehicle Black Box

Deve permanecer independente do veículo.

Responsabilidades:

- aquisição normalizada de dados;
- timestamp;
- telemetria;
- buffer;
- regras de eventos;
- armazenamento local futuro;
- comunicação.

### Backend

Responsabilidades:

- receber dados;
- validar;
- organizar;
- persistir;
- expor APIs para o frontend.

### Frontend

Responsabilidades:

- visualizar telemetria;
- visualizar veículos;
- visualizar histórico e detalhes de eventos.

O frontend não deve conter regras de detecção de eventos.

## Arquitetura atual

```text
C++ Simulator
      ↓
ASP.NET Core
      ↓
SQLite
      ↓
React
```

A Sprint 1 validou os fluxos de Events e Telemetry sem MQTT.

## Comunicação planejada

Direção histórica para a próxima etapa:

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

A implementação definitiva será fechada no planejamento da Sprint 2.

## Arquitetura futura de hardware

```text
Sensores / CAN / OBD
        ↓
Adaptador + Interface
        ↓
Core embarcado em C++
        ↓
Comunicação
        ↓
Backend C#
        ↓
Banco
        ↓
React
```

Possíveis componentes futuros:

- Raspberry Pi ou equivalente;
- interface CAN;
- GPS;
- acelerômetro/giroscópio;
- armazenamento local.

## Banco

Atual:

- SQLite.

Futuro possível:

- PostgreSQL em ambiente servidor.

Não migrar de banco sem decisão explícita de arquitetura.