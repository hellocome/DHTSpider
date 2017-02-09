using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHTSpider.TaskServer.KademliaNetwork.Protocol;

namespace DHTSpider.TaskServer.KademliaNetwork
{
    public interface IKademliaPeer
    {
        ID NodeID { get; }
    }
}
