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
        private DateTime _dateStarted = DateTime.Now;
        private int _roundTripTimeMinimum = 999;
        private StatusObj _monitorStatus=new StatusObj();
        public int DataSetID { get; set; }

        public List<PingInfo> pingInfos = new List<PingInfo>();   
        
        public string Status { get => _monitorStatus.Message; set => _monitorStatus.Message = value; }
        public int DestinationUnreachable { get; set; }
        public int TimeOuts { get; set; }

        public string IPAddress { get; set; }
        public int PacketsRecieved { get; set; }
        public int PacketsLost { get; set; }
        public float PacketsLostPercentage { get; set; }
       
        public int RoundTripTimeMaximum { get; set; }
        public float RoundTripTimeAverage { get; set; }

        public int RoundTripTimeTotal { get; set; }

        public int PacketsSent { get; internal set; }
        public DateTime DateStarted { get => _dateStarted; set => _dateStarted = value; }
        public int RoundTripTimeMinimum { get => _roundTripTimeMinimum; set => _roundTripTimeMinimum = value; }
        public StatusObj MonitorStatus { get => _monitorStatus; set => _monitorStatus = value; }
    }
}
