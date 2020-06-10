using NetworkMonitor.Objects;
using System.Collections.Generic;

namespace NetworkMonitor.Services
{
    public interface INetStatsService
    {
        List<NetStat> NetStatData { get; set; }
        void init(bool disable,int deviceId);
        void start();
        void stop();
    }
}