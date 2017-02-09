using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHTSpider.TaskServer.KademliaNetwork.Protocol;

namespace DHTSpider.TaskServer.KademliaNetwork.Servers.ServerSettings
{
    public class KademliaUDPServerSetting : DHTSpider.Core.Network.IUDPServerSetting
    {
        public ID NodeID
        {
            get;
            set;
        }


        public int UdpPort
        {
            get;
            protected set;
        }
    }
}
