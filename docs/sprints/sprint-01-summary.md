# Vehicle Black Box — Sprint 1 — Resumo Final

Status: CONCLUÍDA.

## Objetivo

Construir fluxos completos de backend + frontend antes da integração do simulador por MQTT.

## Pedro — Events

Fluxo concluído:

```text
SQLite → ASP.NET Core → API Events → React → Histórico → Detalhe
```

Entregas:

- entidade `Event`;
- persistência SQLite;
- migration;
- POST de Event;
- histórico por veículo;
- detalhe de Event;
- histórico no React;
- tela de detalhe;
- gráfico inicial;
- integração com API real;
- testes automatizados.

PR principal:

`#3 Feature/events flow`

## Gisa — Telemetry

Fluxo concluído:

```text
React Dashboard → ASP.NET Core → Telemetry → SQLite
```

Entregas:

- `Vehicle`;
- `Telemetry`;
- DbContext compartilhado;
- migration;
- POST de Telemetry;
- listagem de Vehicles;
- consulta de Vehicle;
- latest Telemetry;
- dashboard;
- velocidade;
- RPM;
- temperatura;
- acelerações;
- GPS;
- service do frontend;
- API real;
- testes automatizados;
- validação ponta a ponta.

PR:

`#4 feat: conclui fluxo de telemetria da Sprint 1`

## Quality Gate

Após a Sprint 1:

- 13 testes backend passaram;
- lint frontend ficou com 0 warnings e 0 erros;
- build frontend passou;
- GitHub Actions foi adicionado;
- CI passou no PR e na `main`.

PR:

`#5 ci: adiciona quality gates automatizados`

## Itens deliberadamente adiados

- MQTT;
- hardware real;
- CAN / OBD-II;
- PostgreSQL;
- Device completo;
- janela temporal de evento;
- streaming realtime para React;
- DTOs completos;
- otimizações de bundle.

Esses itens são evolução e não falhas da Sprint 1.