# Vehicle Black Box — Backlog

## Objetivo do projeto

Construir uma caixa-preta veicular capaz de coletar, processar,
armazenar e visualizar dados de condução.

O projeto começa com dados simulados e evolui posteriormente
para hardware real.

## MVP

Fluxo inicial:

C++ Simulator
      ↓
     MQTT
      ↓
C# / ASP.NET Core
      ↓
    SQLite
      ↓
    React

---

## Fase 1 — Documentação e arquitetura

- [x] Criar repositório
- [x] Criar documentação inicial
- [ ] Definir arquitetura
- [ ] Definir dados de telemetria
- [ ] Definir eventos
- [ ] Definir formato das mensagens MQTT

---

## Fase 2 — Simulador C++

- [ ] Criar projeto C++
- [ ] Criar estrutura TelemetryData
- [ ] Simular velocidade
- [ ] Simular RPM
- [ ] Simular temperatura
- [ ] Simular aceleração longitudinal
- [ ] Simular aceleração lateral
- [ ] Simular GPS
- [ ] Gerar timestamp
- [ ] Criar cenários de condução
- [ ] Criar buffer circular de telemetria
- [ ] Detectar eventos

---

## Fase 3 — MQTT

- [ ] Configurar broker MQTT
- [ ] Definir tópicos MQTT
- [ ] Serializar telemetria em JSON
- [ ] Publicar telemetria pelo C++
- [ ] Receber telemetria no backend
- [ ] Testar comunicação C++ → MQTT → C#

---

## Fase 4 — Backend C#

Stack:

- C#
- ASP.NET Core
- Entity Framework Core
- SQLite

Tarefas:

- [ ] Criar projeto ASP.NET Core
- [ ] Configurar SQLite
- [ ] Criar entidade Vehicle
- [ ] Criar entidade Telemetry
- [ ] Criar entidade Event
- [ ] Criar migrations
- [ ] Criar API de veículos
- [ ] Criar API de telemetria
- [ ] Criar API de eventos
- [ ] Integrar MQTT
- [ ] Validar dados recebidos
- [ ] Persistir telemetria
- [ ] Persistir eventos

---

## Fase 5 — Frontend React

- [ ] Criar projeto React
- [ ] Criar dashboard
- [ ] Mostrar veículos
- [ ] Mostrar telemetria atual
- [ ] Mostrar histórico de eventos
- [ ] Criar tela de detalhes do evento
- [ ] Criar gráficos
- [ ] Criar visualização de localização

---

## Fase 6 — Eventos

Primeiros eventos:

- [ ] Frenagem brusca
- [ ] Aceleração brusca
- [ ] Excesso de velocidade
- [ ] Curva brusca
- [ ] Temperatura elevada
- [ ] Possível impacto

---

## Fase 7 — Testes

- [ ] Testes unitários C++
- [ ] Testes unitários C#
- [ ] Testes da API
- [ ] Testes MQTT
- [ ] Teste completo C++ → MQTT → C# → SQLite → React

---

## Fase 8 — Hardware real

Depois do MVP:

- [ ] Raspberry Pi
- [ ] Interface CAN
- [ ] GPS
- [ ] Acelerômetro
- [ ] Leitura CAN
- [ ] Leitura GPS
- [ ] Leitura do acelerômetro
- [ ] Substituir simulador por dados reais

---

# Milestone 1

O primeiro grande objetivo do projeto é:

C++ Simulator
      ↓
detecta frenagem brusca
      ↓
MQTT
      ↓
Backend C#
      ↓
SQLite
      ↓
React
      ↓
Evento exibido no dashboard

Quando esse fluxo funcionar, teremos o primeiro MVP completo
da Vehicle Black Box.
