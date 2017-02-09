using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHTSpider.TaskServer.KademliaNetwork.Protocol.Messages;
using DHTSpider.TaskServer.KademliaNetwork.Protocol.Operators.BitTorrent;
using DHTSpider.TaskServer.KademliaNetwork.Servers;

namespace DHTSpider.TaskServer.KademliaNetwork.Protocol.Operators
{
    public class MessageOperator : IMessageOperator
    {
        private BitTorrentKRPCMessageOperator biKRPCOperator;

        private static MessageOperator instance = new MessageOperator();

        public static MessageOperator Instance
        {
            get
            {
                return instance;
            }
        }



        private MessageOperator()
        {
        }

        public void RegistryPeer(ProtocolSource source, IKademliaPeer peer)
        {
            switch (source)
            {
                case ProtocolSource.BIT_TORRENT_KRPC:
                    biKRPCOperator = new BitTorrentKRPCMessageOperator(peer);
                    break;
            }
        }

        public void BootStrap(ProtocolSource source)
        {
            switch (source)
            {
                case ProtocolSource.BIT_TORRENT_KRPC:
                    biKRPCOperator.BootStrap();
                    break;
            }
        }

        public void MessageHandle(IMessage msg)
        {
            if(msg != null)
            {
                switch(msg.ProtocolSource)
                {
                    case ProtocolSource.BIT_TORRENT_KRPC:
                        biKRPCOperator.HandleMessage(msg);
                        break;
                }
            } 
        }

        public void PingRequestHandle(ProtocolSource source, Contact contact)
        {
            if (contact != null)
            {
                switch (source)
                {
                    case ProtocolSource.BIT_TORRENT_KRPC:
                        biKRPCOperator.PingRequestHandle(source, contact);
                        break;
                }
            }
        }
    }
}
