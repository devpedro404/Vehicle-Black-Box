import { mockEvents } from "../mocks/events";

const USE_MOCK = false; 
const API_BASE_URL = "http://localhost:5237/api";

export async function getEventsByVehicle(vehicleId) {
  if (USE_MOCK) {
    return mockEvents.filter((e) => e.vehicleId === vehicleId);
  }

  const response = await fetch(`${API_BASE_URL}/vehicles/${vehicleId}/events`);
  if (!response.ok) throw new Error("Erro ao buscar eventos");
  return response.json();
}

export async function getEventById(id) {
  if (USE_MOCK) {
    const event = mockEvents.find((e) => e.id === Number(id));
    if (!event) throw new Error("Evento não encontrado");
    return event;
  }

  const response = await fetch(`${API_BASE_URL}/events/${id}`);
  if (!response.ok) throw new Error("Evento não encontrado");
  return response.json();
}