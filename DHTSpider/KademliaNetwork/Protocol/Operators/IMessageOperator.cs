using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHTSpider.TaskServer.KademliaNetwork.Protocol.Messages;

namespace DHTSpider.TaskServer.KademliaNetwork.Protocol.Operators
{
    public interface IMessageOperator
    {
        void MessageHandle(IMessage msg);
    }
}
