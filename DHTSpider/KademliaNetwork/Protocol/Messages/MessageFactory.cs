using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHTSpider.TaskServer.KademliaNetwork.Protocol.Messages
{
    public class MessageFactory
    {
        public static IMessage CreateMessage(ProtocolSource source, object endPoint, byte[] data, int index, int count)
        {
            if (data == null)
            {
                return null;
            }
            else
            {
                return new RawMessage(source, MessageType.UNKNOWN, endPoint, data, index, count);
            }
        }
    }
}
