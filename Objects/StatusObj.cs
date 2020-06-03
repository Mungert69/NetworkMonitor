﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkMonitor.Objects
{
    public class StatusObj
    {
        private int iD;
        private DateTime eventTime;
        private string message;
        private bool isUp;
        private int downCount;
        private bool alertFlag;
        private bool alertSent;

        public bool IsUp { get => isUp; set => isUp = value; }
        public int DownCount { get => downCount; set => downCount = value; }
        public bool AlertFlag { get => alertFlag; set => alertFlag = value; }
        public bool AlertSent { get => alertSent; set => alertSent = value; }
        public string Message { get => message; set => message = value; }
        [Key]
        public int ID { get => iD; set => iD = value; }
        public DateTime EventTime { get => eventTime; set => eventTime = value; }
    }
}
