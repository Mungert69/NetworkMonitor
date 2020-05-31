using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkMonitor.Objects
{
    public class PingParams

    {

        private  int _bufferLength;
        private int _timeOut;
        private  string _schedule;
        private int _pingBurstNumber;
        private int _pingBurstDelay;

        public int BufferLength { get => _bufferLength; set => _bufferLength = value; }
        public int TimeOut { get => _timeOut; set => _timeOut = value; }
        public string Schedule { get => _schedule; set => _schedule = value; }
        public int PingBurstNumber { get => _pingBurstNumber; set => _pingBurstNumber = value; }
        public int PingBurstDelay { get => _pingBurstDelay; set => _pingBurstDelay = value; }
    }
}
