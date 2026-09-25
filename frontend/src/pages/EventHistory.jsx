import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getEventsByVehicle } from "../services/eventsService";

export default function EventHistory({ vehicleId = "CAR-001" }) {
  const [events, setEvents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    getEventsByVehicle(vehicleId)
      .then(setEvents)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [vehicleId]);

  if (loading) return <p>Carregando eventos...</p>;
  if (error) return <p>Erro: {error}</p>;

  return (
    <div>
      <h1>Histórico de Eventos — {vehicleId}</h1>
      {events.length === 0 ? (
        <p>Nenhum evento encontrado.</p>
      ) : (
        <table>
          <thead>
            <tr>
              <th>Data/Hora</th>
              <th>Tipo</th>
              <th>Velocidade</th>
              <th>Aceleração</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {events.map((event) => (
              <tr key={event.id}>
                <td>{new Date(event.timestamp).toLocaleString()}</td>
                <td>{event.eventType}</td>
                <td>{event.speed} km/h</td>
                <td>{event.acceleration} m/s²</td>
                <td>
                  <Link to={`/events/${event.id}`}>Ver detalhe</Link>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}