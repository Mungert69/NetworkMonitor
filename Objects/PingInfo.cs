using NetworkMonitor.Objects;
using System;
using System.ComponentModel.DataAnnotations;

namespace NetworkMonitor
{
    public class PingInfo
    {
        [Key]public int ID { get; set; }
        public DateTime DateSent { get; set; }

        public string Status { get; set; }
     
        public int RoundTripTime { get; set; }

        public int MonitorPingInfoID { get; set; }
        public MonitorPingInfo MonitorPingInfo { get; set; }
    }
}
