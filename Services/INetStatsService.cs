using NetworkMonitor.Objects;
using System.Collections.Generic;

namespace NetworkMonitor.Services
{
    public interface INetStatsService
    {
        List<NetStat> NetStatData { get; set; }

        void Dispose();
        void start(int deviceId);
        void stop();
    }
}