using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHTSpider.TaskServer.KademliaNetwork.Protocol.Messages
{
    public enum MessageType
    {
        PING_REQUEST = 0x00,
        PING_RESPONSE = 0x01,

        FIND_NODE_REQUEST = 0x02,
        FIND_NODE_RESPONSE = 0x03,

        FIND_VALUE_REQUEST = 0x04,
        FIND_VALUE_RESPONSE = 0x05,

        STORE_REQUEST = 0x06,
        STORE_RESPONSE = 0x07,
        UNKNOWN = 0X08
    }
}
