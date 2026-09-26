import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import VehicleDashboard from "./pages/VehicleDashboard";
import EventHistory from "./pages/EventHistory";
import EventDetail from "./pages/EventDetail";
import "./App.css";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path="/dashboard" element={<VehicleDashboard />} />
        <Route path="/events" element={<EventHistory />} />
        <Route path="/events/:id" element={<EventDetail />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
