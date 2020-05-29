using NetworkMonitor.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkMonitor.Utils
{
    public class PingIt
    {
        private MonitorPingInfo _monitorPingInfo;
        public PingIt(MonitorPingInfo pingInfo)
        {
            _monitorPingInfo = pingInfo;
            pingInfo.PacketsSent++;
           
        }

       
        public  void go()
        {
              string who = _monitorPingInfo.IPAddress;
            AutoResetEvent waiter = new AutoResetEvent(false);

            Ping pingSender = new Ping();

            // When the PingCompleted event is raised,
            // the PingCompletedCallback method is called.
            pingSender.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            // Wait 5 seconds for a reply.
            int timeout = 5000;

            // Set options for transmission:
            // The data can go through 64 gateways or routers
            // before it is destroyed, and the data packet
            // cannot be fragmented.
            PingOptions options = new PingOptions(64, true);

            // Send the ping asynchronously.
            // Use the waiter as the user token.
            // When the callback completes, it can wake up this thread.
            pingSender.SendAsync(who, timeout, buffer, options, waiter);

            // Prevent this example application from ending.
            // A real application should do something useful
            // when possible.
            waiter.WaitOne();
            Console.WriteLine("Ping example completed.");
        }

        private  void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            // If the operation was canceled, display a message to the user.
            if (e.Cancelled)
            {
                _monitorPingInfo.Status = "Ping canceled.";
                _monitorPingInfo.PacketsLost++;
                // Let the main thread resume.
                // UserToken is the AutoResetEvent object that the main thread
                // is waiting for.
                ((AutoResetEvent)e.UserState).Set();
            }

            // If an error occurred, display the exception to the user.
            if (e.Error != null)
            {
                _monitorPingInfo.Status = "Ping failed:"+ e.Error.ToString();
                _monitorPingInfo.PacketsLost++;
                // Let the main thread resume.
                ((AutoResetEvent)e.UserState).Set();
            }

            PingReply reply = e.Reply;

            DisplayReply(reply);

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();
        }

        public  void DisplayReply(PingReply reply)
        {
            if (reply == null)
                return;

            _monitorPingInfo.Status = reply.Status.ToString() ;
            if (reply.Status == IPStatus.Success)
            {
                _monitorPingInfo.PacketsRecieved++;
                int roundTripTime = (int)reply.RoundtripTime;
                PingInfo pingInfo = new PingInfo();
                pingInfo.RoundTripTime = roundTripTime;

                _monitorPingInfo.pingInfos.Add(pingInfo);
                //_pingInfo.RoundTripTime = roundTripTime;
                if (_monitorPingInfo.RoundTripTimeMaximum < roundTripTime) _monitorPingInfo.RoundTripTimeMaximum = roundTripTime;
                if (_monitorPingInfo.RoundTripTimeMinimum > roundTripTime) _monitorPingInfo.RoundTripTimeMinimum = roundTripTime;
                _monitorPingInfo.RoundTripTimeTotal += roundTripTime;
                _monitorPingInfo.RoundTripTimeAverage = (float)_monitorPingInfo.RoundTripTimeTotal / (float)_monitorPingInfo.PacketsRecieved;
            }
            else { 
                _monitorPingInfo.PacketsLost++;
            }
            if (reply.Status == IPStatus.TimedOut) _monitorPingInfo.TimeOuts++;
            if (reply.Status == IPStatus.DestinationHostUnreachable) _monitorPingInfo.DestinationUnreachable++;


        }
    }
}
