import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getLatestTelemetry } from "../services/telemetryService";
import carroTopo from "../assets/carro.png";
import carroLateral from "../assets/carro2.png";

function Gauge({ type, value, max, unit }) {
  const numericValue = Number(value) || 0;
  const percent = Math.min(Math.max(numericValue / max, 0), 1);
  const angle = 270 * percent;

  return (
    <div className={`main-gauge ${type}`}>
      <div
        className="gauge-arc"
        style={{ "--gauge-angle": `${angle}deg` }}
      >
        <div className="gauge-center">
          <span className="gauge-value">
            {type === "rpm"
              ? numericValue.toLocaleString("pt-BR")
              : numericValue}
          </span>

          <span className="gauge-unit">{unit}</span>
        </div>
      </div>

      <div className="gauge-caption">
        {type === "speed" ? "VELOCIDADE" : "ROTAÇÃO DO MOTOR"}
      </div>
    </div>
  );
}

export default function VehicleDashboard({ vehicleId = "CAR-001" }) {
  const [telemetry, setTelemetry] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    let cancelled = false;

    getLatestTelemetry(vehicleId)
      .then((data) => {
        if (!cancelled) {
          setTelemetry(data);
          setError(null);
        }
      })
      .catch((err) => {
        if (!cancelled) {
          setError(err.message);
        }
      })
      .finally(() => {
        if (!cancelled) {
          setLoading(false);
        }
      });

    return () => {
      cancelled = true;
    };
  }, [vehicleId]);

  async function handleRefresh() {
    try {
      setLoading(true);
      setError(null);

      const data = await getLatestTelemetry(vehicleId);
      setTelemetry(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  if (loading) {
    return <div className="screen-status">Carregando telemetria...</div>;
  }

  if (error) {
    return <div className="screen-status">Erro: {error}</div>;
  }

  const formattedDate = new Date(telemetry.timestamp).toLocaleString("pt-BR");

  return (
    <main className="vbb-shell">
      <header className="topbar">
        <div className="brand">
          <div className="brand-symbol">⌁</div>
          <div>
            <h1>
              Vehicle <span>Black Box</span>
            </h1>
            <small>MONITORAMENTO VEICULAR INTELIGENTE</small>
          </div>
        </div>

        <div className="top-status">
          <div className="top-status-card">
            <span className="online-dot" />
            <div>
              <small>Telemetria disponível</small>
              <strong>{vehicleId}</strong>
            </div>
          </div>

          <div className="top-status-card">
            <span className="mini-icon">▣</span>
            <div>
              <small>Dispositivo</small>
              <strong>{telemetry.deviceId}</strong>
            </div>
          </div>

          <div className="top-status-card">
            <span className="wifi-icon">⌁</span>
            <div>
              <small>Integração MQTT</small>
              <strong>Configurada</strong>
            </div>
          </div>

          <div className="top-status-card">
            <span className="mini-icon">◷</span>
            <div>
              <small>Última atualização</small>
              <strong>{formattedDate}</strong>
            </div>
          </div>
        </div>
      </header>

      <div className="dashboard-layout">
        <aside className="sidebar">
          <nav>
            <a className="nav-active" href="/dashboard">
              <span>⌂</span> Dashboard
            </a>

            <a href="#gps">
              <span>⌖</span> Mapa em Tempo Real
            </a>

            <Link to="/events">
              <span>▤</span> Histórico de Eventos
            </Link>

            <a href="#telemetria">
              <span>⌁</span> Telemetria
            </a>

          </nav>

          <div className="sidebar-car">
            <img src={carroLateral} alt="Veículo monitorado" />
          </div>

          <div className="sidebar-version">
            Vehicle Black Box
            <small>v2.0.0</small>
          </div>
        </aside>

        <section className="dashboard-content">
          <div className="vehicle-identification">
            <div className="identity-block">
              <span className="identity-icon">▰</span>
              <div>
                <strong>{vehicleId}</strong>
                <small>Veículo Monitorado</small>
              </div>
            </div>

            <div className="identity-block">
              <span className="identity-icon">▣</span>
              <div>
                <strong>{telemetry.deviceId}</strong>
                <small>Dispositivo de Telemetria</small>
              </div>
            </div>

            <div className="identity-status">
              <strong>● Dados recebidos</strong>
              <small>Status da telemetria</small>
            </div>

            <div className="identity-block">
              <span className="identity-icon">⌁</span>
              <div>
                <strong>MQTT</strong>
                <small>Canal de telemetria configurado</small>
              </div>
            </div>
          </div>

          <section className="hero-panel">
            <Gauge
              type="speed"
              value={telemetry.speed}
              max={240}
              unit="km/h"
            />

            <div className="vehicle-radar">
              <div className="radar-ring radar-ring-1" />
              <div className="radar-ring radar-ring-2" />
              <div className="radar-ring radar-ring-3" />
              <div className="radar-cross radar-horizontal" />
              <div className="radar-cross radar-vertical" />

              <span className="direction north">N</span>
              <span className="direction south">S</span>
              <span className="direction west">O</span>
              <span className="direction east">L</span>

              <img src={carroTopo} alt="Carro visto de cima" />
            </div>

            <Gauge
              type="rpm"
              value={telemetry.rpm}
              max={8000}
              unit="RPM"
            />
          </section>

          <section className="telemetry-bottom" id="telemetria">
            <article className="data-module temperature-module">
              <span className="module-title">♨ TEMPERATURA DO MOTOR</span>

              <strong>
                {telemetry.engineTemperature}
                <small> °C</small>
              </strong>

              <div className="temperature-line">
                <span />
              </div>

              <div className="scale">
                <span>50</span>
                <span>90</span>
                <span>130</span>
              </div>
            </article>

            <article className="data-module">
              <span className="module-title">
                ↔ ACELERAÇÃO LONGITUDINAL
              </span>

              <strong>
                {telemetry.longitudinalAcceleration > 0 ? "+" : ""}
                {telemetry.longitudinalAcceleration}
                <small> m/s²</small>
              </strong>

              <div className="telemetry-wave wave-green" />

              <div className="scale">
                <span>-10</span>
                <span>0</span>
                <span>10</span>
              </div>
            </article>

            <article className="data-module">
              <span className="module-title">
                ↔ ACELERAÇÃO LATERAL
              </span>

              <strong>
                {telemetry.lateralAcceleration > 0 ? "+" : ""}
                {telemetry.lateralAcceleration}
                <small> m/s²</small>
              </strong>

              <div className="telemetry-wave wave-blue" />

              <div className="scale">
                <span>-10</span>
                <span>0</span>
                <span>10</span>
              </div>
            </article>

            <article className="data-module gps-module" id="gps">
              <span className="module-title">⌖ LOCALIZAÇÃO GPS</span>

              <strong className="gps-coordinates">
                {Number(telemetry.latitude).toFixed(4)},{" "}
                {Number(telemetry.longitude).toFixed(4)}
              </strong>

              <div className="fake-map">
                <div className="map-grid" />
                <span className="gps-point" />
              </div>
            </article>
          </section>

          <footer className="dashboard-footer">
            <div className="last-update">
              <span className="clock-icon">◷</span>

              <div>
                <small>Última atualização dos dados</small>
                <strong>{formattedDate}</strong>
              </div>
            </div>

            <button onClick={handleRefresh} className="refresh-button">
              ↻ Atualizar dados
            </button>

            <Link to="/events" className="history-button">
              ▤ Histórico de eventos <span>›</span>
            </Link>
          </footer>
        </section>
      </div>
    </main>
  );
}
