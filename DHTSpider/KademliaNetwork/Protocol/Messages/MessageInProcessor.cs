using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHTSpider.TaskServer.KademliaNetwork.Protocol.Operators;

namespace DHTSpider.TaskServer.KademliaNetwork.Protocol.Messages
{
    public class MessageInProcessor
    {

        private MessageQueueProcessor messageQueueProcessor = new MessageQueueProcessor();

        private static MessageInProcessor instance = new MessageInProcessor();

        public static MessageInProcessor Instance
        {
            get
            {
                return instance;
            }
        }

        private MessageInProcessor()
        {
            messageQueueProcessor.InitializeQueueProcessor(MessageOperator.Instance.MessageHandle);
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
        
    }
}
