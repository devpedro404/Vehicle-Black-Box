# Vehicle Black Box — Backlog Mestre

> Fonte de verdade do estado funcional do projeto.
> Atualizado em: 2026-09-26.
> Estado: Sprint 1 concluída; cleanup e planejamento pré-Sprint 2.

## 1. Objetivo do projeto

Construir uma caixa-preta veicular capaz de coletar, processar, armazenar e visualizar dados de condução, começando por dados simulados e evoluindo depois para hardware real.

## 2. Arquitetura do produto

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

Princípios:

- o Core deve permanecer independente do modelo do veículo;
- diferenças físicas ficam no adaptador/chicote;
- diferenças de comunicação ficam no módulo de interface;
- o frontend não contém regras de detecção de eventos;
- o backend valida, organiza e persiste;
- SQLite é o banco atual;
- PostgreSQL é uma evolução futura.

## 3. Stack atual

- C++17
- CMake
- C# / ASP.NET Core (.NET 10)
- Entity Framework Core
- SQLite
- React
- Vite
- xUnit
- GitHub Actions

## 4. Sprint 0 — Fundação

Status: CONCLUÍDA.

- [x] Repositório criado
- [x] Simulador C++
- [x] CMake
- [x] Simulação dinâmica
- [x] Detecção inicial `HARD_BRAKING`
- [x] Backend ASP.NET Core
- [x] Frontend React + Vite
- [x] Entity Framework Core
- [x] SQLite
- [x] Builds locais funcionando

## 5. Sprint 1A — Pedro / Events

Objetivo:

```text
Event → SQLite → ASP.NET Core → React
```

Concluído:

- [x] Entidade `Event`
- [x] Persistência em SQLite
- [x] Migration de Event
- [x] `POST /api/events`
- [x] `GET /api/vehicles/{vehicleId}/events`
- [x] `GET /api/events/{id}`
- [x] Histórico de eventos no React
- [x] Detalhe de evento no React
- [x] Gráfico inicial
- [x] Frontend consumindo API real
- [x] Testes automatizados do `EventsController`
- [x] Merge na `main`

PR principal: #3 — `Feature/events flow`.

## 6. Sprint 1B — Gisa / Telemetry

Objetivo:

```text
React Dashboard → ASP.NET Core → Telemetry → SQLite
```

Concluído:

- [x] Entidade `Vehicle`
- [x] Entidade `Telemetry`
- [x] `VehicleBlackBoxContext` compartilhado
- [x] Migration `AddVehicleAndTelemetry`
- [x] SQLite atualizado
- [x] `POST /api/telemetry`
- [x] `GET /api/vehicles`
- [x] `GET /api/vehicles/{id}`
- [x] `GET /api/vehicles/{id}/telemetry/latest`
- [x] Tratamento de 404
- [x] Dashboard React
- [x] Velocidade
- [x] RPM
- [x] Temperatura do motor
- [x] Aceleração longitudinal
- [x] Aceleração lateral
- [x] GPS
- [x] Service de telemetria
- [x] Frontend consumindo API real
- [x] Teste ponta a ponta
- [x] Testes automatizados de Telemetry e Vehicles
- [x] Merge na `main`

PR: #4 — `feat: conclui fluxo de telemetria da Sprint 1`.

## 7. Quality gates

- [x] Teste placeholder removido
- [x] 13 testes backend passando
- [x] Frontend lint: 0 warnings / 0 errors
- [x] Frontend build: sucesso
- [x] GitHub Actions configurado
- [x] Job `Backend Tests`
- [x] Job `Frontend Quality`
- [x] CI verde no PR e após merge na `main`

PR: #5 — `ci: adiciona quality gates automatizados`.

## 8. Contrato atual de Telemetry

Campos atuais:

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

Exemplo:

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

`connectionStatus` e `softwareVersion` ficam reservados para evolução futura de Device.

## 9. Dívida técnica conhecida

Não bloqueia o encerramento da Sprint 1:

- [ ] definir estratégia de DTOs da API
- [ ] fortalecer validação de Events
- [ ] fortalecer validação de Telemetry
- [ ] revisar relacionamento/FK Vehicle → Telemetry
- [ ] revisar criação automática de Vehicle no POST de Telemetry
- [ ] preservar explicitamente a semântica UTC do timestamp
- [ ] mover URL da API do frontend para configuração de ambiente
- [ ] tornar origem de CORS configurável
- [ ] otimizar bundle React que gera aviso acima de 500 kB
- [ ] adicionar build/testes do simulador C++ ao CI
- [ ] proteger a branch `main` com PR/checks obrigatórios

## 10. Evolução de Events

Planejado para etapas futuras:

- [ ] buffer circular de telemetria
- [ ] janela pré-evento
- [ ] janela pós-evento
- [ ] congelar dados ao detectar evento
- [ ] relacionar evento a amostras de telemetria
- [ ] gráfico temporal real

Eventos documentados:

- `HARD_BRAKING`
- `HARD_ACCELERATION`
- `SPEEDING`
- `HARD_CORNERING`
- `HIGH_ENGINE_TEMPERATURE`
- `POSSIBLE_IMPACT`

Apenas a detecção inicial de `HARD_BRAKING` está implementada no simulador neste checkpoint.

## 11. Sprint 2

Status: PLANEJAMENTO — NÃO CONGELADO.

Direção histórica já documentada:

```text
C++ Simulator → MQTT → ASP.NET Core → SQLite → React
```

Antes de iniciar implementação:

- revisar ideias e benchmarks externos;
- confirmar a arquitetura de comunicação;
- definir Definition of Done;
- equilibrar a carga entre Gisa e Pedro;
- criar branches novas a partir da `main` atualizada;
- registrar o plano definitivo em `docs/sprints/sprint-02.md`.

## 12. Fora do escopo atual

- hardware real
- Raspberry Pi ou equivalente
- CAN
- OBD-II
- GPS físico
- acelerômetro/giroscópio físico
- PostgreSQL
- streaming realtime para o frontend

## 13. Continuidade

Ao retomar o projeto, ler nesta ordem:

1. `README.md`
2. `docs/continuity.md`
3. `docs/backlog.md`
4. `docs/architecture.md`
5. `docs/telemetry.md`
6. `docs/events.md`
7. `docs/mqtt.md`
8. documento da sprint atual

Sempre conferir a `main`, PRs recentes e CI antes de alterar código.