using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkMonitor.Objects
{
    public class DataSetObj
    {

        private int dataSetId;
        private DateTime dateStarted;

        public int DataSetId { get => dataSetId; set => dataSetId = value; }
        public DateTime DateStarted { get => dateStarted; set => dateStarted = value; }
    }
}
