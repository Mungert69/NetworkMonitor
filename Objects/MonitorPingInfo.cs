using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkMonitor.Objects
{
    public class MonitorPingInfo
    {
        [Key]public int ID { get; set; }
        private DateTime dateStarted = DateTime.Now;
        private int roundTripTimeMinimum = 999;

        public List<PingInfo> pingInfos = new List<PingInfo>();
        public string Status { get; set; }
        public int DestinationUnreachable { get; set; }
        public int TimeOuts { get; set; }

        public string IPAddress { get; set; }
        public int PacketsRecieved { get; set; }
        public int PacketsLost { get; set; }
        public int PacketsLostPercentage { get; set; }
       
        public int RoundTripTimeMaximum { get; set; }
        public float RoundTripTimeAverage { get; set; }

        public int RoundTripTimeTotal { get; set; }

        public int PacketsSent { get; internal set; }
        public DateTime DateStarted { get => dateStarted; set => dateStarted = value; }
        public int RoundTripTimeMinimum { get => roundTripTimeMinimum; set => roundTripTimeMinimum = value; }
    }
}
