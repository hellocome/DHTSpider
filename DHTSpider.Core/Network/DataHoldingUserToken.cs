using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHTSpider.Core.Network
{
    public class DataHoldingUserToken
    {
        public Guid DataHoldingUserTokenGUID
        {
            get;
            private set;
        }

        public int ReceiveOffset
        {
            get;
            private set;
        }
        public int SendOffset
        {
            get;
            private set;
        }

        public int ReceiveBufferSize
        {
            get;
            private set;
        }

        public int SendBufferSize
        {
            get;
            private set;
        }


        public int SendSize
        {
            get;
            set;
        }

        public void ResetReceivedCount()
        {
            RemainingReceiveDataCount = 0;
            ProcessedReceiveDataOffset = ReceiveOffset;
        }

        public void ResetSendSize()
        {
            SendSize = 0;
        }

        public int ProcessedReceiveDataOffset
        {
            get;
            set;
        }

        public int RemainingReceiveDataCount
        {
            get;
            set;
        }

        public int RemainingReceiveDataBufferLength
        {
            get
            {
                return ReceiveBufferSize - RemainingReceiveDataCount;
            }
        }

        public DataHoldingUserToken(int receiveOffset, int receiveBufferSize, int sendOffset, int sendBufferSize)
        {
            DataHoldingUserTokenGUID = Guid.NewGuid();
            ReceiveOffset = receiveOffset;
            SendOffset = sendOffset;

            SendBufferSize = sendBufferSize;
            ReceiveBufferSize = receiveBufferSize;

            ProcessedReceiveDataOffset = ReceiveOffset;
        }
    }
}
