const API_BASE_URL = "http://localhost:5237/api";

export async function getVehicles() {
  const response = await fetch(`${API_BASE_URL}/vehicles`);

  if (!response.ok) {
    throw new Error("Erro ao buscar veículos");
  }

  return response.json();
}

export async function getLatestTelemetry(vehicleId) {
  const response = await fetch(
    `${API_BASE_URL}/vehicles/${encodeURIComponent(vehicleId)}/telemetry/latest`
  );

  if (response.status === 404) {
    throw new Error("Telemetria não encontrada");
  }

  if (!response.ok) {
    throw new Error("Erro ao buscar telemetria");
  }

  return response.json();
}
