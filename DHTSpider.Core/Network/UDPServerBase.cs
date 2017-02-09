using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using DHTSpider.Core.Logging;

namespace DHTSpider.Core.Network
{
    public abstract class UDPServerBase : IUDPServer
    {
        protected bool disposed = false;
        private byte[] udpBuffer = new byte[64 * 1024]; // 64kb

        protected Socket udpListenSocket = null;
        protected UdpClient udpClient = null;

        protected abstract void OnReceiveUDP(int num_bytes, byte[] buf, IPEndPoint ip);

        protected virtual void OnSendUDP(IPEndPoint clientIP, int num_bytes) { }

        protected virtual void OnSendUDPError(UDPServerArgs args, Exception ex) { }

        public int UdpPort
        {
            get;
            private set;
        }

        public IUDPServerSetting ServerSetting
        {
            get;
            private set;
        }

        public UDPServerBase(IUDPServerSetting setting)
        {
            ServerSetting = setting;
        }

        public void InitServer()
        {
            UdpPort = ServerSetting.UdpPort;
        }

        public bool Start()
        {
            IPEndPoint udpEndpoint = new IPEndPoint( IPAddress.Any, UdpPort);

            udpListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpListenSocket.Bind(udpEndpoint);

            udpClient = new UdpClient();

            StartReceivingUdp(null);

            return true;
        }

        private void ProcessUdpReceive(SocketAsyncEventArgs args)
        {
            try
            {
                int num_bytes = args.BytesTransferred;

                // EndPoint ip = new IPEndPoint(IPAddress.Any, 0);
                EndPoint ip = args.RemoteEndPoint;

                OnReceiveUDP(num_bytes, udpBuffer, ip as IPEndPoint);

                StartReceivingUdp(args);
            }
            catch (ObjectDisposedException)
            {

            }
            catch (SocketException socketEx)
            {
                Logging.Logger.Instance.Error(socketEx.Message);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.Error(ex.Message);
            }
        }

        protected void UdpRecvEventCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessUdpReceive(e);
        }

        protected void StartReceivingUdp(SocketAsyncEventArgs args)
        {
            if (args == null)
            {
                args = new SocketAsyncEventArgs();
                args.Completed += UdpRecvEventCompleted;
                args.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                args.SetBuffer(udpBuffer, 0, udpBuffer.Length);
            }

            bool willRaiseEvent = udpListenSocket.ReceiveAsync(args);

            if (!willRaiseEvent)
            {
                ProcessUdpReceive(args);
            }
        }

        private static void SendToCallback(IAsyncResult ar)
        {
            UDPServerArgs args = ar.AsyncState as UDPServerArgs;

            try
            {
                if (args != null)
                {
                    int num_bytes = args.Server.udpClient.EndSend(ar);
                    args.Server.OnSendUDP(args.ClientIP, num_bytes);
                }
            }
            catch (Exception e)
            {
                if (args != null)
                    args.Server.OnSendUDPError(args, e);
            }
        }

        public void SendUDP(IPEndPoint client, byte[] buf)
        {
            if (udpClient != null)
            {
                udpClient.BeginSend(buf, buf.Length, client, SendToCallback, new UDPServerArgs(this, client));
            }
        }


        public bool Stop()
        {
            return true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if(disposing)
                {

                }

                disposed = true;
            }
        }
    }
}
