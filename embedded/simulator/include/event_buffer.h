#ifndef EVENT_BUFFER_H
#define EVENT_BUFFER_H

#include <cstddef>
#include <cstdint>
#include <deque>
#include <vector>

#include "TelemetryData.h"

// Amostra guardada no buffer: telemetria + timestamp numerico (ms).
// O timestamp numerico e a referencia para ordenacao e descarte;
// a string ISO de TelemetryData continua sendo usada so para transporte.
struct BufferedSample
{
    std::int64_t timestampMs;
    TelemetryData telemetry;
};

// Buffer circular baseado em tempo (P3.1).
// - Nao depende de MQTT nem do backend.
// - Mantem apenas amostras dentro do horizonte (newest - oldest <= horizonMs).
// - Nao assume frequencia fixa de amostragem.
class EventBuffer
{
public:
    explicit EventBuffer(std::int64_t horizonMs)
        : horizonMs_(horizonMs < 0 ? 0 : horizonMs)
    {
    }

    bool push(std::int64_t timestampMs, const TelemetryData& telemetry)
    {
        if (!samples_.empty() && timestampMs < samples_.back().timestampMs)
        {
            return false;
        }

        samples_.push_back(BufferedSample{timestampMs, telemetry});

        while (!samples_.empty() &&
               timestampMs - samples_.front().timestampMs > horizonMs_)
        {
            samples_.pop_front();
        }

        return true;
    }

    std::vector<BufferedSample> snapshot() const
    {
        return std::vector<BufferedSample>(samples_.begin(), samples_.end());
    }

    std::size_t size() const { return samples_.size(); }
    bool empty() const { return samples_.empty(); }
    std::int64_t horizonMs() const { return horizonMs_; }

    void clear() { samples_.clear(); }

private:
    std::int64_t horizonMs_;
    std::deque<BufferedSample> samples_;
};

#endif