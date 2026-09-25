import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip } from "recharts";
import { getEventById } from "../services/eventsService";

export default function EventDetail() {
  const { id } = useParams();
  const [event, setEvent] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    getEventById(id)
      .then(setEvent)
      .catch((err) => setError(err.message));
  }, [id]);

  if (error) return <p>Erro: {error}</p>;
  if (!event) return <p>Carregando evento...</p>;

  const chartData = [
    { label: "Velocidade (km/h)", value: event.speed },
    { label: "Aceleração (m/s²)", value: event.acceleration },
  ];

  return (
    <div>
      <h1>Detalhe do Evento #{event.id}</h1>
      <p><strong>Veículo:</strong> {event.vehicleId}</p>
      <p><strong>Tipo:</strong> {event.eventType}</p>
      <p><strong>Data/Hora:</strong> {new Date(event.timestamp).toLocaleString()}</p>
      <p><strong>Velocidade:</strong> {event.speed} km/h</p>
      <p><strong>Aceleração:</strong> {event.acceleration} m/s²</p>

      <LineChart width={400} height={250} data={chartData}>
        <CartesianGrid strokeDasharray="3 3" />
        <XAxis dataKey="label" />
        <YAxis />
        <Tooltip />
        <Line type="monotone" dataKey="value" stroke="#8884d8" />
      </LineChart>
    </div>
  );
}