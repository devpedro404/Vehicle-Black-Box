# Vehicle Black Box — Contexto de Continuidade

> Checkpoint: 2026-09-30
> Objetivo: permitir a retomada segura do projeto por Endy, Pedro ou outra IA.
> Fonte de verdade: `main` atual + documentação versionada + contratos das Sprints.

## Estado atual

Sprint 1 concluída.

Sprint 2 concluída.

Sprint 3 planejada e pronta para execução.

A branch `main` contém atualmente:

- simulador C++;
- detecção inicial de `HARD_BRAKING`;
- serialização JSON de Telemetry;
- integração MQTT no simulador;
- publicação MQTT de Telemetry;
- reconexão mínima no simulador;
- backend ASP.NET Core;
- Entity Framework Core;
- SQLite;
- fluxo completo de Events;
- fluxo completo de Telemetry;
- cliente MQTT no backend;
- `MqttTelemetryService`;
- `TelemetryMqttValidator`;
- validação de topic e payload;
- persistência de Telemetry recebida por MQTT;
- reconexão MQTT no backend;
- frontend React;
- dashboard automotivo;
- histórico e detalhe de eventos;
- testes automatizados;
- GitHub Actions;
- build do simulador C++ no CI;
- documentação da Sprint 2;
- documentação da Sprint 3.

Checkpoint atual da `main` após merge documental:

`7a4436f`

Merge correspondente:

`Merge pull request #13 from devpedro404/docs/update-sprints-02-03`

---

## Fluxo atual do sistema

    C++ Simulator
          ↓
    Telemetry JSON
          ↓
    MQTT / Eclipse Mosquitto
          ↓
    ASP.NET Core
          ↓
    validação
          ↓
    VehicleBlackBoxContext
          ↓
    SQLite
          ↓
    HTTP API
          ↓
    React

MQTT já faz parte do fluxo atual.

Não tratar MQTT como funcionalidade futura da Sprint 2.

---

## Pull Requests principais

### Sprint 1

- PR #3 — Events / Pedro
- PR #4 — Telemetry / Endy
- PR #5 — CI / quality gates

### Sprint 2

- PR #7 — MQTT Simulator
- PR #8 — MQTT Backend
- PR #9 — MQTT Config
- PR #10 — MQTT Reconnect
- PR #11 — MQTT Tests
- PR #12 — Dashboard automotivo

### Documentação

- PR #13 — documentação consolidada das Sprints 2 e 3

---

## Qualidade confirmada

Backend:

- 17 testes passando.

Frontend:

- lint passando;
- production build passando.

Simulador C++:

- build integrado ao CI;
- dependência `libmosquitto-dev` instalada no workflow;
- CMake configure;
- CMake build.

GitHub Actions:

- CI configurado;
- backend testado;
- frontend validado;
- simulador C++ compilado;
- CI verde no PR #13 antes do merge.

---

## Backend

DbContext compartilhado:

`VehicleBlackBoxContext`

DbSets atuais:

- Events
- Vehicles
- Telemetries

Regra:

não criar outro DbContext sem decisão explícita de arquitetura.

---

## Events

Endpoints atuais:

`POST /api/events`

`GET /api/vehicles/{vehicleId}/events`

`GET /api/events/{id}`

O fluxo de Events foi implementado na Sprint 1.

O simulador possui detecção inicial de:

`HARD_BRAKING`

A Sprint 3 irá evoluir esse fluxo com Event Window.

---

## Telemetry / Vehicles

Endpoints atuais:

`POST /api/telemetry`

`GET /api/vehicles`

`GET /api/vehicles/{id}`

`GET /api/vehicles/{id}/telemetry/latest`

A Telemetry pode chegar atualmente por:

- HTTP;
- MQTT.

Ambos os fluxos utilizam a infraestrutura de persistência existente.

---

## MQTT

Broker atual de desenvolvimento:

`Eclipse Mosquitto`

Configuração padrão:

Host:

`localhost`

Port:

`1883`

Topic de publicação:

`vehicle/{vehicleId}/telemetry`

Exemplo:

`vehicle/CAR-001/telemetry`

Subscription do backend:

`vehicle/+/telemetry`

MQTT utiliza:

- QoS 0;
- `retain = false`.

---

## MqttTelemetryService

O backend utiliza:

`MqttTelemetryService`

Tipo:

`BackgroundService`

Responsabilidades:

- conectar ao broker;
- assinar o topic;
- receber mensagens;
- encaminhar validação;
- abrir scope de serviços;
- utilizar o `VehicleBlackBoxContext`;
- persistir Telemetry válida;
- manter loop de reconexão.

---

## TelemetryMqttValidator

A validação MQTT está separada em:

`TelemetryMqttValidator`

Responsabilidades atuais:

- rejeitar JSON inválido;
- rejeitar payload vazio;
- validar `vehicleId`;
- validar `deviceId`;
- validar formato do topic;
- conferir `vehicleId` do topic;
- conferir `vehicleId` do payload;
- rejeitar divergência entre topic e payload.

Exemplo inválido:

    Topic:
    vehicle/CAR-002/telemetry

    Payload:
    vehicleId = CAR-001

Resultado:

`rejeitado`

A mensagem inválida não é persistida.

---

## Reconexão MQTT

O backend não depende de o broker estar ativo quando a aplicação inicia.

Comportamento:

    backend inicia
          ↓
    tenta conectar
          ↓
    falha
          ↓
    aguarda aproximadamente 5 segundos
          ↓
    tenta novamente

Quando a conexão volta:

- conecta;
- reinscreve-se;
- continua recebendo Telemetry.

O simulador também possui reconexão mínima.

---

## Frontend

Rotas atuais:

`/dashboard`

`/events`

`/events/:id`

Dashboard e Events consomem a API real.

O frontend não consome MQTT diretamente.

---

## Dashboard automotivo

O dashboard foi refinado na Sprint 2.

Inclui:

- velocidade;
- RPM;
- temperatura do motor;
- aceleração longitudinal;
- aceleração lateral;
- GPS;
- veículo monitorado;
- dispositivo;
- estado dos dados;
- horário da última atualização;
- indicação de integração MQTT;
- gauges;
- visualização automotiva;
- layout responsivo;
- estados de loading;
- estados de erro;
- acesso ao histórico de eventos.

---

## Simulador

Local:

`embedded/simulator`

Tecnologias:

- C++17
- CMake
- cJSON
- Mosquitto

Já realiza:

- geração de Telemetry;
- detecção inicial de `HARD_BRAKING`;
- serialização JSON;
- publicação MQTT;
- montagem do topic por `vehicleId`;
- tentativa mínima de reconexão.

---

## Git

Não trabalhar diretamente na `main`.

Toda nova branch deve nascer da `main` atualizada.

Antes de iniciar nova branch:

    git switch main
    git fetch --prune origin
    git pull --ff-only origin main
    git status --short
    git log --oneline -10

Depois:

    git switch -c feature/nome-da-feature

ou:

    git switch -c docs/nome-da-alteracao

Não reutilizar branches antigas de outras Sprints.

Branches anteriores não devem ser usadas como base de novos trabalhos.

---

## Sprint 1

Status:

`CONCLUÍDA`

Principais entregas:

- Events;
- Telemetry;
- Vehicles;
- SQLite;
- API;
- React;
- testes automatizados;
- CI inicial.

Documentação:

`docs/sprints/sprint-01-summary.md`

---

## Sprint 2

Status:

`CONCLUÍDA`

Objetivo consolidado:

`C++ Simulator → MQTT → ASP.NET Core → SQLite → React`

Entregas principais:

- MQTT no simulador;
- cJSON;
- publicação de Telemetry;
- MQTT no backend;
- `MqttTelemetryService`;
- `TelemetryMqttValidator`;
- persistência SQLite;
- criação de Vehicle quando necessário;
- `MqttOptions`;
- configuração via `IOptions`;
- reconexão do backend;
- reconexão mínima do simulador;
- testes MQTT;
- total de 17 testes backend;
- build C++ no CI;
- dashboard automotivo;
- integração ponta a ponta.

Documentação oficial:

`docs/sprints/sprint-02.md`

A Sprint 2 não deve mais ser descrita como planejada.

---

## Sprint 3

Status:

`PLANEJADA / PRONTA PARA EXECUÇÃO`

Objetivo principal:

`Event Window`

A Sprint 3 pretende transformar Event de um registro isolado em uma ocorrência com contexto temporal.

Fluxo esperado:

    Vehicle Simulation
          ↓
    Telemetry
          ↓
    Core
          ↓
    Circular Buffer
          ↓
    Event Detection
          ↓
    Event Window
          ↓
    MQTT
          ↓
    ASP.NET Core
          ↓
    Validation
          ↓
    Event + Samples
          ↓
    SQLite
          ↓
    API
          ↓
    React Event Detail
          ↓
    Timeline

Objetivo visual:

    antes do evento
          ↓
    t = 0
          ↓
    depois do evento

Evento inicial de integração:

`HARD_BRAKING`

Documentação oficial:

`docs/sprints/sprint-03.md`

A Sprint 3 ainda não está implementada.

---

## Responsabilidades esperadas na Sprint 3

### Pedro / Core

- buffer circular;
- amostras pré-evento;
- detecção do evento;
- amostras pós-evento;
- construção da Event Window;
- serialização;
- publicação MQTT.

### Endy / Backend

- modelo de Samples;
- relacionamento com Event;
- migration;
- validator de Event Window;
- recepção MQTT;
- persistência;
- endpoint;
- testes.

### Endy / Frontend

- consumo da API;
- timeline;
- gráfico temporal;
- marcação de `t=0`;
- loading;
- erro;
- estado sem dados;
- responsividade.

---

## Dívidas técnicas conhecidas

Continuam válidas:

- estratégia geral de DTOs ainda pode ser consolidada;
- URL da API do frontend ainda merece configuração por ambiente;
- origem de CORS pode ser tornada configurável;
- timestamp SQLite merece revisão de semântica UTC;
- relacionamento Vehicle/Telemetry pode ser fortalecido;
- política de criação automática de Vehicle pode ser revista;
- bundle React pode exigir otimização;
- proteção obrigatória da `main` pode ser reforçada;
- segurança MQTT de produção ainda não existe;
- TLS MQTT ainda não está implementado;
- QoS avançado ainda não está implementado;
- idempotência forte ainda não está implementada.

Itens que NÃO são mais dívida:

- build do simulador C++ no CI;
- MQTT no simulador;
- MQTT no backend;
- reconnect básico;
- testes MQTT.

Esses itens já foram concluídos na Sprint 2.

---

## Fora do escopo atual

Continuam fora do escopo imediato:

- hardware real;
- Raspberry Pi ou equivalente;
- CAN;
- OBD-II;
- GPS físico;
- acelerômetro/giroscópio físico;
- PostgreSQL;
- autenticação;
- TLS MQTT;
- streaming realtime direto para o frontend;
- WebSocket;
- SignalR;
- mapa real;
- relatórios avançados.

Esses itens só devem entrar mediante decisão explícita de escopo.

---

## Como retomar o projeto

Primeiro atualizar e inspecionar:

    git switch main
    git fetch --prune origin
    git pull --ff-only origin main
    git status --short
    git log --oneline -10

Depois ler, nesta ordem:

1. `README.md`
2. `docs/continuity.md`
3. `docs/backlog.md`
4. `docs/architecture.md`
5. `docs/telemetry.md`
6. `docs/events.md`
7. `docs/mqtt.md`
8. `docs/sprints/sprint-02.md`
9. `docs/sprints/sprint-03.md`

Antes de alterar código:

- conferir PRs recentes;
- conferir CI;
- confirmar a branch;
- confirmar o estado atual da `main`;
- não redesenhar a arquitetura sem decisão explícita;
- não migrar para PostgreSQL sem decisão explícita;
- não iniciar hardware real;
- preservar o contrato atual de Telemetry;
- não modificar a feature de outra pessoa sem alinhamento;
- conferir o contrato da Sprint 3 antes da integração.

---

## Fonte de verdade

Se houver divergência entre:

- conversa antiga;
- memória;
- branch antiga;
- documento desatualizado;
- implementação local antiga;

usar esta ordem:

1. `main` atual;
2. documentação versionada;
3. contrato congelado da Sprint;
4. PRs recentemente mergeados.

Não implementar com base apenas em instruções antigas.

---

## Próximo passo

A Sprint 2 está encerrada.

A documentação das Sprints 2 e 3 já está na `main`.

O próximo trabalho funcional é a Sprint 3.

Antes de começar implementação:

- confirmar `main` atualizada;
- confirmar CI verde;
- confirmar contrato inicial da Event Window;
- alinhar topic final de eventos;
- alinhar JSON final da Event Window;
- confirmar comportamento de janela parcial;
- confirmar responsabilidades de Pedro e Endy;
- criar branches novas a partir da `main`.

Meta da próxima Sprint:

`Event Window real, persistida e visualizada do início ao fim.`