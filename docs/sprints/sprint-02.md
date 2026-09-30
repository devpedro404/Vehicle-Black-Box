# Vehicle Black Box — Sprint 02

> Status: **CONCLUÍDA**
> Tema: Integração MQTT e confiabilidade do transporte
> Base anterior: Sprint 1 concluída
> Fluxo consolidado: `C++ Simulator → MQTT → ASP.NET Core → SQLite → React`

---

## 1. Objetivo da Sprint 2

A Sprint 2 teve como objetivo fechar o primeiro fluxo real de comunicação entre o simulador C++ e o backend usando MQTT, preservando o contrato de Telemetry já validado na Sprint 1.

Ao final da Sprint 2, o fluxo funcional passou a ser:

C++ Simulator
↓ publica JSON
MQTT Broker
↓ subscribe
ASP.NET Core
↓ valida e persiste
SQLite
↓ consulta pela API
React Dashboard

A Sprint 2 não alterou o princípio arquitetural do projeto:

- o simulador/Core produz dados;
- MQTT é utilizado como transporte;
- o backend valida e persiste;
- SQLite permanece como banco atual;
- o React continua consumindo a API HTTP;
- o frontend não acessa diretamente o broker MQTT.

---

## 2. Estado recebido da Sprint 1

Antes da Sprint 2, o projeto já possuía dois fluxos principais funcionando.

### Events

Já estavam concluídos:

- entidade `Event`;
- persistência em SQLite;
- POST de Events;
- consulta de Events;
- histórico no React;
- detalhe de evento;
- testes automatizados.

### Telemetry

Já estavam concluídos:

- entidade `Vehicle`;
- entidade `Telemetry`;
- `VehicleBlackBoxContext`;
- SQLite;
- `POST /api/telemetry`;
- `GET /api/vehicles`;
- `GET /api/vehicles/{id}`;
- `GET /api/vehicles/{id}/telemetry/latest`;
- dashboard React;
- testes automatizados.

A Sprint 2 não recriou essas funcionalidades. Ela adicionou MQTT ao fluxo existente.

---

## 3. Decisões técnicas

### Broker de desenvolvimento

Broker utilizado:

`Eclipse Mosquitto`

Configuração padrão de desenvolvimento:

Host: localhost
Port: 1883

Docker não foi tornado obrigatório.

### Topic de Telemetry

Publisher:

`vehicle/{vehicleId}/telemetry`

Exemplo:

`vehicle/CAR-001/telemetry`

O backend assina:

`vehicle/+/telemetry`

### Payload

O MQTT preserva o contrato já existente de `Telemetry`.

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

Não foi criado um segundo contrato apenas para MQTT.

---

## 4. Configuração MQTT

No backend, a configuração MQTT foi retirada da regra de negócio e colocada em configuração externa.

Exemplo atual:

"Mqtt": {
  "Host": "localhost",
  "Port": 1883,
  "ClientId": "vehicle-black-box-backend",
  "Topic": "vehicle/+/telemetry"
}

O backend utiliza:

`IOptions<MqttOptions>`

para acessar essas configurações.

---

## 5. Simulador C++ — MQTT Publisher

A Sprint 2 adicionou MQTT ao simulador localizado em:

`embedded/simulator`

O simulador passou a:

- conectar ao broker Mosquitto;
- gerar Telemetry;
- serializar Telemetry em JSON;
- publicar no topic correspondente ao veículo;
- usar QoS 0;
- usar `retain = false`;
- detectar perda de conexão;
- tentar reconectar;
- continuar a simulação mesmo quando uma publicação não puder ocorrer.

---

## 6. Serialização JSON no C++

A serialização preserva os campos do contrato de Telemetry.

O simulador atualmente publica:

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

A biblioteca cJSON foi incorporada ao simulador para realizar a serialização.

---

## 7. Topic gerado pelo simulador

O simulador monta o topic através do `vehicleId`.

Exemplo:

`vehicle/CAR-001/telemetry`

Regra:

`vehicle/{vehicleId}/telemetry`

Isso permite que diferentes veículos utilizem o mesmo padrão de transporte.

---

## 8. Backend — cliente MQTT

Foi criado no backend:

`MqttTelemetryService`

O serviço roda como:

`BackgroundService`

e é registrado no ASP.NET Core como hosted service.

Isso permite que a API HTTP continue funcionando independentemente do loop MQTT.

---

## 9. Recepção MQTT

Quando o backend conecta ao broker, ele assina:

`vehicle/+/telemetry`

Ao receber uma mensagem:

1. captura o topic;
2. captura o payload;
3. valida o JSON;
4. valida os campos obrigatórios;
5. valida o `vehicleId`;
6. confere o `vehicleId` do topic contra o payload;
7. abre um scope de serviços;
8. utiliza o `VehicleBlackBoxContext`;
9. cria o Vehicle quando necessário;
10. salva a Telemetry;
11. executa `SaveChangesAsync`.

---

## 10. Validação MQTT

A validação foi extraída para:

`TelemetryMqttValidator`

Responsabilidades:

- tentar desserializar o JSON;
- rejeitar JSON inválido;
- rejeitar payload vazio;
- rejeitar `vehicleId` ausente;
- rejeitar `deviceId` ausente;
- validar o formato do topic;
- comparar o veículo do topic com o veículo do payload.

---

## 11. Topic e payload divergentes

Exemplo inválido:

Topic:

`vehicle/CAR-002/telemetry`

Payload:

{
  "vehicleId": "CAR-001"
}

Esse caso é rejeitado.

Mensagem de validação implementada:

`VehicleId do tópico difere do payload.`

Assim, telemetria de um veículo não é gravada acidentalmente como pertencente a outro.

---

## 12. Tratamento de JSON inválido

JSON inválido não pode interromper o serviço MQTT.

Comportamento implementado:

mensagem inválida
↓
validator rejeita
↓
warning no log
↓
mensagem descartada
↓
backend continua funcionando

A mensagem inválida não é persistida.

---

## 13. Persistência

A Sprint 2 manteve o contexto único já criado anteriormente:

`VehicleBlackBoxContext`

Não foi criado outro `DbContext`.

A Telemetry recebida por MQTT utiliza as mesmas estruturas de persistência da Telemetry já existente no projeto.

Isso preserva a arquitetura:

HTTP Telemetry ─┐
                 ├→ VehicleBlackBoxContext → SQLite
MQTT Telemetry ─┘

---

## 14. Criação de Vehicle

Quando uma mensagem MQTT válida chega, o backend verifica se o veículo já existe.

Se o `VehicleId` ainda não estiver registrado, o backend cria:

`Vehicle`

antes de persistir a Telemetry.

Essa é a mesma linha arquitetural usada pelo fluxo existente de telemetria.

---

## 15. API latest

A Sprint 2 preservou o endpoint:

`GET /api/vehicles/{id}/telemetry/latest`

Exemplo:

`GET /api/vehicles/CAR-001/telemetry/latest`

A API procura a Telemetry mais recente através de:

Timestamp DESC
Id DESC

Assim, a telemetria recebida via MQTT pode ser consultada pelo mesmo endpoint que o frontend já utiliza.

---

## 16. React

O React não consome MQTT diretamente.

Fluxo mantido:

MQTT
↓
Backend
↓
SQLite
↓
API HTTP
↓
React

Isso mantém responsabilidades separadas e evita acoplamento do frontend ao transporte MQTT.

---

## 17. Dashboard da Sprint 2

Ao final da Sprint 2, o dashboard foi também refinado visualmente.

O dashboard passou a apresentar de maneira automotiva:

- velocidade;
- RPM;
- temperatura do motor;
- aceleração longitudinal;
- aceleração lateral;
- coordenadas GPS;
- veículo monitorado;
- dispositivo;
- estado dos dados;
- indicação da integração MQTT;
- horário da última atualização.

Também foram adicionados:

- gauges;
- visualização central do veículo;
- layout responsivo;
- estados de carregamento;
- estados de erro;
- acesso ao histórico de eventos.

O dashboard continua consumindo a API real.

---

## 18. Reconexão do backend

O backend não depende de o broker estar ativo no momento em que a aplicação sobe.

Foi implementado loop de reconexão.

Quando desconectado:

Backend
↓
tenta conectar
↓
falha
↓
aguarda aproximadamente 5 segundos
↓
tenta novamente

Quando a conexão volta:

- o backend conecta;
- reinscreve-se no topic;
- continua recebendo Telemetry.

---

## 19. Reconexão do simulador

O simulador possui lógica mínima de reconnect.

Quando está conectado:

`publish`

Quando perde conexão:

`ensureConnected()`

tenta restabelecer a conexão antes da próxima publicação.

Se ainda não houver conexão, a publicação daquele ciclo é pulada sem derrubar a aplicação.

---

## 20. Testes

Antes das validações MQTT, o backend possuía:

`13 testes`

Depois da Sprint 2:

`17 testes`

Foram adicionados testes para:

- payload MQTT válido;
- JSON inválido;
- `deviceId` ausente;
- divergência de `vehicleId` entre topic e payload.

Resultado registrado:

`17/17 testes passando`

---

## 21. CI

O GitHub Actions já verificava:

### Backend

- `dotnet restore`
- `dotnet test`

### Frontend

- `npm ci`
- `npm run lint`
- `npm run build`

Na Sprint 2 foi incluído também o simulador C++.

Novo quality gate:

`C++ Simulator Build`

Etapas:

- instalar `libmosquitto-dev`;
- executar CMake configure;
- executar CMake build.

Com isso, alterações que quebram a compilação C++ passam a ser detectadas automaticamente pelo CI.

---

## 22. Pull Requests da Sprint 2

### PR #7 — MQTT Simulator

Branch:

`feature/mqtt-simulator`

Entregou:

- MQTT no simulador C++;
- cJSON;
- serialização de Telemetry;
- publicação no topic;
- reconnect mínimo;
- build C++ no CI.

Merge commit:

`2df8f249c410afa18975562fca426b736fe2acbc`

### PR #8 — MQTT Backend

Branch:

`feature/mqtt-backend`

Entregou:

- cliente MQTT no ASP.NET Core;
- subscribe;
- recepção de Telemetry;
- validação;
- persistência SQLite;
- criação de Vehicle quando necessário;
- tratamento de mensagem inválida.

Merge commit:

`09adfe5866ec2eeb37466fe1393e78b7f49b73be`

### PR #9 — MQTT Config

Branch:

`feature/mqtt-config`

Entregou:

- `MqttOptions`;
- configuração MQTT externa;
- integração via `IOptions`;
- remoção de host, topic e clientId hardcoded do serviço backend.

Merge commit:

`98caac44de64caa91133e815cd2ed674de544a82`

### PR #10 — MQTT Reconnect

Branch:

`feature/mqtt-reconnect`

Entregou:

- tratamento explícito de desconexão;
- reconexão contínua;
- backend tolerante a broker indisponível;
- nova inscrição após reconectar.

Merge commit:

`408fc95d254aed9c969331a1728162272497d24c`

### PR #11 — MQTT Tests

Branch:

`feature/mqtt-tests`

Entregou:

- extração do `TelemetryMqttValidator`;
- novos testes MQTT;
- validação de JSON;
- validação de IDs;
- total de 17 testes backend.

Merge commit:

`4beebc81db28416920f988ac4c4834c053a273ea`

### PR #12 — Dashboard automotivo

Branch:

`feature/dashboard-automotive`

Entregou:

- redesign do dashboard;
- gauges;
- visualização automotiva;
- melhorias de temperatura;
- aceleração;
- GPS;
- responsividade desktop, tablet e mobile;
- manutenção do consumo da API real.

Merge commit:

`a3d602e0647cedb1f0d6afcb15d44dddae75ad68`

---

## 23. Cenário E2E validado

Fluxo final:

Simulator C++
↓
Telemetry JSON
↓
MQTT / Mosquitto
↓
MqttTelemetryService
↓
TelemetryMqttValidator
↓
VehicleBlackBoxContext
↓
SQLite
↓
GET /api/vehicles/{id}/telemetry/latest
↓
React Dashboard

Cenário principal utilizado:

`CAR-001`

O simulador publica os dados, o backend os recebe, valida e persiste, e a API latest torna o último registro disponível ao dashboard.

---

## 24. Cenários de validação

### Cenário A — Telemetria nominal

Broker ativo
→ simulador publica
→ backend recebe
→ SQLite persiste
→ latest API retorna
→ dashboard exibe

Resultado:

`OK`

### Cenário B — Broker indisponível

broker OFF
→ backend continua ativo
→ broker volta
→ backend reconecta

Resultado:

`OK`

### Cenário C — JSON inválido

JSON inválido
→ rejeitado
→ não persiste
→ serviço continua ativo

Resultado:

`OK`

### Cenário D — Payload incompleto

Campos obrigatórios como:

- `vehicleId`;
- `deviceId`;

são validados.

Resultado:

`payload inválido é rejeitado`

### Cenário E — Topic divergente

Exemplo:

topic: CAR-002
payload: CAR-001

Resultado:

`rejeitado`

### Cenário F — Regressão

Os testes existentes continuaram funcionando e novos testes MQTT foram adicionados.

Resultado final:

- 17 testes backend passando;
- frontend lint OK;
- frontend build OK;
- C++ build integrado ao CI.

---

## 25. Definition of Done

Ao final da Sprint 2:

- [x] simulador publica Telemetry por MQTT;
- [x] JSON utiliza o contrato de Telemetry existente;
- [x] topic segue `vehicle/{vehicleId}/telemetry`;
- [x] backend assina `vehicle/+/telemetry`;
- [x] backend recebe MQTT sem endpoint HTTP intermediário;
- [x] payload é validado;
- [x] topic e payload são comparados;
- [x] mensagem válida é persistida;
- [x] mensagem inválida é descartada;
- [x] JSON inválido não derruba o serviço;
- [x] backend tenta reconectar ao broker;
- [x] simulador possui reconnect mínimo;
- [x] Vehicle pode ser criado quando necessário;
- [x] SQLite continua sendo o banco;
- [x] API latest continua funcionando;
- [x] React continua consumindo API HTTP;
- [x] backend possui testes MQTT;
- [x] 17 testes backend passam;
- [x] frontend lint passa;
- [x] frontend build passa;
- [x] simulador C++ entrou no CI;
- [x] dashboard automotivo concluído;
- [x] PRs da Sprint 2 foram mergeados na `main`.

---

## 26. Fora do escopo da Sprint 2

Não foram objetivos desta Sprint:

- hardware real;
- Raspberry Pi;
- CAN;
- OBD-II;
- GPS físico;
- acelerômetro físico;
- PostgreSQL;
- autenticação MQTT de produção;
- TLS MQTT;
- QoS avançado;
- idempotência completa;
- WebSocket;
- SignalR;
- streaming MQTT direto para React;
- mapa real;
- novos eventos automotivos;
- Event Window;
- buffer circular;
- janela pré-evento;
- janela pós-evento.

Esses itens permanecem para evoluções futuras.

---

## 27. Pontos que seguem para evolução futura

A conclusão da Sprint 2 não elimina algumas melhorias arquiteturais já identificadas:

- revisar estratégia geral de DTOs;
- revisar semântica UTC no SQLite;
- tornar CORS configurável;
- mover outras configurações de ambiente quando necessário;
- fortalecer relacionamento `Vehicle → Telemetry`;
- revisar a política de criação automática de Vehicle;
- evoluir segurança MQTT para cenários reais;
- implementar TLS quando houver ambiente real;
- considerar QoS e idempotência mais fortes quando necessário;
- adicionar testes E2E totalmente automatizados em etapa futura.

Esses pontos não invalidam a conclusão da Sprint 2.

---

## 28. Resultado final da Sprint 2

A Sprint 2 transformou a comunicação do projeto.

Antes:

C++ Simulator
→ ASP.NET Core
→ SQLite
→ React

Depois:

C++ Simulator
→ MQTT
→ ASP.NET Core
→ SQLite
→ React

O resultado principal é que o projeto deixou de depender de comunicação direta entre simulador e backend e passou a utilizar uma camada de mensageria compatível com a evolução futura para dispositivos embarcados reais.

---

## 29. Checkpoint final

Estado ao encerramento da Sprint 2:

- Sprint 1 preservada;
- fluxo de Telemetry preservado;
- fluxo de Events preservado;
- MQTT integrado ao simulador;
- MQTT integrado ao backend;
- persistência funcionando;
- API latest funcionando;
- dashboard funcionando;
- reconnect implementado;
- validação MQTT implementada;
- testes ampliados;
- CI ampliado para C++;
- dashboard finalizado;
- PRs #7, #8, #9, #10, #11 e #12 mergeados.

A partir deste checkpoint, a `main` deve ser considerada a fonte de verdade.

---

## 30. Próxima etapa

A próxima evolução planejada é a Sprint 3.

Direção principal:

- Event Window;
- amostras relacionadas a eventos;
- contexto antes e depois da ocorrência;
- persistência da janela;
- API de consulta da janela;
- gráfico temporal real;
- marcação visual de `t=0`;
- integração com o Core de eventos.

A Sprint 3 deve partir da `main` atualizada após o encerramento documental da Sprint 2.

---

## 31. Resumo executivo

A Sprint 2 está concluída.

Fluxo final validado:

`C++ Simulator → MQTT → ASP.NET Core → SQLite → React`

Principais entregas:

- MQTT Publisher no C++;
- MQTT Subscriber no backend;
- contrato de Telemetry preservado;
- validação de payload;
- validação de topic;
- persistência em SQLite;
- reconnect;
- configuração MQTT;
- testes automatizados;
- CI para C++;
- dashboard automotivo;
- integração ponta a ponta.

Status final:

**SPRINT 02 — CONCLUÍDA**