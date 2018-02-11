using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PrimS.Telnet;

namespace SWH.SmartWaterRelay
{
    public class SmartSensorManager
    {
        public event StatusUpdateEvent StatusUpdate;
        public bool Connected => _commClient?.IsConnected ?? false;

        private Client _commClient;

        private CancellationToken _connCancel = new CancellationToken();

        public SmartSensorManager()
        {
            Connect();
        }

        private void Connect()
        {
            _commClient?.Dispose();
            _commClient = new Client("192.168.25.139", 23, _connCancel);
        }

        public async Task<bool> GetStatus()
        {
            try
            {
                if (!Connected)
                    Connect();

                _commClient.WriteLine("GO");

                var s = await _commClient.TerminatedReadAsync("}", TimeSpan.FromMilliseconds(15000));

                if (!string.IsNullOrEmpty(s))
                {
                    OnStatusUpdate(JsonConvert.DeserializeObject<StatusReport>(s));
                    return true;
                }
            }
            catch (Exception e)
            {
            }

            return false;
        }

        public async Task<bool> SetTemp(int newTemp)
        {
            try
            {
                if (!Connected)
                    Connect();

                _commClient.WriteLine($"SET {newTemp.ToString("00#")}");

                var s = await _commClient.TerminatedReadAsync("}", TimeSpan.FromMilliseconds(15000));

                if (!string.IsNullOrEmpty(s))
                {
                    OnStatusUpdate(JsonConvert.DeserializeObject<StatusReport>(s));
                    return true;
                }

            }
            catch (Exception e)
            {
            }

            return false;
        }

        public async Task<bool> ZeroStats()
        {
            try
            {
                if (!Connected)
                    Connect();

                _commClient.WriteLine("ZERO");

                var s = await _commClient.TerminatedReadAsync("}", TimeSpan.FromMilliseconds(15000));

                if (!string.IsNullOrEmpty(s))
                {
                    OnStatusUpdate(JsonConvert.DeserializeObject<StatusReport>(s));
                    return true;
                }
            }
            catch (Exception e)
            {
            }

            return false;
        }

        protected virtual void OnStatusUpdate(StatusReport report)
        {
            StatusUpdate?.Invoke(this, new StatusUpdateEventArgs(report));
        }
    }
}
