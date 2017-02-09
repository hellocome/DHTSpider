using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace DHTSpider.Core.Network
{
    public class UDPServerArgs
    {
        public UDPServerBase Server
        {
            get;
            private set;
        }

        public IPEndPoint ClientIP
        {
            get;
            private set;
        }

        public UDPServerArgs(UDPServerBase srvr, IPEndPoint client)
        {
            Server = srvr;
            ClientIP = client;
        }
    }
}
