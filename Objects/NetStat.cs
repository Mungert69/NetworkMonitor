﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetworkMonitor.Objects
{
    public class NetStat
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int DataSetID { get; set; }
        public DateTime statDate { get; set; }
        public ulong BPS { get; set; }
        public ulong PPS { get; set; }
    }
}
