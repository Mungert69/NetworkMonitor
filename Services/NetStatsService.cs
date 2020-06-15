using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharpPcap;
using SharpPcap.Npcap;
using NetworkMonitor.Objects;

namespace NetworkMonitor.Services
{
    public class NetStatsService : INetStatsService, IDisposable
    {

        /// <summary>
        /// Stat collection capture example
        /// Npcap specific feature
        /// </summary>

        /// <summary>
        /// Stat collection capture example
        /// </summary>
        /// 
        private NpcapDevice _device;
        private List<NetStat> _netStatData = new List<NetStat>();
        private bool _statsOn;
        private bool _disable;


        public void resetData() {
            _netStatData = new List<NetStat>();
        }
        private void init(int deviceId)
        {

            // Retrieve the device list
            var devices = CaptureDeviceList.Instance;

            // If no devices were found print an error
            if (devices.Count < 1)
            {
                Console.WriteLine("No devices were found on this machine in NetStatsService.init()");
                return;
            }

            int i = deviceId;
            

            _device = devices[i] as NpcapDevice;

            // Register our handler function to the 'pcap statistics' event
            _device.OnPcapStatistics +=
                new SharpPcap.Npcap.StatisticsModeEventHandler(device_OnPcapStatistics);

            // Open the device for capturing
            _device.Open();

            // Handle TCP packets only
            _device.Filter = "tcp";

            // Set device to statistics mode
            _device.Mode = SharpPcap.Npcap.CaptureMode.Statistics;




        }

        public void start(int deviceId)
        {


            if (!_statsOn)
            {
                init(deviceId);
                Console.WriteLine();
                Console.WriteLine("-- Gathering statistics on \"{0}\", hit 'Enter' to stop...",
                    _device.Description);
                // Start the capturing process
                _device.StartCapture();
                _statsOn = true;

            }
        }

        public void stop()
        {

            if (_statsOn)
            {
                // Stop the capturing process
                _device.StopCapture();

                // Print out the device statistics
                Console.WriteLine(_device.Statistics.ToString());
                _device.Close();

                _statsOn = false;
            }
        }

        public void Dispose()
        {
            // Close the pcap device
            _device.Close();
        }


        ulong oldSec = 0;
        ulong oldUsec = 0;

        public List<NetStat> NetStatData { get => _netStatData; set => _netStatData = value; }

        /// <summary>
        /// Gets a pcap stat object and calculate bps and pps
        /// </summary>
        private void device_OnPcapStatistics(object sender, SharpPcap.Npcap.StatisticsModeEventArgs e)
        {
            // Calculate the delay in microseconds from the last sample.
            // This value is obtained from the timestamp that's associated with the sample.
            ulong delay = (e.Statistics.Timeval.Seconds - oldSec) * 1000000 - oldUsec + e.Statistics.Timeval.MicroSeconds;

            // Get the number of Bits per second
            ulong bps = 0;
            if (delay != 0)
            {


                bps = ((ulong)e.Statistics.RecievedBytes * 8 * 1000000) / delay;
            }

            /*                                       ^       ^
                                                     |       |
                                                     |       | 
                                                     |       |
                            converts bytes in bits --        |
                                                             |
                        delay is expressed in microseconds --
            */

            // Get the number of Packets per second
            ulong pps = 0;
            if (delay != 0)
            {
                pps = ((ulong)e.Statistics.RecievedPackets * 1000000) / delay;

            }
            // Convert the timestamp to readable format
            var ts = e.Statistics.Timeval.Date.ToLongTimeString();
            var ds = e.Statistics.Timeval.Date.ToLongDateString();

            // Print Statistics
            //Console.WriteLine("{0} {1}: bps={2}, pps={3}", ds, ts, bps, pps);
            NetStat netStat = new NetStat();
            netStat.ID = 0;
            netStat.statDate = e.Statistics.Timeval.Date;
            netStat.BPS = bps;
            netStat.PPS = pps;
            _netStatData.Add(netStat);

            //store current timestamp
            oldSec = e.Statistics.Timeval.Seconds;
            oldUsec = e.Statistics.Timeval.MicroSeconds;
        }
    }
}


