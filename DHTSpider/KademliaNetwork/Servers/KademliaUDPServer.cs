using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using DHTSpider.Core.Logging;
using DHTSpider.Core.Network;
using DHTSpider.TaskServer.KademliaNetwork.Protocol.Messages;
using DHTSpider.TaskServer.KademliaNetwork.Protocol;
using DHTSpider.TaskServer.KademliaNetwork.Servers.ServerSettings;

namespace DHTSpider.TaskServer.KademliaNetwork.Servers
{
    public abstract class KademliaUDPServer : UDPServerBase, IMessageDispatcher, IKademliaPeer
    {
        public ProtocolSource ProtocolSource
        {
            get;
            set;
        }

        public ID NodeID
        {
            get;
            set;
        }

        public KademliaUDPServer(KademliaUDPServerSetting setting) : base(setting)
        {
            NodeID = setting.NodeID;
        }

        protected override void OnReceiveUDP(int num_bytes, byte[] buf, IPEndPoint ip)
        {
            IMessage message = MessageFactory.CreateMessage(ProtocolSource, ip, buf, 0, num_bytes);
            MessageInProcessor.Instance.Enqueue(message);
        }

        public void Send(IMessage msg)
        {
            SendUDP(msg.EndPoint as IPEndPoint, msg.Content);
        }
    }
}
