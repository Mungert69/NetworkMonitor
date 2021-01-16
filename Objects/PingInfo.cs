using NetworkMonitor.Objects;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetworkMonitor
{
    public class PingInfo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public DateTime DateSent { get; set; }

        public string Status { get; set; }

        public int RoundTripTime { get; set; }

        public int MonitorPingInfoID { get; set; }
        public MonitorPingInfo MonitorPingInfo { get; set; }
    }
}
