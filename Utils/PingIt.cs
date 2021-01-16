using NetworkMonitor.Objects;
using System;
using System.Net.NetworkInformation;
using System.Threading;

namespace NetworkMonitor.Utils
{
    public class PingIt
    {
        private MonitorPingInfo _monitorPingInfo;
        private PingParams _pingParams;
        private int _timeout;
        private int _roundTrip;

        public int RoundTrip { get => _roundTrip; set => _roundTrip = value; }

        public PingIt(MonitorPingInfo pingInfo, PingParams pingParams)
        {
            _monitorPingInfo = pingInfo;
            _pingParams = pingParams;
            pingInfo.PacketsSent++;

        }


        public void go()
        {
            string who = _monitorPingInfo.IPAddress;
            AutoResetEvent waiter = new AutoResetEvent(false);

            Ping pingSender = new Ping();

            // When the PingCompleted event is raised,
            // the PingCompletedCallback method is called.
            pingSender.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);

            // Create a buffer of x bytes of data to be transmitted.

            byte[] buffer = new byte[_pingParams.BufferLength];
            new Random().NextBytes(buffer);


            // Wait 5 seconds for a reply.
            _timeout = _pingParams.TimeOut;

            // Set options for transmission:
            // The data can go through 64 gateways or routers
            // before it is destroyed, and the data packet
            // cannot be fragmented.
            PingOptions options = new PingOptions(64, true);

            // Send the ping asynchronously.
            // Use the waiter as the user token.
            // When the callback completes, it can wake up this thread.
            pingSender.SendAsync(who, _timeout, buffer, options, waiter);

            // Prevent this example application from ending.
            // A real application should do something useful
            // when possible.
            waiter.WaitOne();
        }

        private void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            // If the operation was canceled, display a message to the user.
            if (e.Cancelled)
            {
                _monitorPingInfo.MonitorStatus.Message = "Ping canceled.";
                _monitorPingInfo.MonitorStatus.IsUp = false;
                _monitorPingInfo.MonitorStatus.DownCount++;
                _monitorPingInfo.MonitorStatus.EventTime = DateTime.Now;
                _monitorPingInfo.PacketsLost++;
                // Let the main thread resume.
                // UserToken is the AutoResetEvent object that the main thread
                // is waiting for.
                ((AutoResetEvent)e.UserState).Set();
            }

            // If an error occurred, display the exception to the user.
            if (e.Error != null)
            {
                _monitorPingInfo.MonitorStatus.Message = "Ping failed:" + e.Error.ToString();
                _monitorPingInfo.MonitorStatus.IsUp = false;
                _monitorPingInfo.MonitorStatus.DownCount++;
                _monitorPingInfo.MonitorStatus.EventTime = DateTime.Now;
                _monitorPingInfo.PacketsLost++;
                // Let the main thread resume.
                ((AutoResetEvent)e.UserState).Set();
            }

            PingReply reply = e.Reply;

            DisplayReply(reply);

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();
        }

        public void DisplayReply(PingReply reply)
        {

            PingInfo pingInfo = new PingInfo();
            _roundTrip = -20;
            pingInfo.DateSent = DateTime.Now;
            pingInfo.Status = reply.Status.ToString();

            _monitorPingInfo.MonitorStatus.Message = reply.Status.ToString();
            if (reply.Status == IPStatus.Success)
            {
                _monitorPingInfo.PacketsRecieved++;
                _roundTrip = (int)reply.RoundtripTime;
                pingInfo.RoundTripTime = _roundTrip;
                _monitorPingInfo.MonitorStatus.IsUp = true;
                _monitorPingInfo.MonitorStatus.DownCount = 0;



                if (_monitorPingInfo.RoundTripTimeMaximum < _roundTrip)
                {
                    _monitorPingInfo.RoundTripTimeMaximum = _roundTrip;
                }

                if (_monitorPingInfo.RoundTripTimeMinimum > _roundTrip)
                {
                    _monitorPingInfo.RoundTripTimeMinimum = _roundTrip;
                }

                _monitorPingInfo.RoundTripTimeTotal += _roundTrip;
                _monitorPingInfo.RoundTripTimeAverage = _monitorPingInfo.RoundTripTimeTotal / (float)_monitorPingInfo.PacketsRecieved;
            }
            else
            {
                _monitorPingInfo.PacketsLost++;
                _monitorPingInfo.MonitorStatus.IsUp = false;
                _monitorPingInfo.MonitorStatus.DownCount++;
                _monitorPingInfo.MonitorStatus.EventTime = pingInfo.DateSent;
                pingInfo.RoundTripTime = -20;
            }
            _monitorPingInfo.PacketsLostPercentage = _monitorPingInfo.PacketsLost * (float)100 / _monitorPingInfo.PacketsSent;
            if (reply.Status == IPStatus.TimedOut)
            {
                _monitorPingInfo.TimeOuts++;
            }

            if (reply.Status == IPStatus.DestinationHostUnreachable)
            {
                _monitorPingInfo.DestinationUnreachable++;
            }

            _monitorPingInfo.pingInfos.Add(pingInfo);

        }
    }
}
