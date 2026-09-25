
Os campos principais — velocidade, RPM, temperatura, aceleração e GPS — são os que já planejamos para a primeira versão. :contentReference[oaicite:1]{index=1}

---

## `docs/events.md`

```markdown
# Vehicle Black Box — Events

Este documento define os eventos inicialmente detectados
pela Vehicle Black Box.

Os limites são experimentais e poderão ser ajustados.

---

## HARD_BRAKING

Frenagem brusca.

Regra inicial:

longitudinalAcceleration <= -5.0 m/s²

Exemplo:

Velocidade:
82 km/h → 31 km/h

Aceleração:
-7.2 m/s²

---

## HARD_ACCELERATION

Aceleração brusca.

Regra inicial:

longitudinalAcceleration >= 4.0 m/s²

---

## SPEEDING

Excesso de velocidade.

Regra inicial:

speed > speedLimit

Exemplo inicial:

speedLimit = 100 km/h

O limite deverá ser configurável.

---

## HARD_CORNERING

Curva brusca.

Utiliza aceleração lateral.

Regra inicial:

abs(lateralAcceleration) >= lateralAccelerationLimit

O limite será definido durante os testes.

---

## HIGH_ENGINE_TEMPERATURE

Temperatura elevada do motor.

Regra inicial:

engineTemperature > 100°C

O limite deverá ser configurável.

---

## POSSIBLE_IMPACT

Possível impacto.

Esse evento não deverá depender apenas de uma variável.

Poderá considerar:

- desaceleração muito elevada
- pico de aceleração
- mudança brusca de velocidade
- aceleração lateral elevada

Inicialmente será apenas uma heurística experimental.

Não representa um sistema certificado de detecção de acidentes.

---

# Event Window

Quando um evento ocorrer, o sistema deverá preservar dados:

5 segundos antes
+
momento do evento
+
5 segundos depois

Exemplo:

[-5][-4][-3][-2][-1][EVENTO][+1][+2][+3][+4][+5]
