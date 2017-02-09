using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHTSpider.TaskServer.KademliaNetwork.Protocol.Messages
{
    public interface IMessage : IDisposable
    {
        object EndPoint
        {
            get;
            set;
        }

        ProtocolSource ProtocolSource
        {
            get;
        }

        MessageType MessageType
        {
            get;
        }

        byte[] Content
        {
            get;
        }

        void BuildMessage();
    }
}
