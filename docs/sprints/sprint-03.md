# Vehicle Black Box — Sprint 03

> Status: **PLANEJADA / PRONTA PARA EXECUÇÃO**
> Tema: Event Window, persistência de contexto e visualização temporal
> Responsáveis principais: Pedro e Endy
> Base: Sprint 02 concluída
> Fluxo atual: `C++ Simulator → MQTT → ASP.NET Core → SQLite → React`
> Objetivo da Sprint 03: persistir e visualizar o contexto antes e depois de um evento.

---

## 1. Contexto da Sprint 3

A Sprint 02 concluiu o transporte de telemetria via MQTT e deixou funcionando o fluxo:

C++ Simulator
→ MQTT
→ ASP.NET Core
→ SQLite
→ React

A Sprint 03 parte desse fluxo já funcional.

O objetivo agora não é apenas receber uma telemetria isolada.

A Sprint 03 introduz o conceito de:

`Event Window`

Uma Event Window representa um conjunto de amostras de telemetria relacionadas a um evento, permitindo observar o que o veículo estava fazendo:

- antes do evento;
- no instante do evento;
- depois do evento.

Exemplo conceitual:

-5s
-4s
-3s
-2s
-1s
0
+1s
+2s
+3s
+4s
+5s

Onde:

`t=0`

representa o instante exato da ocorrência.

---

## 2. Estado atual do projeto

A Sprint 02 fechou o transporte de telemetria via MQTT.

Fluxo atual:

C++ Simulator
→ MQTT
→ ASP.NET Core
→ SQLite
→ React

Já concluído antes da Sprint 03:

- telemetria simulada;
- detecção inicial de `HARD_BRAKING`;
- MQTT no simulador;
- MQTT no backend;
- reconexão automática;
- validação de payload;
- persistência em SQLite;
- API de Telemetry;
- dashboard automotivo responsivo;
- histórico de eventos;
- detalhe de eventos;
- CI com backend;
- CI com frontend;
- CI com simulador C++.

---

## 3. O que a Sprint 3 adiciona

A Sprint 03 adicionará:

- amostras relacionadas a um evento;
- persistência da Event Window;
- endpoint para consultar a janela;
- validação do payload de evento;
- gráfico temporal real;
- marcação visual de `t=0`;
- testes backend para Event Window;
- integração ponta a ponta com o Core desenvolvido pelo Pedro.

O resultado esperado é que um evento deixe de ser apenas um registro pontual e passe a possuir contexto temporal.

---

## 4. Divisão principal de responsabilidades

### Pedro — Core / Event Window

Responsável principalmente por:

- buffer circular;
- captura das amostras anteriores ao evento;
- captura das amostras posteriores ao evento;
- regra do evento;
- geração da Event Window;
- serialização da Event Window;
- publicação do payload via MQTT.

### Endy — Backend / Persistência / API / Frontend

Responsável principalmente por:

- receber Event Window;
- validar payload;
- persistir evento e amostras;
- criar relacionamento entre Event e Samples;
- disponibilizar API de consulta;
- construir visualização temporal no React;
- criar testes backend;
- validar integração ponta a ponta.

Fluxo:

Pedro: Event Window JSON
→ MQTT
→ Endy: Validator
→ Persistence
→ API
→ React

---

## 5. Escopo da trilha da Endy

A trilha da Endy está dividida nos seguintes blocos:

### G3.1 — Modelo de amostra de evento

Criar entidade:

`EventTelemetry`

ou:

`EventSample`

A entidade deve estar ligada ao `Event`.

---

### G3.2 — Migration e relacionamento

Criar relacionamento:

`Event 1 → N EventTelemetry`

dentro do:

`VehicleBlackBoxContext`

---

### G3.3 — Recepção MQTT

Ampliar a infraestrutura MQTT existente para:

- assinar topic de eventos;
- receber Event Window;
- validar payload.

---

### G3.4 — Persistência atômica

Persistir:

`Event + Samples`

como uma única unidade lógica.

---

### G3.5 — API

Criar endpoint:

`GET /api/events/{id}/telemetry`

---

### G3.6 — Frontend

Criar gráfico temporal real dentro do detalhe do evento.

---

### G3.7 — Testes

Adicionar testes para:

- payload válido;
- payload inválido;
- persistência;
- relacionamento;
- consulta;
- 404;
- ordenação.

---

### G3.8 — Integração E2E

Validar o payload real produzido pelo Core do Pedro desde o simulador até a tela.

---

## 6. Fora do escopo da trilha da Endy

Não faz parte desta trilha implementar:

- buffer circular;
- regras de detecção no C++;
- CAN;
- OBD-II;
- hardware real;
- PostgreSQL;
- streaming realtime para frontend.

Essas responsabilidades não devem ser incorporadas ao backend/frontend nesta Sprint.

---

## 7. Dependência com a trilha do Pedro

A Endy consegue adiantar:

- modelo;
- banco;
- endpoint;
- frontend;
- testes;

utilizando payload de exemplo.

Porém, a integração real depende do contrato definitivo da Event Window produzido pelo Pedro.

Antes da integração final, precisam estar congelados:

- topic;
- estrutura JSON;
- campos obrigatórios;
- unidade de `relativeTime`;
- formato do timestamp;
- comportamento de janela parcial;
- definição do instante do evento.

Regra:

**não integrar no escuro.**

Antes do PR de recepção MQTT, o contrato deve estar documentado.

---

## 8. Contrato proposto da Event Window

Exemplo de payload:

{
  "vehicleId": "CAR-001",
  "deviceId": "VBB-001",
  "eventType": "HARD_BRAKING",
  "eventTimestamp": "2026-09-30T15:40:10Z",
  "samples": [
    {
      "timestamp": "2026-09-30T15:40:05Z",
      "relativeTimeMs": -5000,
      "speed": 82,
      "rpm": 3100,
      "engineTemperature": 89,
      "longitudinalAcceleration": -0.4,
      "lateralAcceleration": 0.1,
      "latitude": -3.119,
      "longitude": -60.0217
    }
  ]
}

O contrato definitivo deve ser alinhado com o payload realmente produzido pelo Core.

---

## 9. Campos principais da Event Window

Nível principal:

- `vehicleId`;
- `deviceId`;
- `eventType`;
- `eventTimestamp`;
- `samples`.

Cada item de `samples` deve conter os dados temporais da telemetria.

Campos esperados:

- `timestamp`;
- `relativeTimeMs`;
- `speed`;
- `rpm`;
- `engineTemperature`;
- `longitudinalAcceleration`;
- `lateralAcceleration`;
- `latitude`;
- `longitude`.

---

## 10. Conceito de relativeTimeMs

O campo:

`relativeTimeMs`

representa a distância temporal da amostra em relação ao instante do evento.

Exemplo:

`-5000`

significa:

5 segundos antes do evento.

`0`

significa:

instante do evento.

`3000`

significa:

3 segundos depois do evento.

Esse campo permitirá ao frontend construir a timeline sem depender exclusivamente dos timestamps absolutos.

---

## 11. G3.1 — Entidade de amostra do evento

O backend precisa armazenar cada ponto da Event Window como uma entidade relacionada ao evento.

Nome recomendado:

`EventTelemetry`

Estrutura conceitual:

EventTelemetry

- Id
- EventId
- Timestamp
- RelativeTimeMs
- Speed
- Rpm
- EngineTemperature
- LongitudinalAcceleration
- LateralAcceleration
- Latitude
- Longitude

---

## 12. Regras do modelo

Regras obrigatórias:

- utilizar o `VehicleBlackBoxContext` existente;
- não criar outro `DbContext`;
- `EventId` deve ser uma FK real;
- Samples devem pertencer a um único Event;
- ordenação deve utilizar `Timestamp` ou `RelativeTimeMs`;
- o frontend não deve depender da ordem acidental do banco.

Escolher um único nome para a entidade e utilizá-lo em:

- model;
- migration;
- DbContext;
- DTO;
- API;
- testes;
- documentação.

Recomendação:

`EventTelemetry`

---

## 13. G3.2 — Migration, FK e integridade

Relacionamento:

`Event 1 → N EventTelemetry`

A migration deve criar:

- tabela de amostras;
- primary key;
- foreign key para Event;
- índice por `EventId`;
- campos numéricos coerentes com Telemetry;
- política de delete documentada.

---

## 14. Migration incremental

Não apagar ou recriar o banco sem necessidade.

A Sprint 03 deve preservar migrations existentes.

Criar uma nova migration incremental.

Validação recomendada:

`dotnet ef database update`

Depois:

`dotnet test backend/VehicleBlackBox.Api.Tests/VehicleBlackBox.Api.Tests.csproj`

---

## 15. VehicleBlackBoxContext

O projeto deve continuar utilizando apenas:

`VehicleBlackBoxContext`

Não criar:

- `EventContext`;
- `TelemetryContext`;
- `EventWindowContext`;
- qualquer segundo DbContext.

O novo DbSet deve ser incorporado ao contexto existente.

Exemplo conceitual:

`DbSet<EventTelemetry>`

---

## 16. G3.3 — Recepção MQTT da Event Window

O backend já possui infraestrutura MQTT.

A Sprint 03 deve ampliar essa infraestrutura sem quebrar o topic de telemetria existente.

O fluxo existente:

`vehicle/{vehicleId}/telemetry`

deve continuar funcionando.

Topic recomendado para eventos:

`vehicle/{vehicleId}/events`

Exemplo:

`vehicle/CAR-001/events`

---

## 17. Validação mínima da Event Window

A recepção MQTT deve verificar:

### JSON válido

Se falhar:

- logar;
- descartar;
- manter serviço ativo.

### vehicleId presente

Se ausente:

- descartar.

### deviceId presente

Se ausente:

- descartar.

### eventType suportado

Se inválido:

- descartar ou rejeitar explicitamente.

### eventTimestamp válido

Se inválido:

- descartar.

### samples não vazio

Se vazio:

- rejeitar conforme contrato definido.

### vehicleId do topic = payload

Se diferente:

- rejeitar.

### timestamps coerentes

Se a janela estiver inconsistente:

- rejeitar.

---

## 18. Regra de resiliência

Uma mensagem de Event Window ruim nunca pode derrubar:

`BackgroundService MQTT`

O comportamento esperado é:

payload inválido
→ validator rejeita
→ log
→ mensagem descartada
→ serviço continua vivo

---

## 19. EventWindow Validator

A validação da Event Window deve preferencialmente ficar isolada da lógica de persistência.

Estrutura conceitual:

MQTT Message
↓
EventWindowValidator
↓
válido?
↓ sim
Persistence

Isso mantém separadas as responsabilidades de:

- transporte;
- validação;
- persistência.

---

## 20. G3.4 — Persistência atômica

Uma Event Window deve ser tratada como uma unidade lógica.

Fluxo:

Validate
→ Create/Resolve Event
→ Add Samples
→ SaveChanges

Evento e amostras devem ser persistidos de forma consistente.

---

## 21. Comportamento esperado da persistência

O backend não deve:

- salvar Samples sem Event;
- deixar uma janela parcialmente salva por erro de parsing;
- derrubar o serviço por falha de persistência;
- duplicar registros por erro interno de processamento.

O backend deve:

- logar falha de persistência;
- manter aplicação viva;
- preservar consistência;
- reduzir duplicações acidentais.

---

## 22. Deduplicação

Deduplicação sofisticada não é requisito obrigatório desta Sprint.

Pode ficar para Sprint futura.

Porém, deve-se evitar duplicação causada pelo próprio fluxo interno.

Exemplo:

a mesma mensagem não deve ser persistida duas vezes simplesmente porque um handler interno foi executado duas vezes por engano.

---

## 23. Relação Event + Samples

Estrutura esperada:

Event

- Id
- VehicleId
- EventType
- Timestamp
- ...

↓

EventTelemetry[]

- EventId
- Timestamp
- RelativeTimeMs
- Speed
- RPM
- etc.

Uma Event Window representa:

`1 Event + N Samples`

---

## 24. G3.5 — Endpoint da janela

Endpoint principal:

`GET /api/events/{id}/telemetry`

Objetivo:

retornar as amostras relacionadas a um evento.

---

## 25. Comportamento do endpoint

### Evento existe com Samples

Status:

`200`

Resultado:

lista ordenada.

### Evento existe sem Samples

Status:

`200`

Resultado:

lista vazia ou comportamento documentado.

### Evento inexistente

Status:

`404`

### ID inválido

Seguir a convenção atual da API.

Possibilidades:

- 400;
- 404.

Não criar comportamento inconsistente apenas para este endpoint.

---

## 26. DTO recomendado

Exemplo conceitual:

{
  "eventId": 42,
  "eventType": "HARD_BRAKING",
  "eventTimestamp": "2026-09-30T15:40:10Z",
  "samples": [
    ...
  ]
}

Este endpoint é um bom ponto para utilizar um DTO específico sem precisar refatorar toda a API para DTOs nesta Sprint.

---

## 27. Ordenação da API

As Samples devem ser retornadas em ordem temporal.

Preferência:

`RelativeTimeMs ASC`

ou:

`Timestamp ASC`

Exemplo:

-5000
-4000
-3000
-2000
-1000
0
1000
2000
3000
4000
5000

O frontend não deve precisar ordenar uma resposta inconsistente.

---

## 28. G3.6 — Frontend temporal

O frontend já possui rota:

`/events/:id`

A Sprint 03 deve transformar o detalhe do evento em uma visualização temporal real.

Não redesenhar o dashboard inteiro novamente.

O foco é:

**detalhe temporal do evento.**

---

## 29. Timeline esperada

Exemplo:

HARD_BRAKING — Event #42

-5s  -4s  -3s  -2s  -1s   0   +1s  +2s  +3s  +4s  +5s

--------------------------|-----------------------------

                          EVENTO

Velocidade -----------\____

Aceleração -----------\_/--

---

## 30. Requisitos obrigatórios do gráfico

O gráfico precisa:

- utilizar eixo X baseado em tempo relativo;
- marcar claramente `t=0`;
- mostrar velocidade;
- mostrar aceleração longitudinal;
- utilizar dados reais da API;
- funcionar em desktop;
- funcionar em mobile.

---

## 31. Marcação de t=0

O instante do evento precisa ser visualmente evidente.

Exemplo:

`0s`

ou:

`t=0`

ou uma linha vertical identificada como:

`EVENTO`

O usuário precisa conseguir perceber imediatamente:

- o que aconteceu antes;
- o que aconteceu no instante;
- o que aconteceu depois.

---

## 32. Estados de tela

O frontend deve tratar:

### Loading

Enquanto consulta a API.

### Erro

Quando API falhar.

### Sem Samples

Quando Event existir, mas ainda não possuir amostras.

### Sucesso

Quando Event Window estiver disponível.

Nenhum desses estados deve quebrar a tela.

---

## 33. Não exagerar no frontend

A Sprint 03 não deve virar outro redesign geral.

Não alterar sem necessidade:

- dashboard principal;
- navegação;
- identidade visual;
- estrutura global;
- outras páginas.

O foco é apenas:

`Event Detail + Timeline`

---

## 34. G3.7 — Testes backend obrigatórios

A Sprint 03 deve adicionar testes específicos da Event Window.

---

## 35. Teste — Payload válido

Entrada:

Event Window válida.

Esperado:

`aceito`

---

## 36. Teste — JSON inválido

Entrada:

JSON quebrado.

Esperado:

- rejeitado;
- sem crash;
- sem persistência inválida.

---

## 37. Teste — vehicleId ausente

Esperado:

`rejeitado`

---

## 38. Teste — deviceId ausente

Esperado:

`rejeitado`

---

## 39. Teste — samples vazio

Esperado:

`rejeitado`

ou comportamento explicitamente definido pelo contrato.

---

## 40. Teste — topic/payload divergentes

Exemplo:

Topic:

`vehicle/CAR-002/events`

Payload:

`CAR-001`

Esperado:

`rejeitado`

---

## 41. Teste — Event + Samples persistidos

Validar:

- Event criado/resolvido;
- quantidade correta de Samples;
- todas as Samples possuem FK correta;
- relacionamento Event → Samples funciona.

---

## 42. Teste — GET janela existente

Endpoint:

`GET /api/events/{id}/telemetry`

Esperado:

`200`

Validar:

- quantidade;
- conteúdo;
- ordenação temporal.

---

## 43. Teste — Evento inexistente

Endpoint com ID inexistente.

Esperado:

`404`

---

## 44. Teste — Janela parcial

Caso o Core permita uma janela parcial, o backend deve possuir comportamento previsível e documentado.

Exemplo:

evento aconteceu, mas nem todas as amostras pós-evento foram capturadas.

A decisão deve ser explícita:

- aceitar;
ou
- rejeitar.

Não deixar esse comportamento acidental.

---

## 45. Quality Gate local

Antes de abrir PR:

Backend:

`dotnet test backend/VehicleBlackBox.Api.Tests/VehicleBlackBox.Api.Tests.csproj`

Frontend:

`npm --prefix frontend run lint`

Frontend build:

`npm --prefix frontend run build`

Se necessário:

C++:

`cmake --build embedded/simulator/build`

---

## 46. G3.8 — Integração ponta a ponta

Quando o Pedro disponibilizar o payload real, validar:

Simulator
→ Event Window
→ MQTT
→ Backend
→ SQLite
→ API
→ React

---

## 47. Cenário E2E principal

1. `CAR-001` trafega normalmente.
2. Telemetria é gerada continuamente.
3. Ocorre `HARD_BRAKING`.
4. Core detecta o evento.
5. Core preserva amostras anteriores ao evento.
6. Core captura amostras posteriores.
7. Event Window é construída.
8. Event Window é publicada via MQTT.
9. Backend recebe.
10. Backend valida.
11. Event é persistido.
12. Samples são persistidas.
13. API retorna a janela.
14. Frontend abre `/events/{id}`.
15. Gráfico mostra antes, `t=0` e depois.

---

## 48. Evidências da integração

Guardar como evidência:

- log do simulador;
- log MQTT;
- log do backend;
- payload real;
- resposta da API;
- quantidade de Samples persistidas;
- print do gráfico temporal;
- CI verde.

---

## 49. Ordem recomendada dos PRs da Endy

A trilha da Endy deve evitar um PR gigante.

Ordem recomendada:

### PR 1

Branch:

`feature/event-samples`

Conteúdo:

- entidade;
- relacionamento;
- migration;
- testes básicos.

---

### PR 2

Branch:

`feature/event-window-mqtt`

Conteúdo:

- validator;
- recepção MQTT;
- persistência.

---

### PR 3

Branch:

`feature/event-window-api`

Conteúdo:

- endpoint;
- DTO;
- testes de API.

---

### PR 4

Branch:

`feature/event-timeline-ui`

Conteúdo:

- service React;
- consumo da API;
- gráfico temporal;
- estados da tela.

---

## 50. Regra de criação de branch

Cada branch nova deve nascer da `main` atualizada depois do merge do PR anterior quando houver dependência.

Antes de cada branch:

`git switch main`

`git fetch --prune origin`

`git pull --ff-only origin main`

`git status`

Depois:

`git switch -c feature/nome-da-feature`

---

## 51. Regras de repositório

Não esquecer:

- [ ] nunca trabalhar diretamente na `main`;
- [ ] atualizar `main` antes de criar branch;
- [ ] uma responsabilidade principal por PR;
- [ ] rodar testes antes do push;
- [ ] conferir `git status` antes de commit;
- [ ] não usar force push na `main`;
- [ ] não reutilizar branch antiga de outra Sprint;
- [ ] não alterar código do Pedro sem alinhamento;
- [ ] manter `VehicleBlackBoxContext` único;
- [ ] preservar topic de telemetria existente;
- [ ] atualizar documentação quando contrato mudar;
- [ ] anexar evidências no PR;
- [ ] esperar CI verde antes do merge;
- [ ] depois do merge, voltar para `main` e atualizar novamente.

---

## 52. Template recomendado de PR

### Resumo

- o que foi implementado.

### Validação

- testes backend;
- lint frontend;
- build frontend;
- cenário manual ou E2E quando aplicável.

### Evidências

- endpoints;
- logs;
- prints;
- payloads;
- resultados relevantes.

---

## 53. Pontos de sincronização com Pedro

A integração possui momentos específicos de sincronização.

### Contrato

Pedro entrega:

- topic;
- JSON final.

Endy confirma:

- validator aceita;
- DTO representa corretamente;
- backend consegue persistir.

---

### Primeiro payload real

Pedro publica uma Event Window real.

Endy confirma:

- backend recebeu;
- logou;
- validou;
- persistiu.

---

### Janela parcial

Pedro define o comportamento do Core.

Endy implementa comportamento compatível.

---

### Cooldown

Pedro define regra para eventos próximos.

Endy confirma que o backend não duplica eventos por erro de integração.

---

### E2E

Pedro executa simulador/Core.

Endy confirma:

- API;
- SQLite;
- UI.

---

## 54. Regra de contrato

Se o payload mudar:

**atualizar primeiro a documentação.**

Não deixar:

C++

e:

C#

evoluírem com versões diferentes do JSON.

A documentação da Sprint e/ou contrato é a fonte de verdade antes da integração.

---

## 55. Definition of Done — trilha da Endy

A trilha da Endy será considerada concluída quando:

- [ ] entidade `EventTelemetry` ou `EventSample` criada;
- [ ] migration aplicada;
- [ ] FK `Event → Samples` funcionando;
- [ ] índice por `EventId` criado;
- [ ] validator de Event Window implementado;
- [ ] topic de eventos configurado;
- [ ] topic documentado;
- [ ] payload inválido não derruba backend;
- [ ] evento persistido corretamente;
- [ ] Samples persistidas corretamente;
- [ ] endpoint `GET /api/events/{id}/telemetry` implementado;
- [ ] 404 validado;
- [ ] resposta ordenada por tempo;
- [ ] frontend consome endpoint real;
- [ ] gráfico temporal utiliza dados reais;
- [ ] `t=0` aparece claramente;
- [ ] loading tratado;
- [ ] erro tratado;
- [ ] estado sem dados tratado;
- [ ] mobile funciona;
- [ ] desktop funciona;
- [ ] testes backend passam;
- [ ] frontend lint passa;
- [ ] frontend build passa;
- [ ] CI verde;
- [ ] E2E com payload do Pedro concluído;
- [ ] evidências adicionadas aos PRs;
- [ ] documentação atualizada.

---

## 56. Definition of Done geral da Sprint 3

A Sprint 03 estará concluída quando o sistema conseguir demonstrar, de ponta a ponta:

`Evento detectado`

→ contexto anterior preservado

→ contexto posterior preservado

→ Event Window publicada

→ backend recebe

→ backend valida

→ Event + Samples persistidos

→ API retorna janela

→ React exibe timeline real

→ `t=0` claramente identificado

→ testes e CI verdes

---

## 57. O que não fazer nesta Sprint

Não expandir escopo para:

- PostgreSQL;
- WebSocket;
- SignalR;
- login;
- autenticação;
- mapa real;
- relatórios;
- configurações gerais;
- hardware real;
- implantação de todos os novos tipos de evento.

---

## 58. Não refatorar sem necessidade

Evitar:

- trocar framework;
- criar outro DbContext;
- substituir MQTT;
- reescrever dashboard;
- mudar toda a API para DTOs de uma vez;
- migrar banco;
- alterar arquitetura global;
- iniciar hardware real.

---

## 59. Meta arquitetural da Sprint 3

A meta não é construir todas as funcionalidades futuras.

A meta é terminar:

**uma Event Window completa e observável.**

Qualquer melhoria que não ajude diretamente nisso deve ir para backlog futuro.

---

## 60. Checklist de retomada diária

Sempre que abrir o projeto depois de algumas horas ou dias:

`git switch main`

`git fetch --prune origin`

`git pull --ff-only origin main`

`git status --short`

`git log --oneline -10`

Depois conferir:

- [ ] qual PR foi mergeado por último;
- [ ] se Pedro atualizou contrato/payload;
- [ ] qual branch está sendo utilizada;
- [ ] se a branch está atrás da `main`;
- [ ] se backend compila;
- [ ] se testes atuais passam antes de novas alterações;
- [ ] se `docs/sprints/sprint-03.md` continua sendo a fonte de verdade.

---

## 61. Fonte de verdade

Se houver divergência entre:

- conversa;
- memória;
- instrução antiga;
- documentação local desatualizada;

prevalecem:

1. `main` atual;
2. documentação versionada;
3. contrato congelado da Sprint.

Não implementar com base apenas em conversa antiga.

---

## 62. Resultado técnico esperado

Ao final da Sprint 03, será possível abrir um evento e enxergar:

- como o veículo estava antes;
- o que aconteceu no instante do evento;
- como ele se comportou depois.

Exemplo:

Velocidade:

82
81
80
78
76
→ EVENTO
55
42
35

Aceleração longitudinal:

0.2
-0.4
-1.0
-3.2
-5.1
→ EVENTO
-7.2
-4.0
-1.2

A informação deixa de ser apenas:

`HARD_BRAKING aconteceu`

e passa a mostrar:

**como o evento aconteceu.**

---

## 63. Valor da Sprint 3

A Sprint 03 transforma o Event de um registro isolado em uma ocorrência observável.

Antes:

Event

- tipo;
- timestamp;
- dados pontuais.

Depois:

Event

+

Event Window

+

Samples

+

Timeline

Isso aproxima o sistema do conceito real de:

**Vehicle Black Box**

porque uma caixa-preta precisa preservar contexto, e não apenas registrar que algo aconteceu.

---

## 64. Responsabilidade do Core

A geração da janela pertence ao Core.

O Core será responsável por:

- manter buffer;
- detectar evento;
- congelar contexto;
- manter amostras antes;
- coletar amostras depois;
- montar Event Window;
- publicar Event Window.

O backend não deve tentar reconstruir retroativamente uma janela que deveria ter sido produzida no Core.

---

## 65. Responsabilidade do Backend

O backend será responsável por:

- receber Event Window;
- validar;
- garantir consistência;
- persistir;
- relacionar Samples ao Event;
- expor consulta.

O backend não deve conter a regra principal de detecção automotiva.

---

## 66. Responsabilidade do Frontend

O frontend será responsável por:

- buscar Event;
- buscar Event Window;
- organizar visualmente;
- mostrar timeline;
- marcar `t=0`;
- mostrar dados antes e depois;
- tratar loading;
- tratar erro;
- tratar ausência de Samples.

O frontend não deve detectar o evento.

---

## 67. Fluxo consolidado da Sprint 3

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
Temporal Chart

---

## 68. Evento inicial da Sprint 3

O evento principal utilizado para fechar a integração deve continuar sendo:

`HARD_BRAKING`

A Sprint não precisa implementar todos os tipos de eventos.

O objetivo é validar toda a arquitetura com pelo menos um evento funcionando de ponta a ponta.

Depois disso, novos eventos podem reutilizar a mesma infraestrutura.

---

## 69. Evolução futura

Após a Sprint 03 poderão ser consideradas evoluções como:

- novos tipos de evento;
- Event Windows com duração configurável;
- deduplicação mais sofisticada;
- cooldown avançado;
- score de severidade;
- múltiplas métricas no gráfico;
- exportação;
- relatórios;
- WebSocket;
- hardware real;
- CAN;
- OBD-II;
- armazenamento embarcado;
- sincronização posterior;
- PostgreSQL;
- observabilidade avançada.

Nenhum desses itens deve impedir o fechamento da Sprint 03.

---

## 70. Critério de sucesso

A pergunta principal para validar a Sprint 03 é:

**“Consigo abrir um evento e enxergar claramente o que o carro fazia antes, no momento e depois da ocorrência?”**

Se a resposta for sim, com:

- dados reais;
- Samples persistidas;
- timeline ordenada;
- `t=0`;
- API real;
- CI verde;

a Sprint cumpriu seu objetivo.

---

## 71. Checkpoint antes de começar

Antes de iniciar implementação:

- [ ] Sprint 02 documentada;
- [ ] `main` atualizada;
- [ ] PR #12 presente na `main`;
- [ ] backend atual compilando;
- [ ] frontend atual compilando;
- [ ] testes atuais passando;
- [ ] CI atual funcionando;
- [ ] contrato inicial da Event Window documentado;
- [ ] responsabilidades de Pedro e Endy confirmadas.

---

## 72. Ordem macro da execução

### Etapa 1

Congelar contrato da Event Window.

### Etapa 2

Criar modelo e persistência.

### Etapa 3

Criar validator e recepção MQTT.

### Etapa 4

Criar API da janela.

### Etapa 5

Criar timeline no frontend.

### Etapa 6

Integrar payload real do Pedro.

### Etapa 7

Executar E2E.

### Etapa 8

Guardar evidências.

### Etapa 9

Atualizar documentação.

### Etapa 10

Encerrar Sprint com CI verde.

---

## 73. Resumo executivo — Endy

A trilha da Endy na Sprint 03 é:

### Backend

- modelo;
- relacionamento;
- migration;
- validator;
- MQTT;
- persistência;
- API;
- testes.

### Frontend

- service;
- timeline;
- gráfico temporal;
- marcação `t=0`;
- loading;
- erro;
- estado sem dados;
- responsividade.

### Integração

- receber payload real;
- validar;
- persistir;
- consultar;
- visualizar;
- executar E2E com Pedro.

---

## 74. Resumo executivo — Pedro

A trilha do Pedro na Sprint 03 concentra:

- Core;
- buffer;
- captura pré-evento;
- captura pós-evento;
- geração da Event Window;
- serialização;
- publicação MQTT;
- contrato real utilizado pela integração.

As duas trilhas se encontram no:

`Event Window JSON`

---

## 75. Entrega final da Sprint 03

Entrega final esperada:

**Abrir um evento no React e enxergar, utilizando dados reais da Event Window, o que o veículo fazia antes, no instante e depois da ocorrência.**

Fluxo final:

`C++ Core → Event Window → MQTT → ASP.NET Core → SQLite → API → React Timeline`

---

# CHECKLIST FINAL DA SPRINT 03

## Contrato

- [ ] topic final definido;
- [ ] JSON final definido;
- [ ] `relativeTimeMs` definido;
- [ ] timestamp definido;
- [ ] comportamento de janela parcial definido.

## Core / Pedro

- [ ] buffer implementado;
- [ ] pré-evento capturado;
- [ ] pós-evento capturado;
- [ ] Event Window criada;
- [ ] JSON publicado via MQTT;
- [ ] payload real entregue para integração.

## Backend / Endy

- [ ] EventTelemetry criado;
- [ ] FK criada;
- [ ] migration aplicada;
- [ ] índice EventId criado;
- [ ] validator implementado;
- [ ] MQTT de eventos funcionando;
- [ ] Event + Samples persistidos;
- [ ] erros tratados sem crash.

## API

- [ ] `GET /api/events/{id}/telemetry`;
- [ ] evento existente retorna 200;
- [ ] evento inexistente retorna 404;
- [ ] Samples ordenadas;
- [ ] DTO documentado.

## Frontend

- [ ] endpoint consumido;
- [ ] velocidade exibida;
- [ ] aceleração longitudinal exibida;
- [ ] timeline exibida;
- [ ] `t=0` marcado;
- [ ] loading tratado;
- [ ] erro tratado;
- [ ] ausência de Samples tratada;
- [ ] desktop funcionando;
- [ ] mobile funcionando.

## Testes

- [ ] payload válido;
- [ ] JSON inválido;
- [ ] vehicleId ausente;
- [ ] deviceId ausente;
- [ ] samples vazio;
- [ ] topic divergente;
- [ ] persistência Event + Samples;
- [ ] FK validada;
- [ ] GET existente;
- [ ] GET inexistente;
- [ ] ordenação temporal.

## Qualidade

- [ ] backend tests verdes;
- [ ] frontend lint verde;
- [ ] frontend build verde;
- [ ] simulador compila;
- [ ] CI verde.

## E2E

- [ ] HARD_BRAKING detectado;
- [ ] Event Window publicada;
- [ ] backend recebeu;
- [ ] backend persistiu;
- [ ] API retornou Samples;
- [ ] React exibiu timeline;
- [ ] `t=0` visível;
- [ ] evidências anexadas.

## Documentação

- [ ] `docs/sprints/sprint-03.md` atualizado;
- [ ] contrato atualizado;
- [ ] backlog atualizado após conclusão;
- [ ] continuity atualizado após conclusão;
- [ ] README atualizado se necessário.

---

# STATUS

**SPRINT 03 — PLANEJADA / PRONTA PARA EXECUÇÃO**

Objetivo central:

**Event Window real, persistida e visualizada do início ao fim.**

Critério definitivo:

**ver no React o comportamento do veículo antes, em `t=0` e depois do evento.**