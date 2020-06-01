using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkMonitor.Objects
{
    public class ResultObj
    {

        private string message;
        private bool success;

        public string Message { get => message; set => message = value; }
        public bool Success { get => success; set => success = value; }
    }
}
