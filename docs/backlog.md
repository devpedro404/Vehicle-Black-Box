# Vehicle Black Box — Backlog Mestre

> Fonte de verdade do estado funcional e da evolução planejada do projeto.
> Atualizado em: 2026-09-30.
> Estado atual: Sprint 1 concluída; Sprint 2 concluída; Sprint 3 planejada e pronta para execução.
> Fonte principal: `main` atual + documentação versionada das Sprints.

## 1. Objetivo do projeto

Construir uma caixa-preta veicular capaz de coletar, processar, transportar, armazenar e visualizar dados de condução, começando por dados simulados e evoluindo posteriormente para hardware real.

O sistema deve preservar a separação entre:

- aquisição e processamento no Core;
- transporte;
- backend;
- persistência;
- frontend.

A evolução deve ocorrer sem acoplar o Core a um modelo específico de veículo.

---

## 2. Arquitetura do produto

Arquitetura conceitual:

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

Princípios:

- o Core deve permanecer independente do modelo do veículo;
- diferenças físicas ficam no adaptador/chicote;
- diferenças de comunicação com o veículo ficam no módulo de interface;
- regras de detecção pertencem ao Core;
- MQTT é transporte;
- o backend valida, organiza e persiste;
- o frontend não contém regras de detecção de eventos;
- SQLite é o banco atual;
- PostgreSQL permanece como evolução futura;
- hardware real não deve ser introduzido sem decisão explícita de arquitetura.

---

## 3. Arquitetura atual em execução

Fluxo atual:

    C++ Simulator
          ↓
    Telemetry JSON
          ↓
    MQTT / Eclipse Mosquitto
          ↓
    ASP.NET Core
          ↓
    VehicleBlackBoxContext
          ↓
    SQLite
          ↓
    HTTP API
          ↓
    React

MQTT já faz parte da arquitetura implementada.

O fluxo com MQTT não deve mais ser tratado como planejamento futuro da Sprint 2.

---

## 4. Stack atual

- C++17
- CMake
- cJSON
- Eclipse Mosquitto
- MQTT
- C# / ASP.NET Core (.NET 10)
- Entity Framework Core
- SQLite
- React
- Vite
- xUnit
- GitHub Actions

---

## 5. Sprint 0 — Fundação

Status:

CONCLUÍDA

Entregas:

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

---

## 6. Sprint 1A — Pedro / Events

Status:

CONCLUÍDA

Objetivo:

    Event → SQLite → ASP.NET Core → React

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

PR principal:

`#3 — Feature/events flow`

---

## 7. Sprint 1B — Endy / Telemetry

Status:

CONCLUÍDA

Objetivo:

    React Dashboard → ASP.NET Core → Telemetry → SQLite

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

PR principal:

`#4 — feat: conclui fluxo de telemetria da Sprint 1`

---

## 8. Quality gates — Sprint 1

Status:

CONCLUÍDO

- [x] Teste placeholder removido
- [x] 13 testes backend passando no checkpoint da Sprint 1
- [x] Frontend lint: 0 warnings / 0 errors
- [x] Frontend build: sucesso
- [x] GitHub Actions configurado
- [x] Job `Backend Tests`
- [x] Job `Frontend Quality`
- [x] CI verde no PR e após merge na `main`

PR:

`#5 — ci: adiciona quality gates automatizados`

A Sprint 2 ampliou posteriormente esses quality gates.

---

## 9. Contrato atual de Telemetry

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

    {
      "deviceId": "VBB-001",
      "vehicleId": "CAR-001",
      "timestamp": "2026-09-30T04:00:00Z",
      "speed": 80,
      "rpm": 3000,
      "engineTemperature": 89,
      "longitudinalAcceleration": -0.5,
      "lateralAcceleration": 0.2,
      "latitude": -3.119027,
      "longitude": -60.021731
    }

`connectionStatus` e `softwareVersion` permanecem reservados para evolução futura de Device.

---

## 10. Sprint 2 — Integração MQTT e confiabilidade do transporte

Status:

CONCLUÍDA

Objetivo:

    C++ Simulator
          ↓
    MQTT
          ↓
    ASP.NET Core
          ↓
    SQLite
          ↓
    React

A Sprint 2 integrou o simulador C++ ao backend por MQTT preservando o contrato existente de Telemetry.

---

## 11. Sprint 2 — MQTT Simulator

PR:

`#7 — MQTT Simulator`

Concluído:

- [x] integração Mosquitto no simulador;
- [x] conexão MQTT;
- [x] cJSON;
- [x] serialização da Telemetry;
- [x] publicação MQTT;
- [x] topic construído com `vehicleId`;
- [x] QoS 0;
- [x] `retain = false`;
- [x] lógica mínima de reconnect;
- [x] simulação continua mesmo sem publicação;
- [x] build do simulador adicionado ao CI.

Topic atual:

`vehicle/{vehicleId}/telemetry`

Exemplo:

`vehicle/CAR-001/telemetry`

---

## 12. Sprint 2 — MQTT Backend

PR:

`#8 — MQTT Backend`

Concluído:

- [x] cliente MQTT no ASP.NET Core;
- [x] `MqttTelemetryService`;
- [x] execução como `BackgroundService`;
- [x] subscribe em `vehicle/+/telemetry`;
- [x] recepção de payload MQTT;
- [x] desserialização;
- [x] validação;
- [x] persistência SQLite;
- [x] utilização do `VehicleBlackBoxContext`;
- [x] criação de Vehicle quando necessário;
- [x] tratamento de mensagens inválidas sem crash.

---

## 13. Sprint 2 — Configuração MQTT

PR:

`#9 — MQTT Config`

Concluído:

- [x] `MqttOptions`;
- [x] configuração externa;
- [x] integração via `IOptions<MqttOptions>`;
- [x] host removido da regra de negócio;
- [x] port removida da regra de negócio;
- [x] clientId removido da regra de negócio;
- [x] topic removido da regra de negócio.

Configuração padrão de desenvolvimento:

    Host: localhost
    Port: 1883
    ClientId: vehicle-black-box-backend
    Topic: vehicle/+/telemetry

---

## 14. Sprint 2 — Reconexão

PR:

`#10 — MQTT Reconnect`

Concluído:

- [x] backend tolera broker indisponível;
- [x] tentativas contínuas de conexão;
- [x] intervalo aproximado de 5 segundos;
- [x] reconexão após retorno do broker;
- [x] nova inscrição no topic após reconectar;
- [x] backend HTTP continua ativo sem broker;
- [x] simulador possui reconnect mínimo.

Fluxo:

    backend inicia
          ↓
    tenta conectar
          ↓
    falha
          ↓
    espera aproximadamente 5 segundos
          ↓
    tenta novamente

---

## 15. Sprint 2 — Validação MQTT

PR:

`#11 — MQTT Tests`

Componente:

`TelemetryMqttValidator`

Concluído:

- [x] extração da lógica de validação;
- [x] JSON inválido é rejeitado;
- [x] payload vazio é rejeitado;
- [x] `vehicleId` obrigatório;
- [x] `deviceId` obrigatório;
- [x] formato do topic validado;
- [x] veículo do topic comparado ao payload;
- [x] divergência é rejeitada;
- [x] mensagem inválida não é persistida;
- [x] serviço continua funcionando após payload inválido.

Exemplo inválido:

    Topic:
    vehicle/CAR-002/telemetry

    Payload:
    vehicleId = CAR-001

Resultado:

`rejeitado`

---

## 16. Sprint 2 — Testes

Checkpoint anterior:

`13 testes`

Checkpoint após Sprint 2:

`17 testes`

Novos cenários:

- [x] payload MQTT válido;
- [x] JSON inválido;
- [x] `deviceId` ausente;
- [x] divergência de `vehicleId` entre topic e payload.

Resultado:

`17/17 testes backend passando`

---

## 17. Sprint 2 — CI

Quality gates atuais:

### Backend

- [x] `dotnet restore`
- [x] `dotnet test`

### Frontend

- [x] `npm ci`
- [x] `npm run lint`
- [x] `npm run build`

### Simulador C++

- [x] instalação de `libmosquitto-dev`
- [x] CMake configure
- [x] CMake build

O simulador C++ faz parte do CI atual.

A antiga dívida:

`adicionar build/testes do simulador C++ ao CI`

deve ser considerada encerrada.

---

## 18. Sprint 2 — Dashboard automotivo

PR:

`#12 — Dashboard automotivo`

Status:

CONCLUÍDO

Entregas:

- [x] redesign do dashboard;
- [x] visual automotivo;
- [x] gauges;
- [x] velocidade;
- [x] RPM;
- [x] temperatura;
- [x] aceleração longitudinal;
- [x] aceleração lateral;
- [x] GPS;
- [x] veículo monitorado;
- [x] dispositivo;
- [x] horário da última atualização;
- [x] indicação de integração MQTT;
- [x] loading;
- [x] erro;
- [x] responsividade desktop;
- [x] responsividade tablet;
- [x] responsividade mobile;
- [x] acesso ao histórico de eventos;
- [x] consumo da API real preservado.

---

## 19. Sprint 2 — Resultado final

Status:

CONCLUÍDA

Fluxo validado:

    C++ Simulator
          ↓
    MQTT
          ↓
    ASP.NET Core
          ↓
    SQLite
          ↓
    React

Entregas consolidadas:

- [x] MQTT Publisher no C++;
- [x] MQTT Subscriber no backend;
- [x] contrato de Telemetry preservado;
- [x] Mosquitto;
- [x] cJSON;
- [x] validação;
- [x] persistência;
- [x] configuração externa;
- [x] reconexão;
- [x] testes;
- [x] CI do C++;
- [x] dashboard automotivo;
- [x] integração ponta a ponta.

PRs da Sprint 2:

- [x] #7
- [x] #8
- [x] #9
- [x] #10
- [x] #11
- [x] #12

Documentação oficial:

`docs/sprints/sprint-02.md`

---

## 20. Documentação das Sprints

PR:

`#13 — documentação consolidada das Sprints 2 e 3`

Concluído:

- [x] `docs/sprints/sprint-02.md`;
- [x] `docs/sprints/sprint-03.md`;
- [x] Sprint 2 registrada como concluída;
- [x] Sprint 3 registrada como planejada e pronta para execução;
- [x] documentação mergeada na `main`.

---

## 21. Estado atual dos eventos

Eventos documentados:

- `HARD_BRAKING`
- `HARD_ACCELERATION`
- `SPEEDING`
- `HARD_CORNERING`
- `HIGH_ENGINE_TEMPERATURE`
- `POSSIBLE_IMPACT`

Neste checkpoint, a detecção inicial implementada no simulador é:

`HARD_BRAKING`

Os demais eventos permanecem como evolução futura.

---

## 22. Sprint 3 — Event Window

Status:

PLANEJADA / PRONTA PARA EXECUÇÃO

Objetivo:

transformar um Event isolado em uma ocorrência com contexto temporal antes, durante e depois do evento.

Fluxo planejado:

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

Critério central:

    antes
      ↓
    t = 0
      ↓
    depois

---

## 23. Sprint 3 — Contrato

Antes da implementação integrada, congelar:

- [ ] topic final de Events;
- [ ] JSON final da Event Window;
- [ ] campos obrigatórios;
- [ ] `relativeTimeMs`;
- [ ] timestamp;
- [ ] semântica de `t=0`;
- [ ] comportamento para janela parcial;
- [ ] quantidade/duração de amostras;
- [ ] regra de cooldown;
- [ ] regra de duplicidade.

Topic inicialmente previsto:

`vehicle/{vehicleId}/events`

Exemplo:

`vehicle/CAR-001/events`

Esse contrato deve ser confirmado antes da integração final.

---

## 24. Sprint 3 — Pedro / Core

Responsabilidades planejadas:

- [ ] buffer circular de telemetria;
- [ ] preservação de amostras pré-evento;
- [ ] detecção de `HARD_BRAKING`;
- [ ] congelamento do contexto;
- [ ] captura de amostras pós-evento;
- [ ] montagem da Event Window;
- [ ] serialização;
- [ ] publicação MQTT;
- [ ] entrega de payload real para integração.

---

## 25. Sprint 3 — Endy / Backend

Responsabilidades planejadas:

- [ ] entidade `EventTelemetry` ou `EventSample`;
- [ ] relacionamento com `Event`;
- [ ] FK;
- [ ] índice por EventId;
- [ ] migration;
- [ ] DTO;
- [ ] validator de Event Window;
- [ ] configuração do topic;
- [ ] recepção MQTT;
- [ ] persistência do Event;
- [ ] persistência das Samples;
- [ ] tratamento de payload inválido;
- [ ] endpoint de consulta;
- [ ] testes backend.

Endpoint planejado:

`GET /api/events/{id}/telemetry`

Comportamento esperado:

- evento existente: 200;
- evento inexistente: 404;
- Samples ordenadas temporalmente.

---

## 26. Sprint 3 — Endy / Frontend

Responsabilidades planejadas:

- [ ] service para Event Window;
- [ ] consumo do endpoint real;
- [ ] timeline;
- [ ] gráfico temporal;
- [ ] eixo X baseado em tempo relativo;
- [ ] marcação visual de `t=0`;
- [ ] velocidade;
- [ ] aceleração longitudinal;
- [ ] loading;
- [ ] erro;
- [ ] estado sem Samples;
- [ ] responsividade desktop;
- [ ] responsividade mobile.

O frontend não deve:

- detectar eventos;
- reconstruir a Event Window;
- consumir MQTT diretamente.

---

## 27. Sprint 3 — Testes planejados

Backend:

- [ ] payload válido;
- [ ] JSON inválido;
- [ ] `vehicleId` ausente;
- [ ] `deviceId` ausente;
- [ ] Samples vazio;
- [ ] topic/payload divergentes;
- [ ] Event persistido;
- [ ] Samples persistidas;
- [ ] FK correta;
- [ ] GET janela existente;
- [ ] GET evento inexistente;
- [ ] ordenação temporal;
- [ ] comportamento de janela parcial.

Frontend:

- [ ] loading;
- [ ] erro;
- [ ] sem Samples;
- [ ] dados válidos;
- [ ] `t=0` visível;
- [ ] desktop;
- [ ] mobile.

---

## 28. Sprint 3 — E2E planejado

Cenário principal:

1. `CAR-001` trafega normalmente.
2. Telemetria é gerada.
3. Ocorre `HARD_BRAKING`.
4. Core detecta o evento.
5. Core preserva dados anteriores.
6. Core coleta dados posteriores.
7. Event Window é montada.
8. Event Window é publicada via MQTT.
9. Backend recebe.
10. Backend valida.
11. Event é persistido.
12. Samples são persistidas.
13. API retorna a janela.
14. React abre o evento.
15. Timeline mostra antes, `t=0` e depois.

Evidências esperadas:

- [ ] log do simulador;
- [ ] payload MQTT;
- [ ] log do backend;
- [ ] registros no SQLite;
- [ ] resposta da API;
- [ ] gráfico no frontend;
- [ ] CI verde.

---

## 29. Sprint 3 — Definition of Done

A Sprint 3 será considerada concluída quando:

- [ ] Event Window for produzida pelo Core;
- [ ] contexto anterior for preservado;
- [ ] contexto posterior for preservado;
- [ ] publicação MQTT funcionar;
- [ ] backend receber;
- [ ] backend validar;
- [ ] Event for persistido;
- [ ] Samples forem persistidas;
- [ ] API retornar a janela;
- [ ] React exibir dados reais;
- [ ] `t=0` estiver claramente identificado;
- [ ] testes backend passarem;
- [ ] frontend lint passar;
- [ ] frontend build passar;
- [ ] simulador compilar;
- [ ] CI estiver verde;
- [ ] E2E estiver validado;
- [ ] documentação estiver atualizada.

Documentação oficial:

`docs/sprints/sprint-03.md`

---

## 30. Ordem recomendada da Sprint 3

### Etapa 1

Congelar contrato.

### Etapa 2

Implementar modelo e persistência.

### Etapa 3

Implementar Core / Event Window.

### Etapa 4

Implementar recepção e validação MQTT.

### Etapa 5

Implementar API.

### Etapa 6

Implementar timeline no frontend.

### Etapa 7

Integrar payload real.

### Etapa 8

Executar E2E.

### Etapa 9

Guardar evidências.

### Etapa 10

Atualizar documentação e encerrar Sprint.

---

## 31. Dívida técnica conhecida

Pontos ainda válidos:

- [ ] consolidar estratégia de DTOs;
- [ ] revisar semântica UTC no SQLite;
- [ ] mover URL da API do frontend para configuração de ambiente;
- [ ] tornar origem de CORS configurável;
- [ ] fortalecer relacionamento Vehicle → Telemetry;
- [ ] revisar política de criação automática de Vehicle;
- [ ] otimizar bundle React quando necessário;
- [ ] proteger `main` com regras/checks obrigatórios, se ainda não configurado;
- [ ] adicionar segurança MQTT para produção;
- [ ] implementar TLS MQTT quando houver ambiente real;
- [ ] avaliar QoS superior quando necessário;
- [ ] avaliar idempotência forte;
- [ ] ampliar testes E2E automatizados no futuro.

---

## 32. Dívidas encerradas pela Sprint 2

Não manter estes itens como pendentes:

- [x] MQTT no simulador;
- [x] MQTT no backend;
- [x] validação de MQTT;
- [x] reconnect backend;
- [x] reconnect mínimo no simulador;
- [x] testes MQTT;
- [x] build do simulador C++ no CI;
- [x] dashboard automotivo.

---

## 33. Fora do escopo atual

Não fazem parte do escopo imediato da Sprint 3:

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
- relatórios avançados;
- implementação completa de todos os eventos possíveis.

Esses itens só devem entrar mediante nova decisão de escopo.

---

## 34. Evolução futura

Após a Sprint 3 poderão ser avaliados:

- novos tipos de eventos;
- Event Windows configuráveis;
- severidade de eventos;
- deduplicação avançada;
- cooldown avançado;
- múltiplas métricas na timeline;
- exportação;
- relatórios;
- WebSocket;
- SignalR;
- PostgreSQL;
- autenticação;
- MQTT TLS;
- observabilidade;
- hardware real;
- CAN;
- OBD-II;
- armazenamento embarcado;
- sincronização offline.

---

## 35. Regras de Git

Nunca trabalhar diretamente na `main`.

Antes de iniciar uma nova branch:

    git switch main
    git fetch --prune origin
    git pull --ff-only origin main
    git status --short
    git log --oneline -10

Criar branch específica:

    git switch -c feature/nome-da-feature

ou:

    git switch -c docs/nome-da-documentacao

Regras:

- não reutilizar branch antiga;
- não usar force push na `main`;
- manter PRs focados;
- rodar testes antes do push;
- conferir CI;
- esperar CI verde antes do merge;
- atualizar `main` após cada merge relevante.

---

## 36. Continuidade

Ao retomar o projeto, ler nesta ordem:

1. `README.md`
2. `docs/continuity.md`
3. `docs/backlog.md`
4. `docs/architecture.md`
5. `docs/telemetry.md`
6. `docs/events.md`
7. `docs/mqtt.md`
8. `docs/sprints/sprint-02.md`
9. `docs/sprints/sprint-03.md`

Sempre conferir:

- `main`;
- PRs recentes;
- CI;
- branch atual;
- documentação da Sprint;
- contrato atual.

---

## 37. Fonte de verdade

Em caso de divergência entre:

- conversa;
- memória;
- branch antiga;
- documento antigo;
- instrução anterior;
- implementação local desatualizada;

usar esta prioridade:

1. `main` atual;
2. documentação versionada;
3. contrato congelado da Sprint;
4. PRs mergeados recentemente.

Não implementar funcionalidade com base apenas em conversa antiga.

---

## 38. Checkpoint atual

Sprint 0:

CONCLUÍDA

Sprint 1:

CONCLUÍDA

Sprint 2:

CONCLUÍDA

Sprint 3:

PLANEJADA / PRONTA PARA EXECUÇÃO

Último marco documental:

`PR #13`

Objetivo atual do projeto:

`Event Window real, persistida e visualizada do início ao fim.`

Critério principal da Sprint 3:

`abrir um evento no React e enxergar o comportamento do veículo antes, em t=0 e depois da ocorrência.`