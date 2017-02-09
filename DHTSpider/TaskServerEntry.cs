using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHTSpider.TaskServer.KademliaNetwork.Protocol.Messages;
using DHTSpider.TaskServer.KademliaNetwork.Servers;
using DHTSpider.Core.Network;
using DHTSpider.TaskServer.KademliaNetwork.Protocol;
using DHTSpider.TaskServer.KademliaNetwork.Servers.ServerSettings;
using DHTSpider.TaskServer.KademliaNetwork.Protocol.Operators;

namespace DHTSpider.TaskServer
{
    public class TaskServerEntry
    {
        private static TaskServerEntry instance = new TaskServerEntry();
        public static TaskServerEntry Instance
        {
            get
            {
                return instance;
            }
        }


        BitTorrentKRPCUDPServer btKRPCServer = null;


        private TaskServerEntry()
        {
            Init();
        }

        private void Init()
        {
            btKRPCServer = new BitTorrentKRPCUDPServer(BitTorrentKRPCUDPServerSetting.Default);

            MessageOperator.Instance.RegistryPeer(ProtocolSource.BIT_TORRENT_KRPC, btKRPCServer);
            MessageOutProcessor.Instance.AddMessageDispatcher(ProtocolSource.BIT_TORRENT_KRPC, btKRPCServer);
        }

        public void Start()
        {
            MessageOutProcessor.Instance.Start();
            MessageInProcessor.Instance.Start();

            btKRPCServer.Start();

            MessageOperator.Instance.BootStrap(ProtocolSource.BIT_TORRENT_KRPC);
        }

        public void Stop()
        {
            btKRPCServer.Stop();

            MessageOutProcessor.Instance.Stop();
            MessageInProcessor.Instance.Stop();
        }
    }
}
