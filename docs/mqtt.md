# Vehicle Black Box — MQTT

## Status

Sprint 2 concluída.

MQTT já faz parte da arquitetura atual do projeto.

Fluxo atual:

    C++ Simulator
          ↓
    MQTT / Eclipse Mosquitto
          ↓
    ASP.NET Core
          ↓
    SQLite
          ↓
    React

MQTT não deve mais ser tratado como funcionalidade futura ou apenas planejada.

A Sprint 3 reutilizará a infraestrutura MQTT existente para transportar Event Window.

---

## Objetivo

Usar MQTT como camada de comunicação entre o simulador/Core em C++ e o backend em C#.

Princípio arquitetural:

- o Core produz dados;
- MQTT transporta;
- o backend valida e persiste;
- SQLite armazena;
- a API expõe;
- o React visualiza.

MQTT não contém regra de persistência.

MQTT não detecta eventos.

MQTT não substitui o backend.

O frontend não consome MQTT diretamente.

---

## Broker de desenvolvimento

Broker atual:

`Eclipse Mosquitto`

Configuração padrão:

    Host: localhost
    Port: 1883

Docker não é obrigatório para o fluxo atual.

O broker deve permanecer uma dependência de infraestrutura, não uma regra de negócio da aplicação.

---

## Telemetry — Topic implementado

Publisher:

`vehicle/{vehicleId}/telemetry`

Exemplo:

`vehicle/CAR-001/telemetry`

Backend subscription:

`vehicle/+/telemetry`

Esse contrato está implementado.

---

## Telemetry — QoS e retain

Configuração atual:

    QoS: 0
    retain: false

A Sprint 2 adotou essa configuração para o fluxo atual de desenvolvimento.

QoS superior pode ser considerado futuramente se houver necessidade real.

---

## Payload atual de Telemetry

O MQTT preserva o contrato existente de Telemetry.

Campos:

- `deviceId`
- `vehicleId`
- `timestamp`
- `speed`
- `rpm`
- `engineTemperature`
- `longitudinalAcceleration`
- `lateralAcceleration`
- `latitude`
- `longitude`

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

Não foi criado um segundo contrato de Telemetry apenas para MQTT.

---

## Simulador C++ — Publisher

Local:

`embedded/simulator`

Responsabilidades atuais:

- gerar Telemetry;
- serializar Telemetry em JSON;
- montar o topic pelo `vehicleId`;
- conectar ao broker;
- publicar mensagens;
- detectar perda de conexão;
- tentar reconectar;
- continuar simulando mesmo quando uma publicação não puder ocorrer.

Tecnologias relevantes:

- C++17
- CMake
- cJSON
- Mosquitto

---

## Serialização no C++

A serialização utiliza cJSON.

Campos publicados:

- `deviceId`;
- `vehicleId`;
- `timestamp`;
- `speed`;
- `rpm`;
- `engineTemperature`;
- `longitudinalAcceleration`;
- `lateralAcceleration`;
- `latitude`;
- `longitude`.

Exemplo de topic gerado:

`vehicle/CAR-001/telemetry`

---

## Backend — Subscriber

O backend utiliza:

`MqttTelemetryService`

Tipo:

`BackgroundService`

Responsabilidades:

- criar cliente MQTT;
- conectar ao broker;
- assinar topic;
- receber mensagens;
- encaminhar payload para validação;
- abrir scope;
- utilizar `VehicleBlackBoxContext`;
- persistir Telemetry válida;
- tentar reconectar quando necessário.

O loop MQTT deve permanecer independente da API HTTP.

---

## Configuração MQTT no Backend

O backend utiliza configuração externa.

Estrutura atual:

    "Mqtt": {
      "Host": "localhost",
      "Port": 1883,
      "ClientId": "vehicle-black-box-backend",
      "Topic": "vehicle/+/telemetry"
    }

O acesso é realizado através de:

`IOptions<MqttOptions>`

Objetivo:

evitar configuração de infraestrutura hardcoded na regra principal do serviço.

---

## Validação de mensagens

Componente:

`TelemetryMqttValidator`

Responsabilidades:

- desserializar JSON;
- rejeitar JSON inválido;
- rejeitar payload vazio;
- validar `vehicleId`;
- validar `deviceId`;
- validar formato do topic;
- extrair veículo do topic;
- comparar topic e payload.

---

## Topic e payload divergentes

Exemplo inválido:

    Topic:
    vehicle/CAR-002/telemetry

    Payload:
    {
      "vehicleId": "CAR-001"
    }

Resultado:

`rejeitado`

Motivo:

o `vehicleId` do topic difere do `vehicleId` do payload.

A mensagem não deve ser persistida.

---

## JSON inválido

Comportamento esperado e implementado:

    mensagem inválida
          ↓
    validator rejeita
          ↓
    warning no log
          ↓
    mensagem descartada
          ↓
    backend continua ativo

JSON inválido não deve derrubar o serviço MQTT.

---

## Persistência

MQTT não possui persistência própria.

Fluxo:

    MQTT
      ↓
    MqttTelemetryService
      ↓
    TelemetryMqttValidator
      ↓
    VehicleBlackBoxContext
      ↓
    SQLite

A Telemetry recebida por MQTT utiliza a mesma infraestrutura de persistência já usada pelo projeto.

Não criar outro DbContext para o fluxo MQTT.

---

## Vehicle

Ao receber uma Telemetry válida, o backend verifica o veículo.

Se o Vehicle ainda não existir, o fluxo atual pode criá-lo antes da persistência da Telemetry.

Essa política pode ser revisada futuramente, mas faz parte do comportamento atual.

---

## API depois do MQTT

O frontend não lê mensagens diretamente do broker.

Fluxo:

    MQTT
      ↓
    Backend
      ↓
    SQLite
      ↓
    API HTTP
      ↓
    React

Endpoint principal para a última Telemetry:

`GET /api/vehicles/{id}/telemetry/latest`

Exemplo:

`GET /api/vehicles/CAR-001/telemetry/latest`

A API retorna a Telemetry mais recente persistida.

---

## Reconexão do Backend

O backend não depende de o broker já estar disponível no momento da inicialização.

Fluxo:

    Backend inicia
          ↓
    tenta conectar
          ↓
    falha
          ↓
    aguarda aproximadamente 5 segundos
          ↓
    tenta novamente

Quando o broker retorna:

- backend conecta;
- reinscreve-se no topic;
- continua recebendo Telemetry.

---

## Reconexão do Simulador

O simulador possui reconnect mínimo.

Comportamento:

- quando conectado, publica;
- quando desconectado, tenta reconectar;
- se não conseguir, pula aquela publicação;
- a simulação continua.

Esse comportamento evita que indisponibilidade temporária do broker derrube o processo inteiro.

---

## Testes MQTT

Checkpoint antes dos testes MQTT:

`13 testes backend`

Checkpoint após Sprint 2:

`17 testes backend`

Cenários adicionados:

- payload MQTT válido;
- JSON inválido;
- `deviceId` ausente;
- divergência entre `vehicleId` do topic e payload.

Resultado registrado:

`17/17 testes passando`

---

## CI

O MQTT também impactou o CI do simulador.

Quality gates atuais:

### Backend

- `dotnet restore`
- `dotnet test`

### Frontend

- `npm ci`
- `npm run lint`
- `npm run build`

### C++ Simulator

- instalação de `libmosquitto-dev`;
- CMake configure;
- CMake build.

O build C++ já faz parte do CI.

---

## Sprint 2

Status:

`CONCLUÍDA`

PRs principais relacionados a MQTT:

- #7 — MQTT Simulator
- #8 — MQTT Backend
- #9 — MQTT Config
- #10 — MQTT Reconnect
- #11 — MQTT Tests

PR adicional da Sprint 2:

- #12 — Dashboard automotivo

Documentação oficial:

`docs/sprints/sprint-02.md`

---

## Events — Topic planejado para Sprint 3

Direção atual:

`vehicle/{vehicleId}/events`

Exemplo:

`vehicle/CAR-001/events`

Esse topic pertence ao plano da Sprint 3.

Ele não deve ser tratado como implementado enquanto a integração de Event Window não estiver concluída.

---

## Event Window

A Sprint 3 utilizará MQTT para transportar uma Event Window.

Fluxo planejado:

    Core
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
    React Timeline

MQTT continuará sendo somente o transporte.

---

## Contrato da Event Window

Antes da implementação final, confirmar:

- topic final;
- estrutura JSON;
- `vehicleId`;
- `deviceId`;
- tipo do evento;
- timestamp do evento;
- `relativeTimeMs`;
- estrutura de Samples;
- comportamento de janela parcial;
- política de duplicidade;
- cooldown.

A documentação da Sprint 3 contém o contrato inicial.

Arquivo:

`docs/sprints/sprint-03.md`

---

## Status do dispositivo

Topic reservado para evolução futura:

`vehicle/{vehicleId}/status`

Exemplo:

`vehicle/CAR-001/status`

Esse fluxo não faz parte da implementação atual.

Não tratar como concluído.

---

## Segurança MQTT

Situação atual:

desenvolvimento local.

Ainda não implementado:

- autenticação MQTT de produção;
- TLS;
- gestão de certificados;
- autorização por topic;
- rotação de credenciais.

Esses itens pertencem a uma evolução futura.

---

## QoS futuro

Atual:

`QoS 0`

Possíveis evoluções:

- QoS 1;
- análise de duplicidade;
- idempotência;
- confirmação de entrega.

Não alterar QoS sem necessidade e sem avaliar impacto no backend.

---

## Regras arquiteturais

- MQTT é transporte, não banco;
- MQTT não contém regra de persistência;
- MQTT não detecta eventos;
- backend continua responsável por validação e persistência;
- frontend não consome MQTT diretamente;
- Telemetry mantém um único contrato;
- topic e payload devem ser coerentes;
- mensagens inválidas não devem derrubar o serviço;
- `VehicleBlackBoxContext` continua sendo o contexto compartilhado;
- Event Window deve ser criada no Core;
- backend não deve reconstruir retrospectivamente a Event Window;
- alterações de contrato devem ser documentadas antes da integração.

---

## Fora do escopo atual

Não fazem parte do MQTT atual:

- TLS de produção;
- autenticação;
- QoS avançado;
- idempotência forte;
- cluster MQTT;
- alta disponibilidade;
- broker gerenciado em nuvem;
- streaming MQTT direto para React;
- hardware real;
- CAN;
- OBD-II.

---

## Fonte de verdade

Em caso de divergência entre:

- documentação antiga;
- conversa;
- branch antiga;
- memória;
- implementação local desatualizada;

usar esta ordem:

1. `main` atual;
2. documentação versionada;
3. contrato da Sprint;
4. PRs mergeados recentemente.

Documentação relacionada:

- `README.md`
- `docs/architecture.md`
- `docs/telemetry.md`
- `docs/events.md`
- `docs/continuity.md`
- `docs/backlog.md`
- `docs/sprints/sprint-02.md`
- `docs/sprints/sprint-03.md`

---

## Estado atual

Telemetry por MQTT:

`IMPLEMENTADO`

Publisher C++:

`IMPLEMENTADO`

Subscriber backend:

`IMPLEMENTADO`

Validação:

`IMPLEMENTADA`

Persistência:

`IMPLEMENTADA`

Reconnect backend:

`IMPLEMENTADO`

Reconnect mínimo do simulador:

`IMPLEMENTADO`

Build C++ no CI:

`IMPLEMENTADO`

Event Window por MQTT:

`PLANEJADA PARA SPRINT 3`

Status topic:

`FUTURO`

Segurança MQTT de produção:

`FUTURO`

Fluxo consolidado atual:

`C++ Simulator → MQTT → ASP.NET Core → SQLite → React`