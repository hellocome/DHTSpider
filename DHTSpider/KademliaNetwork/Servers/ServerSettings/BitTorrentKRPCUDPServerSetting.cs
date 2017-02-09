using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHTSpider.TaskServer.KademliaNetwork.Servers.ServerSettings
{
    public class BitTorrentKRPCUDPServerSetting : KademliaUDPServerSetting
    {
        public static BitTorrentKRPCUDPServerSetting Default = new BitTorrentKRPCUDPServerSetting();

        public BitTorrentKRPCUDPServerSetting()
        {
            UdpPort = 7758;
            NodeID = DefaultSettings.KADEMLIA_SERVER_ID;
        }
    }
}
