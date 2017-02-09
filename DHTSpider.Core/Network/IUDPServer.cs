using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace DHTSpider.Core.Network
{
    public interface IUDPServer : IServer
    {
        int UdpPort
        {
            get;
        }
    }
}
