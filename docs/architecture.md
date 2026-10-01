# Vehicle Black Box — Architecture

## Objetivo

Construir um sistema de caixa-preta veicular capaz de coletar, processar, transportar, armazenar e visualizar dados de condução sem acoplar o Core a um modelo específico de veículo.

A arquitetura deve preservar separação clara entre:

- aquisição;
- processamento;
- transporte;
- backend;
- persistência;
- visualização.

---

## Arquitetura modular do produto

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

---

## Responsabilidades

### Adaptador do veículo

Responsável por diferenças físicas:

- conector;
- pinagem;
- alimentação;
- chicote;
- acesso OBD/CAN quando aplicável.

O adaptador deve evitar que diferenças físicas entre veículos contaminem o Core.

---

### Módulo de interface

Responsável por conversar com a eletrônica do veículo.

Possíveis tecnologias futuras:

- CAN;
- OBD-II;
- outros barramentos;
- protocolos específicos do veículo.

A responsabilidade desse módulo é traduzir a origem física dos dados para uma interface consistente utilizada pelo Core.

---

### Core Vehicle Black Box

O Core deve permanecer independente do veículo.

Responsabilidades:

- aquisição normalizada de dados;
- geração de timestamp;
- processamento de telemetria;
- regras de eventos;
- buffer;
- Event Window;
- armazenamento local futuro;
- serialização;
- comunicação com a camada de transporte.

No estado atual do projeto, parte dessas responsabilidades é simulada pelo C++ Simulator.

A Sprint 3 irá evoluir especificamente:

- buffer circular;
- contexto pré-evento;
- contexto pós-evento;
- construção de Event Window.

---

### Comunicação

A camada atual de comunicação é MQTT.

MQTT é responsável pelo transporte dos dados entre o simulador/Core e o backend.

MQTT não é:

- banco;
- camada de persistência;
- responsável por regras de negócio;
- responsável por detectar eventos;
- responsável por reconstruir contexto.

Fluxo atual:

    C++ Simulator
          ↓
    MQTT
          ↓
    ASP.NET Core

Broker utilizado no desenvolvimento:

`Eclipse Mosquitto`

Configuração padrão:

    Host: localhost
    Port: 1883

Topic atual de Telemetry:

`vehicle/{vehicleId}/telemetry`

Subscription atual do backend:

`vehicle/+/telemetry`

---

### Backend

Responsabilidades:

- receber dados;
- validar;
- validar consistência de topic/payload;
- organizar;
- persistir;
- expor APIs para o frontend;
- tratar mensagens inválidas sem interromper o serviço;
- gerenciar reconexão MQTT no processo atual.

Tecnologia:

`ASP.NET Core (.NET 10)`

Persistência:

`Entity Framework Core + SQLite`

DbContext compartilhado:

`VehicleBlackBoxContext`

Regra arquitetural:

não criar outro DbContext sem decisão explícita.

O backend não deve conter regras automotivas que pertencem ao Core.

Exemplo:

a detecção de `HARD_BRAKING` pertence ao Core/simulador.

O backend recebe, valida e persiste o resultado.

---

### Banco

Banco atual:

`SQLite`

SQLite é o armazenamento persistente atual da aplicação.

Responsabilidades:

- Events;
- Vehicles;
- Telemetry;
- futuras Samples/EventTelemetry da Sprint 3.

Evolução possível:

`PostgreSQL`

A migração para PostgreSQL não faz parte do escopo atual.

Não migrar de banco sem decisão explícita de arquitetura.

---

### Frontend

Responsabilidades:

- visualizar telemetria;
- visualizar veículos;
- visualizar histórico de eventos;
- visualizar detalhe de eventos;
- tratar loading;
- tratar erro;
- apresentar dados consumidos da API;
- apresentar futuramente Event Window e timeline.

Tecnologia atual:

`React + Vite`

O frontend não deve:

- detectar eventos;
- consumir MQTT diretamente;
- persistir dados;
- reconstruir Event Window;
- implementar regra automotiva do Core.

---

## Arquitetura atual

A arquitetura atual em execução é:

    C++ Simulator
          ↓
    Telemetry JSON
          ↓
    MQTT / Eclipse Mosquitto
          ↓
    ASP.NET Core
          ↓
    Validation
          ↓
    VehicleBlackBoxContext
          ↓
    SQLite
          ↓
    HTTP API
          ↓
    React

Esse fluxo foi consolidado na Sprint 2.

MQTT não deve mais ser descrito como arquitetura futura ou planejada.

Ele já faz parte do runtime atual.

---

## Arquitetura anterior — Sprint 1

A Sprint 1 validou inicialmente os fluxos de Events e Telemetry antes da integração MQTT.

Fluxo simplificado da etapa:

    C++ Simulator
          ↓
    ASP.NET Core
          ↓
    SQLite
          ↓
    React

Essa arquitetura representa um estágio histórico do projeto.

Não representa mais o fluxo principal atual.

---

## Arquitetura de Telemetry atual

Fluxo MQTT:

    C++ Simulator
          ↓
    Telemetry
          ↓
    JSON
          ↓
    vehicle/{vehicleId}/telemetry
          ↓
    Eclipse Mosquitto
          ↓
    vehicle/+/telemetry
          ↓
    MqttTelemetryService
          ↓
    TelemetryMqttValidator
          ↓
    VehicleBlackBoxContext
          ↓
    SQLite

Consulta:

    SQLite
      ↓
    GET /api/vehicles/{id}/telemetry/latest
      ↓
    React Dashboard

---

## MqttTelemetryService

Componente atual do backend:

`MqttTelemetryService`

Tipo:

`BackgroundService`

Responsabilidades arquiteturais:

- criar cliente MQTT;
- conectar ao broker;
- assinar o topic;
- receber mensagens;
- encaminhar payload para validação;
- abrir scope de serviços;
- utilizar o `VehicleBlackBoxContext`;
- persistir Telemetry válida;
- tentar reconectar quando necessário.

O serviço MQTT deve continuar desacoplado do ciclo HTTP da API.

---

## TelemetryMqttValidator

Componente:

`TelemetryMqttValidator`

Responsabilidade:

validar mensagens MQTT antes da persistência.

Verificações atuais:

- JSON válido;
- payload não vazio;
- `vehicleId`;
- `deviceId`;
- formato do topic;
- correspondência entre veículo no topic e no payload.

Exemplo:

    Topic:
    vehicle/CAR-002/telemetry

    Payload:
    vehicleId = CAR-001

Resultado:

`rejeitado`

Essa validação evita persistência de Telemetry no veículo incorreto.

---

## Reconexão

O backend deve permanecer funcional mesmo se o broker MQTT estiver temporariamente indisponível.

Fluxo atual:

    ASP.NET Core inicia
          ↓
    tenta MQTT
          ↓
    conexão falha
          ↓
    backend continua ativo
          ↓
    aguarda aproximadamente 5 segundos
          ↓
    tenta novamente
          ↓
    broker retorna
          ↓
    backend conecta
          ↓
    subscribe novamente

O simulador também possui lógica mínima de reconnect.

---

## Arquitetura do frontend atual

Fluxo:

    MQTT
      ↓
    Backend
      ↓
    SQLite
      ↓
    HTTP API
      ↓
    React

O React não consome MQTT diretamente.

Esse princípio deve ser preservado na Sprint 3.

---

## Arquitetura da Sprint 3

Status:

`PLANEJADA / PRONTA PARA EXECUÇÃO`

Objetivo:

evoluir Events para incluir contexto temporal.

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
    Temporal Timeline

A Event Window deve mostrar:

    antes
      ↓
    t = 0
      ↓
    depois

---

## Event Window — responsabilidade do Core

A geração da janela pertence ao Core.

Responsabilidades planejadas:

- manter buffer circular;
- armazenar amostras recentes;
- detectar evento;
- preservar contexto anterior;
- coletar contexto posterior;
- definir o instante `t=0`;
- montar Event Window;
- serializar;
- publicar via MQTT.

O backend não deve reconstruir retroativamente a janela a partir de Telemetry solta.

---

## Event Window — responsabilidade do Backend

Responsabilidades planejadas:

- receber a Event Window;
- validar contrato;
- validar topic/payload;
- persistir Event;
- persistir Samples;
- relacionar Samples ao Event;
- expor consulta;
- ordenar Samples temporalmente;
- responder 404 para Event inexistente;
- tratar payload inválido sem crash.

Endpoint planejado:

`GET /api/events/{id}/telemetry`

A nomenclatura final do modelo poderá ser:

`EventTelemetry`

ou:

`EventSample`

A decisão deve ser congelada no contrato da Sprint 3 antes da implementação definitiva.

---

## Event Window — responsabilidade do Frontend

Responsabilidades planejadas:

- buscar Event;
- buscar Event Window;
- mostrar Samples;
- organizar timeline;
- mostrar velocidade;
- mostrar aceleração longitudinal;
- usar eixo de tempo relativo;
- marcar `t=0`;
- tratar loading;
- tratar erro;
- tratar ausência de Samples;
- funcionar em desktop;
- funcionar em mobile.

O frontend continua sem responsabilidade de detectar o evento.

---

## Topic de Events planejado para Sprint 3

Direção atual documentada:

`vehicle/{vehicleId}/events`

Exemplo:

`vehicle/CAR-001/events`

Esse topic ainda pertence ao contrato da Sprint 3 e deve ser confirmado antes da implementação final.

Diferentemente do topic de Telemetry, ele ainda não deve ser tratado como funcionalidade implementada até a integração da Sprint 3 ser concluída.

---

## Persistência atual e futura

### Atual

Entidades principais:

- Event;
- Vehicle;
- Telemetry.

Contexto:

`VehicleBlackBoxContext`

Banco:

`SQLite`

### Sprint 3

Planejado:

- Event;
- EventSample/EventTelemetry;
- relacionamento Event → Samples;
- FK;
- índice por EventId;
- migration correspondente.

O contexto deve continuar sendo:

`VehicleBlackBoxContext`

---

## Arquitetura futura de hardware

Visão futura:

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

Possíveis componentes futuros:

- Raspberry Pi ou equivalente;
- microcontrolador ou computador embarcado apropriado;
- interface CAN;
- OBD-II;
- GPS físico;
- acelerômetro;
- giroscópio;
- armazenamento local;
- sincronização posterior.

Esses componentes não fazem parte da implementação atual.

---

## Princípios de desacoplamento

### Core

Deve conhecer:

- dados normalizados;
- telemetria;
- regras automotivas;
- eventos;
- buffer;
- Event Window;
- comunicação abstrata.

Não deve depender de:

- React;
- SQLite;
- detalhes de API HTTP;
- modelo específico do veículo.

### MQTT

Deve transportar.

Não deve:

- persistir;
- detectar eventos;
- aplicar lógica de UI.

### Backend

Deve:

- receber;
- validar;
- persistir;
- expor API.

Não deve:

- detectar comportamento automotivo principal;
- reconstruir lógica que pertence ao Core.

### Frontend

Deve:

- consultar;
- apresentar;
- facilitar leitura.

Não deve:

- detectar eventos;
- consumir MQTT diretamente;
- armazenar estado persistente do veículo.

---

## Quality gates arquiteturais

A arquitetura atual é protegida por CI.

Backend:

- restore;
- testes automatizados.

Frontend:

- instalação;
- lint;
- build.

C++:

- instalação de dependência Mosquitto;
- CMake configure;
- CMake build.

Checkpoint atual:

`17 testes backend passando`

O build C++ já faz parte do CI.

---

## Fora do escopo arquitetural atual

Não introduzir sem decisão explícita:

- PostgreSQL;
- hardware real;
- CAN real;
- OBD-II real;
- autenticação;
- TLS MQTT;
- WebSocket;
- SignalR;
- streaming MQTT direto para React;
- múltiplos DbContexts;
- reescrita do frontend;
- troca do broker;
- nova arquitetura global.

---

## Evoluções possíveis

Futuro:

- novos tipos de evento;
- Event Window configurável;
- severidade;
- deduplicação;
- cooldown;
- WebSocket;
- SignalR;
- PostgreSQL;
- autenticação;
- TLS MQTT;
- QoS avançado;
- idempotência forte;
- hardware real;
- armazenamento embarcado;
- sincronização offline;
- observabilidade avançada.

---

## Fonte de verdade arquitetural

Em caso de divergência entre:

- documentação antiga;
- conversa;
- branch antiga;
- memória;
- código local desatualizado;

usar esta prioridade:

1. `main` atual;
2. documentação versionada;
3. contrato da Sprint atual;
4. PRs mergeados recentemente.

Documentação relevante:

- `README.md`
- `docs/continuity.md`
- `docs/backlog.md`
- `docs/architecture.md`
- `docs/telemetry.md`
- `docs/events.md`
- `docs/mqtt.md`
- `docs/sprints/sprint-02.md`
- `docs/sprints/sprint-03.md`

---

## Estado arquitetural atual

Sprint 1:

`CONCLUÍDA`

Sprint 2:

`CONCLUÍDA`

Sprint 3:

`PLANEJADA / PRONTA PARA EXECUÇÃO`

Arquitetura atual:

`C++ Simulator → MQTT → ASP.NET Core → SQLite → React`

Próxima evolução arquitetural:

`C++ Core → Event Window → MQTT → ASP.NET Core → SQLite → API → React Timeline`