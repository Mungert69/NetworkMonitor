using System;

namespace NetworkMonitor
{
    public class PingInfo
    {
        public DateTime DateSent { get; set; }

        public string Status { get; set; }
     
        public int RoundTripTime { get; set; }
       }
}
