import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import EventHistory from "./pages/EventHistory";
import EventDetail from "./pages/EventDetail";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/events" replace />} />
        <Route path="/events" element={<EventHistory />} />
        <Route path="/events/:id" element={<EventDetail />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;