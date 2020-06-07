using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkMonitor.Objects
{
    public class NetStat
    {
        [Key]
        public int ID { get; set; }
        public DateTime statDate { get; set; }
        public ulong BPS { get; set; }
        public ulong PPS { get; set; }
    }
}
