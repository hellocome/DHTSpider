using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHTSpider.Core.Network;

using DHTSpider.TaskServer.KademliaNetwork.Servers;

namespace DHTSpider.TaskServer.KademliaNetwork.Protocol.Messages
{
    public class MessageOutProcessor
    {
        private MessageQueueProcessor messageQueueProcessor = new MessageQueueProcessor();

        private static MessageOutProcessor instance = new MessageOutProcessor();

        public static MessageOutProcessor Instance
        {
            get
            {
                return instance;
            }
        }

        private Dictionary<ProtocolSource, IMessageDispatcher> dispatchers = new Dictionary<ProtocolSource, IMessageDispatcher>();

        private MessageOutProcessor()
        {
            messageQueueProcessor.InitializeQueueProcessor(MessageHandle);
        }

        public void AddMessageDispatcher(ProtocolSource source, IMessageDispatcher dispatcher)
        {
            lock (dispatchers)
            {
                if (!dispatchers.ContainsKey(source))
                {
                    dispatchers.Add(source, dispatcher);
                }
            }
        }

        public void RemoveAllMessageDispatcher()
        {
            lock (dispatchers)
            {
                dispatchers.Clear();
            }
        }

        public void RemoveMessageDispatcher(ProtocolSource source)
        {
            lock (dispatchers)
            {
                if (dispatchers.ContainsKey(source))
                {
                    dispatchers.Remove(source);
                }
            }
        }
        

        public void Start()
        {
            messageQueueProcessor.Start();
        }

        public void Stop()
        {
            messageQueueProcessor.Stop();
        }

        public void Enqueue(IMessage msg)
        {
            messageQueueProcessor.Enqueue(msg);
        }

        private void MessageHandle(IMessage msg)
        {
            if (msg != null)
            {
                switch (msg.ProtocolSource)
                {
                    case ProtocolSource.BIT_TORRENT_KRPC:
                        dispatchers[ProtocolSource.BIT_TORRENT_KRPC].Send(msg);
                        break;
                }
            }
        }
    }
}
