using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharpPcap;
using SharpPcap.Npcap;
using NetworkMonitor.Objects;

namespace NetworkMonitor.Services
{
    public class NetStatsService : INetStatsService
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
        public void init(int deviceId)
        {

            // Retrieve the device list
            var devices = CaptureDeviceList.Instance;

            // If no devices were found print an error
            if (devices.Count < 1)
            {
                Console.WriteLine("No devices were found on this machine");
                return;
            }

            int i = deviceId;
            // ToDo find out how to get the right device id

            /*
            Console.WriteLine();
                Console.WriteLine("The following devices are available on this machine:");
                Console.WriteLine("----------------------------------------------------");
                Console.WriteLine();

              
           

            //Print out the available devices
            foreach (var dev in devices)
                {
                    Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Description);
                    i++;
                }

                
                i = int.Parse(Console.ReadLine());
            */

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

        public void start()
        {
            Console.WriteLine();
            Console.WriteLine("-- Gathering statistics on \"{0}\", hit 'Enter' to stop...",
                _device.Description);
            // Start the capturing process
            _device.StartCapture();
        }
        public void stop()
        {
            // Stop the capturing process
            _device.StopCapture();

            // Print out the device statistics
            Console.WriteLine(_device.Statistics.ToString());

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
            ulong bps = ((ulong)e.Statistics.RecievedBytes * 8 * 1000000) / delay;
            /*                                       ^       ^
                                                     |       |
                                                     |       | 
                                                     |       |
                            converts bytes in bits --        |
                                                             |
                        delay is expressed in microseconds --
            */

            // Get the number of Packets per second
            ulong pps = ((ulong)e.Statistics.RecievedPackets * 1000000) / delay;

            // Convert the timestamp to readable format
            var ts = e.Statistics.Timeval.Date.ToLongTimeString();
            var ds = e.Statistics.Timeval.Date.ToLongDateString();

            // Print Statistics
            Console.WriteLine("{0} {1}: bps={2}, pps={3}", ds, ts, bps, pps);
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


