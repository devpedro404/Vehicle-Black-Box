#include <cstdint>
#include <iostream>

#include "event_buffer.h"

namespace
{
    int failures = 0;

    void check(bool condition, const char* expr, const char* file, int line)
    {
        if (!condition)
        {
            ++failures;
            std::cerr << "  FALHOU: " << expr << " (" << file << ":" << line << ")\n";
        }
    }

#define CHECK(expr) check((expr), #expr, __FILE__, __LINE__)

    TelemetryData makeSample(double speed)
    {
        TelemetryData t;
        t.deviceId = "VBB-001";
        t.vehicleId = "CAR-001";
        t.timestamp = "2026-01-01T00:00:00Z";
        t.speed = speed;
        t.rpm = 1000;
        t.engineTemperature = 89.0;
        t.longitudinalAcceleration = 0.0;
        t.lateralAcceleration = 0.0;
        t.latitude = 0.0;
        t.longitude = 0.0;
        return t;
    }

    void testEmptyBuffer()
    {
        EventBuffer buffer(5000);
        CHECK(buffer.empty());
        CHECK(buffer.size() == 0);
        CHECK(buffer.snapshot().empty());
    }

    void testPartialBufferKeepsAvailableHistory()
    {
        EventBuffer buffer(5000);
        buffer.push(0, makeSample(60));
        buffer.push(1000, makeSample(62));

        auto snap = buffer.snapshot();
        CHECK(snap.size() == 2);
        CHECK(snap.front().timestampMs == 0);
        CHECK(snap.back().timestampMs == 1000);
    }

    void testFullBufferDiscardsOldest()
    {
        EventBuffer buffer(5000);
        for (int i = 0; i <= 8; ++i)
        {
            buffer.push(i * 1000, makeSample(60 + i));
        }

        auto snap = buffer.snapshot();
        CHECK(snap.size() == 6);
        CHECK(snap.front().timestampMs == 3000);
        CHECK(snap.back().timestampMs == 8000);
    }

    void testBoundarySampleIsKept()
    {
        EventBuffer buffer(5000);
        buffer.push(0, makeSample(60));
        buffer.push(5000, makeSample(61));

        CHECK(buffer.size() == 2);

        buffer.push(5001, makeSample(62));

        CHECK(buffer.size() == 2);
        CHECK(buffer.snapshot().front().timestampMs == 5000);
    }

    void testTemporalOrder()
    {
        EventBuffer buffer(5000);
        for (int i = 0; i < 10; ++i)
        {
            buffer.push(i * 500, makeSample(i));
        }

        auto snap = buffer.snapshot();
        bool ordered = true;
        for (std::size_t i = 1; i < snap.size(); ++i)
        {
            if (snap[i].timestampMs < snap[i - 1].timestampMs)
            {
                ordered = false;
            }
        }
        CHECK(ordered);
    }

    void testOutOfOrderIsRejected()
    {
        EventBuffer buffer(5000);
        CHECK(buffer.push(2000, makeSample(60)));
        CHECK(!buffer.push(1000, makeSample(61)));
        CHECK(buffer.size() == 1);
        CHECK(buffer.snapshot().front().timestampMs == 2000);

        CHECK(buffer.push(2000, makeSample(62)));
        CHECK(buffer.size() == 2);
    }

    void testSnapshotIsIndependent()
    {
        EventBuffer buffer(5000);
        buffer.push(0, makeSample(60));
        buffer.push(1000, makeSample(61));

        auto snap = buffer.snapshot();
        CHECK(snap.size() == 2);

        buffer.push(2000, makeSample(62));
        buffer.push(7000, makeSample(63));

        CHECK(snap.size() == 2);
        CHECK(snap.front().timestampMs == 0);
        CHECK(snap.back().timestampMs == 1000);
        CHECK(buffer.size() == 2);
        CHECK(buffer.snapshot().front().timestampMs == 2000);
        CHECK(buffer.snapshot().back().timestampMs == 7000);
    }

    void testBufferDoesNotGrowForever()
    {
        EventBuffer buffer(5000);
        for (int i = 0; i < 10000; ++i)
        {
            buffer.push(i * 1000, makeSample(i));
        }
        CHECK(buffer.size() == 6);
    }

    void testClear()
    {
        EventBuffer buffer(5000);
        buffer.push(0, makeSample(60));
        buffer.clear();
        CHECK(buffer.empty());
    }

    void run(const char* name, void (*fn)())
    {
        int before = failures;
        fn();
        std::cout << (failures == before ? "[OK]   " : "[FAIL] ") << name << "\n";
    }
}

int main()
{
    run("buffer vazio", testEmptyBuffer);
    run("buffer parcial", testPartialBufferKeepsAvailableHistory);
    run("buffer cheio descarta o mais antigo", testFullBufferDiscardsOldest);
    run("amostra no limite do horizonte", testBoundarySampleIsKept);
    run("ordem temporal", testTemporalOrder);
    run("timestamp fora de ordem", testOutOfOrderIsRejected);
    run("snapshot independente", testSnapshotIsIndependent);
    run("buffer nao cresce indefinidamente", testBufferDoesNotGrowForever);
    run("clear", testClear);

    if (failures > 0)
    {
        std::cerr << "\n" << failures << " verificacao(oes) falharam.\n";
        return 1;
    }

    std::cout << "\nTodos os testes passaram.\n";
    return 0;
}