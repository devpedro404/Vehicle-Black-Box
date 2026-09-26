import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getLatestTelemetry } from "../services/telemetryService";

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
    return <p className="status">Carregando telemetria...</p>;
  }

  if (error) {
    return <p className="status">Erro: {error}</p>;
  }

  return (
    <main className="dashboard">
      <div className="dashboard-header">
        <div>
          <p className="eyebrow">Vehicle Black Box</p>
          <h1>Dashboard — {vehicleId}</h1>
          <p>
            Dispositivo: <strong>{telemetry.deviceId}</strong>
          </p>
        </div>

        <button onClick={handleRefresh}>Atualizar</button>
      </div>

      <section className="telemetry-grid">
        <article className="telemetry-card">
          <span>Velocidade</span>
          <strong>{telemetry.speed} km/h</strong>
        </article>

        <article className="telemetry-card">
          <span>RPM</span>
          <strong>{telemetry.rpm}</strong>
        </article>

        <article className="telemetry-card">
          <span>Temperatura</span>
          <strong>{telemetry.engineTemperature} °C</strong>
        </article>

        <article className="telemetry-card">
          <span>Aceleração longitudinal</span>
          <strong>{telemetry.longitudinalAcceleration} m/s²</strong>
        </article>

        <article className="telemetry-card">
          <span>Aceleração lateral</span>
          <strong>{telemetry.lateralAcceleration} m/s²</strong>
        </article>

        <article className="telemetry-card">
          <span>GPS</span>
          <strong>
            {telemetry.latitude}, {telemetry.longitude}
          </strong>
        </article>
      </section>

      <section className="telemetry-footer">
        <p>
          Última leitura:{" "}
          {new Date(telemetry.timestamp).toLocaleString()}
        </p>

        <Link to="/events">Ver histórico de eventos</Link>
      </section>
    </main>
  );
}
