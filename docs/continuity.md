# Vehicle Black Box — Contexto de Continuidade

> Checkpoint: 2026-09-26
> Objetivo: permitir a retomada segura do projeto por Gisa, Pedro ou outra IA.

## Estado atual

Sprint 1 concluída.

A branch `main` contém:

- simulador C++;
- detecção inicial `HARD_BRAKING`;
- backend ASP.NET Core;
- Entity Framework Core;
- SQLite;
- fluxo completo de Events;
- fluxo completo de Telemetry;
- frontend React;
- dashboard de telemetria;
- histórico e detalhe de eventos;
- testes automatizados;
- GitHub Actions.

Checkpoint da `main` no início deste cleanup:

`01dad6d`

## Pull Requests principais

- PR #3 — Events / Pedro
- PR #4 — Telemetry / Gisa
- PR #5 — CI / quality gates

## Qualidade confirmada

- backend: 13 testes passando;
- frontend lint: 0 warnings e 0 erros;
- frontend build: sucesso;
- GitHub Actions: sucesso;
- CI verde na `main`.

## Backend

DbContext compartilhado:

`VehicleBlackBoxContext`

DbSets:

- Events
- Vehicles
- Telemetries

Regra: não criar outro DbContext.

### Events

- POST `/api/events`
- GET `/api/vehicles/{vehicleId}/events`
- GET `/api/events/{id}`

### Telemetry / Vehicles

- POST `/api/telemetry`
- GET `/api/vehicles`
- GET `/api/vehicles/{id}`
- GET `/api/vehicles/{id}/telemetry/latest`

## Frontend

Rotas atuais:

- `/dashboard`
- `/events`
- `/events/:id`

Dashboard e Events consomem a API real.

## Simulador

Local:

`embedded/simulator`

Tecnologias:

- C++17
- CMake

Já simula telemetria e detecta `HARD_BRAKING`.

## Git

Não trabalhar diretamente na `main`.

Branches antigas não devem ser reutilizadas para novas features:

- `feature/events-flow`
- `feature/simulator`
- `chore/project-foundation`

Novas sprints devem nascer da `main` atualizada.

## Sprint 2

Ainda não está congelada.

Existe uma direção histórica de integração por MQTT, mas arquitetura, escopo e divisão entre Gisa e Pedro serão revisados antes da implementação.

Não iniciar código da Sprint 2 antes de o plano definitivo ser aprovado.

## Dívidas técnicas conhecidas

- estratégia de DTOs ainda não consolidada;
- URL da API do frontend está hardcoded;
- origem de CORS está hardcoded;
- timestamp SQLite merece revisão de UTC;
- relacionamento Vehicle/Telemetry pode ser fortalecido;
- criação automática de Vehicle merece revisão futura;
- bundle React gera aviso acima de 500 kB;
- CI ainda não compila o simulador C++;
- `main` ainda não possui proteção obrigatória.

Esses itens não significam que a Sprint 1 esteja quebrada.

## Como retomar o projeto

Primeiro atualizar e inspecionar:

```bash
git switch main
git fetch --prune origin
git pull --ff-only origin main
git status --short
git log --oneline -10

