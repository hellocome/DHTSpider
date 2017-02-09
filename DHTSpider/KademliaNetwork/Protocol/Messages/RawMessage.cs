using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DHTSpider.TaskServer.KademliaNetwork.Protocol.Messages
{
    public class RawMessage : IMessage
    {
        protected bool disposed = false;
        protected bool ensureMessage = false;

        public object EndPoint
        {
            get;
            set;
        }

        public MessageType MessageType
        {
            get;
            protected set;
        }

        public ProtocolSource ProtocolSource
        {
            get;
            protected set;
        }

        public byte[] Content
        {
            get;
            protected set;
        }

        public RawMessage(ProtocolSource source, MessageType msgType, object endPoint, byte[] buff, int index, int count)
        {
            MessageType = msgType;
            ProtocolSource = source;
            EndPoint = endPoint;
            InitMessage(buff, index, count);
        }

        public RawMessage(ProtocolSource source, MessageType msgType, object endPoint, byte[] buff)
        {
            MessageType = msgType;
            ProtocolSource = source;
            EndPoint = endPoint;
            InitMessage(buff, 0, buff.Length);
        }

        public RawMessage(ProtocolSource source, object endPoint, byte[] buff)
        {
            MessageType = MessageType.UNKNOWN;
            ProtocolSource = source;
            EndPoint = endPoint;
            InitMessage(buff, 0, buff.Length);
        }

        private void InitMessage(byte[] buff, int index, int count)
        {
            if (buff == null || buff.Length < index || buff.Length - index < count)
            {
                ensureMessage = false;
            }
            else
            {
                byte[] cloneBuff = new byte[count];
                Array.Copy(buff, index, cloneBuff, 0, count);

                Content = cloneBuff;
            }
        }

        protected bool EnsureMessage()
        {
            return ensureMessage;
        }

        public virtual void BuildMessage() { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if(disposing)
                {

                }
            }
        }
    }
}
