# Vehicle Black Box — Architecture

## Objetivo

Construir um sistema de caixa-preta veicular capaz de coletar,
processar, armazenar e visualizar dados de condução.

## Arquitetura inicial

C++ Simulator
      ↓
     MQTT
      ↓
C# / ASP.NET Core
      ↓
    SQLite
      ↓
    React

## Arquitetura futura

CAN + GPS + Acelerômetro
          ↓
      Raspberry Pi
          ↓
          C++
          ↓
         MQTT
          ↓
     C# Backend
          ↓
 SQLite / PostgreSQL
          ↓
        React
