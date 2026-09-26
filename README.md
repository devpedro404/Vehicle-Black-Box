# Vehicle Black Box

Automotive black box platform for simulated and future real-world vehicle telemetry, event detection, persistence, and visualization.

## Current status

Sprint 1 is complete.

Current working flow:

```text
C++ Simulator
      ↓
ASP.NET Core
      ↓
SQLite
      ↓
React
```

MQTT is the planned communication layer for Sprint 2 and is not yet part of the completed runtime flow.

## Current stack

- C++17
- CMake
- C# / ASP.NET Core (.NET 10)
- Entity Framework Core
- SQLite
- React
- Vite
- xUnit
- GitHub Actions

PostgreSQL is a future evolution for server environments. It is not the current database.

## Main capabilities already implemented

- simulated vehicle telemetry;
- initial `HARD_BRAKING` detection in the C++ simulator;
- Events API and persistence;
- Telemetry API and persistence;
- vehicle queries;
- React telemetry dashboard;
- event history and detail screens;
- automated backend tests;
- frontend lint and production build checks;
- CI through GitHub Actions.

## Repository structure

```text
backend/     ASP.NET Core API, EF Core, SQLite, tests
embedded/    C++ simulator
frontend/    React + Vite application
docs/        architecture, contracts, backlog and continuity
.github/     GitHub Actions workflows
```

## Documentation

Start here when resuming the project:

1. `docs/continuity.md`
2. `docs/backlog.md`
3. `docs/architecture.md`
4. `docs/telemetry.md`
5. `docs/events.md`
6. `docs/mqtt.md`

## Development rules

- do not work directly on `main`;
- create feature/chore/docs branches from an updated `main`;
- keep commits small and focused;
- do not use force push on `main`;
- preserve the shared `VehicleBlackBoxContext`;
- do not migrate to PostgreSQL without an explicit project decision;
- do not start real hardware work before the simulator integration is complete.

## Quality gates

Current CI validates:

- backend restore;
- backend automated tests;
- frontend dependency installation;
- frontend lint;
- frontend production build.

## Next phase

Sprint 2 will be defined after pre-sprint cleanup and architecture review.

Historical direction:

```text
C++ Simulator → MQTT → ASP.NET Core → SQLite → React
```

The final Sprint 2 scope and task split between Gisa and Pedro must be documented before implementation starts.