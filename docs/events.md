# Vehicle Black Box — Events

Este documento define os eventos planejados para a Vehicle Black Box.

Os limites atuais são experimentais e poderão ser ajustados durante testes. Eles não representam calibração certificada de segurança automotiva.

## HARD_BRAKING

Frenagem brusca.

Regra inicial:

```text
longitudinalAcceleration <= -5.0 m/s²
```

Exemplo:

```text
Velocidade: 82 km/h → 31 km/h
Aceleração: -7.2 m/s²
```

Este é o único evento com detecção inicial já implementada no simulador no checkpoint de 2026-09-26.

## HARD_ACCELERATION

Aceleração brusca.

Regra inicial planejada:

```text
longitudinalAcceleration >= 4.0 m/s²
```

## SPEEDING

Excesso de velocidade.

Regra planejada:

```text
speed > speedLimit
```

O limite deverá ser configurável.

## HARD_CORNERING

Curva brusca.

Usa aceleração lateral.

Regra planejada:

```text
abs(lateralAcceleration) >= lateralAccelerationLimit
```

O limite será definido durante testes.

## HIGH_ENGINE_TEMPERATURE

Temperatura elevada do motor.

Regra inicial planejada:

```text
engineTemperature > 100°C
```

O limite deverá ser configurável.

## POSSIBLE_IMPACT

Possível impacto.

Não deverá depender de uma única variável.

Poderá considerar:

- desaceleração muito elevada;
- pico de aceleração;
- mudança brusca de velocidade;
- aceleração lateral elevada.

Inicialmente será apenas uma heurística experimental.

Não representa um sistema certificado de detecção de acidentes.

## Event Window

Visão futura:

```text
[-5s][-4s][-3s][-2s][-1s][EVENTO][+1s][+2s][+3s][+4s][+5s]
```

Quando essa funcionalidade for implementada, o sistema deverá preservar telemetria anterior e posterior ao evento.

Dados previstos:

- velocidade;
- aceleração;
- RPM;
- temperatura;
- GPS;
- timestamp.

Essa janela ainda não está implementada.