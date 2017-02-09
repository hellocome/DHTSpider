using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHTSpider.TaskServer.KademliaNetwork.Protocol.Messages;

namespace DHTSpider.TaskServer.KademliaNetwork.Servers
{
    public interface IMessageDispatcher
    {
        void Send(IMessage msg);
    }
}
