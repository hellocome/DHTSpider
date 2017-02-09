using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using DHTSpider.Core.Logging;
using DHTSpider.TaskServer.KademliaNetwork.Servers.ServerSettings;
using DHTSpider.TaskServer.KademliaNetwork.Protocol;

namespace DHTSpider.TaskServer.KademliaNetwork.Servers
{
    public sealed class BitTorrentKRPCUDPServer : KademliaUDPServer
    {
        public BitTorrentKRPCUDPServer(BitTorrentKRPCUDPServerSetting setting) : base(setting)
        {
            ProtocolSource = ProtocolSource.BIT_TORRENT_KRPC;

            RoutingTableFactory.Instance.CreateRoutingTable(setting.NodeID, ProtocolSource.BIT_TORRENT_KRPC);
        }
    }
}
