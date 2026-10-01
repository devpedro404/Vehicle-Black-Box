# Vehicle Black Box

Automotive black box platform for simulated and future real-world vehicle telemetry, event detection, persistence, transport and visualization.

## Current status

Sprint 1 is complete.

Sprint 2 is complete.

Sprint 3 is planned and ready for execution.

Current runtime flow:

    C++ Simulator
          ↓
    MQTT / Eclipse Mosquitto
          ↓
    ASP.NET Core
          ↓
    SQLite
          ↓
    React

The project currently supports telemetry generation in the C++ simulator, MQTT transport, backend validation and persistence, HTTP API access and visualization through the React frontend.

MQTT is already part of the runtime architecture.

Sprint 3 will evolve the Events flow with Event Window support, preserving telemetry context before, during and after an event.

---

## Current stack

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

PostgreSQL remains a possible future evolution for server environments.

It is not the current database.

---

## Main capabilities already implemented

### Simulator / Core

- simulated vehicle telemetry;
- initial `HARD_BRAKING` detection;
- telemetry JSON serialization;
- MQTT connection;
- telemetry publication through MQTT;
- topic generation based on `vehicleId`;
- minimal MQTT reconnect handling.

### MQTT

Current telemetry topic:

    vehicle/{vehicleId}/telemetry

Example:

    vehicle/CAR-001/telemetry

Backend subscription:

    vehicle/+/telemetry

Current development broker:

    Eclipse Mosquitto
    Host: localhost
    Port: 1883

MQTT is used as transport.

It is not responsible for persistence or business rules.

---

## Backend

The ASP.NET Core backend currently provides:

- Events API;
- Telemetry API;
- Vehicles API;
- MQTT subscriber;
- telemetry validation;
- topic/payload consistency validation;
- SQLite persistence;
- automatic Vehicle creation when required by the current telemetry flow;
- MQTT reconnect handling;
- HTTP endpoints consumed by the frontend.

Shared DbContext:

    VehicleBlackBoxContext

The project must continue using the shared context rather than introducing parallel DbContexts without an explicit architecture decision.

---

## Events

Implemented in Sprint 1:

- `Event` entity;
- SQLite persistence;
- Events API;
- event history;
- event detail screen;
- initial visualization;
- automated backend tests.

Main endpoints:

    POST /api/events
    GET /api/vehicles/{vehicleId}/events
    GET /api/events/{id}

The current simulator includes initial detection of:

    HARD_BRAKING

Additional event types remain part of future evolution.

---

## Telemetry

Implemented and evolved through Sprints 1 and 2:

- `Vehicle` entity;
- `Telemetry` entity;
- SQLite persistence;
- HTTP telemetry ingestion;
- MQTT telemetry ingestion;
- vehicle queries;
- latest telemetry query;
- payload validation;
- MQTT topic validation;
- React dashboard using the real API.

Main endpoints:

    POST /api/telemetry
    GET /api/vehicles
    GET /api/vehicles/{id}
    GET /api/vehicles/{id}/telemetry/latest

Current telemetry fields include:

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

Example:

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

---

## MQTT validation

The backend uses:

    TelemetryMqttValidator

Current validation includes:

- malformed JSON rejection;
- empty payload rejection;
- required `vehicleId`;
- required `deviceId`;
- MQTT topic format validation;
- comparison between `vehicleId` in the topic and payload.

Example of an invalid combination:

    Topic:
    vehicle/CAR-002/telemetry

    Payload:
    vehicleId = CAR-001

This message is rejected and is not persisted.

Invalid MQTT payloads must not terminate the backend service.

---

## MQTT reconnect

The backend is designed to tolerate a temporarily unavailable broker.

Current behavior:

    Backend starts
          ↓
    tries MQTT connection
          ↓
    connection fails
          ↓
    waits approximately 5 seconds
          ↓
    tries again

When the broker becomes available, the backend reconnects and subscribes again.

The simulator also contains minimal reconnect behavior and can continue running when a publication cannot be completed.

---

## Frontend

The React application currently provides:

- automotive telemetry dashboard;
- vehicle information;
- speed visualization;
- RPM visualization;
- engine temperature;
- longitudinal acceleration;
- lateral acceleration;
- GPS information;
- event history;
- event detail screen;
- loading states;
- error states;
- responsive layout.

The frontend consumes the backend HTTP API.

It does not connect directly to the MQTT broker.

Current main routes include:

    /dashboard
    /events
    /events/:id

---

## Automotive dashboard

The Sprint 2 dashboard includes:

- automotive visual layout;
- speed gauge;
- RPM gauge;
- engine temperature visualization;
- acceleration information;
- GPS information;
- monitored vehicle information;
- device information;
- last update information;
- MQTT integration indication;
- responsive behavior for desktop, tablet and mobile;
- access to event history.

The dashboard continues using real backend API data.

---

## Repository structure

    backend/     ASP.NET Core API, EF Core, SQLite and automated tests
    embedded/    C++ simulator and MQTT publisher
    frontend/    React + Vite application
    docs/        architecture, contracts, backlog, continuity and sprint documentation
    .github/     GitHub Actions workflows

---

## Documentation

When resuming the project, start with:

1. `README.md`
2. `docs/continuity.md`
3. `docs/backlog.md`
4. `docs/architecture.md`
5. `docs/telemetry.md`
6. `docs/events.md`
7. `docs/mqtt.md`
8. `docs/sprints/sprint-02.md`
9. `docs/sprints/sprint-03.md`

Sprint-specific documentation:

    docs/sprints/sprint-01-summary.md
    docs/sprints/sprint-02.md
    docs/sprints/sprint-03.md

The current `main` branch and versioned documentation are the project source of truth.

---

## Development rules

- do not work directly on `main`;
- create new feature, chore or docs branches from an updated `main`;
- run `git fetch` and update `main` before creating a new branch;
- keep commits focused;
- keep pull requests focused on one main responsibility;
- do not use force push on `main`;
- preserve the shared `VehicleBlackBoxContext`;
- do not create another DbContext without an explicit architecture decision;
- do not migrate to PostgreSQL without an explicit project decision;
- preserve the current telemetry contract unless a contract change is explicitly agreed;
- document contract changes before integrating different implementations;
- do not reuse old feature branches for new Sprint work;
- do not expand Sprint scope without explicit alignment;
- wait for CI before merging pull requests.

---

## Quality gates

Current GitHub Actions CI validates:

### Backend

- dependency restore;
- automated tests.

Current backend test checkpoint:

    17 tests passing

### Frontend

- dependency installation;
- lint;
- production build.

### C++ simulator

- required MQTT development dependency installation;
- CMake configuration;
- CMake build.

The C++ simulator is now part of the automated CI quality gate.

---

## Completed Sprints

### Sprint 1 — Complete

Main achievements:

- Events flow;
- Telemetry flow;
- SQLite persistence;
- backend APIs;
- React integration;
- automated tests;
- initial CI.

### Sprint 2 — Complete

Main objective:

    C++ Simulator → MQTT → ASP.NET Core → SQLite → React

Main achievements:

- MQTT publisher in C++;
- cJSON telemetry serialization;
- Mosquitto integration;
- MQTT subscriber in ASP.NET Core;
- telemetry validation;
- topic/payload validation;
- SQLite persistence of MQTT telemetry;
- MQTT configuration through application settings;
- backend reconnect;
- simulator reconnect;
- expanded backend tests;
- C++ build added to CI;
- automotive dashboard finalization.

Sprint 2 implementation was delivered through PRs #7 to #12.

Detailed documentation:

    docs/sprints/sprint-02.md

---

## Current Sprint direction

### Sprint 3 — Planned / Ready for execution

Main objective:

    Event Window

Sprint 3 is intended to evolve Events from isolated records into observable occurrences with telemetry context.

Expected high-level flow:

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

The objective is to preserve and visualize what the vehicle was doing:

    before the event
          ↓
    t = 0
          ↓
    after the event

The initial integration target remains:

    HARD_BRAKING

Expected Sprint 3 responsibilities include:

### Core / C++

- circular telemetry buffer;
- pre-event samples;
- event detection;
- post-event samples;
- Event Window creation;
- Event Window serialization;
- MQTT publication.

### Backend

- Event Window reception;
- validation;
- Event/Sample persistence;
- relationship between Event and Samples;
- API access to the temporal window.

### Frontend

- Event Window consumption;
- temporal visualization;
- clear `t=0` indication;
- loading, error and empty states;
- responsive Event Detail visualization.

Detailed Sprint 3 plan:

    docs/sprints/sprint-03.md

---

## Sprint 3 Definition of Success

The central validation question is:

> Can the system open an event and clearly show what the vehicle was doing before, at the moment of the event and after it?

The expected final flow is:

    C++ Core → Event Window → MQTT → ASP.NET Core → SQLite → API → React Timeline

Sprint 3 is not considered implemented yet.

Its documentation defines the planned contract, responsibilities, tests, integration and Definition of Done.

---

## Future evolution

Possible future developments include:

- additional event types;
- configurable Event Window duration;
- stronger event deduplication;
- advanced cooldown rules;
- event severity scoring;
- WebSocket or SignalR where justified;
- PostgreSQL;
- authentication;
- MQTT TLS;
- stronger MQTT QoS/idempotency strategies;
- real vehicle hardware;
- CAN;
- OBD-II;
- physical GPS;
- accelerometer/gyroscope;
- embedded local storage;
- offline synchronization;
- advanced observability.

These items are not required to begin Sprint 3 unless explicitly added to scope.

---

## Current architectural principle

The project currently follows this separation:

    Simulator / Core
          ↓
    produces telemetry and detects events

    MQTT
          ↓
    transports data

    ASP.NET Core
          ↓
    validates and persists

    SQLite
          ↓
    stores current data

    HTTP API
          ↓
    exposes data

    React
          ↓
    visualizes data

Business responsibilities should remain separated.

The frontend must not become responsible for event detection.

MQTT must not become responsible for persistence.

The backend must not reconstruct Core logic that belongs to the simulator/embedded layer.

---

## Source of truth

When documentation, old conversations, local assumptions or previous branches disagree, use this priority:

1. current `main`;
2. versioned project documentation;
3. frozen Sprint contract;
4. recent merged pull requests.

Always inspect the current repository state before implementing new work.