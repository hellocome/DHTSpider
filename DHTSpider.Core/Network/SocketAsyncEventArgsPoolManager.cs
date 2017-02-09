using System;
using System.Net.Sockets;

namespace DHTSpider.Core.Network
{
    public class SocketAsyncEventArgsPoolManager
    {
        private static int SAFE_SIZE = 10;

        private SocketAsyncEventArgsPool readWritePool;
        private BufferManager bufferManager;

        public int MaxConnections
        {
            get;
            private set;
        }

        public int OpsToPreAllocate
        {
            get;
            private set;
        }

        public int BufferSize
        {
            get;
            private set;
        }

        public int BufferAllocateForSocketAsyncEventArgs
        {
            get;
            private set;
        }

        private int MaxConnections_SAFE_SIZE = 0;


        // 1024 * 1024 * 1024 * 2
        // 2GB max-size-per-object limit, don't think we will exceed this
        public SocketAsyncEventArgsPoolManager(int bufferSize, int maxConnections)
        {
            MaxConnections = maxConnections;
            MaxConnections_SAFE_SIZE = MaxConnections + SAFE_SIZE;

            // Always 2
            OpsToPreAllocate = 2;
            BufferSize = bufferSize;

            BufferAllocateForSocketAsyncEventArgs = BufferSize * OpsToPreAllocate;

            readWritePool = new SocketAsyncEventArgsPool(MaxConnections_SAFE_SIZE);
            bufferManager = new BufferManager(BufferSize * MaxConnections_SAFE_SIZE * OpsToPreAllocate, BufferAllocateForSocketAsyncEventArgs);
        }

        public virtual void Init()
        {
            bufferManager.InitBuffer();

            // preallocate pool of SocketAsyncEventArgs objects
            SocketAsyncEventArgs readWriteEventArg;

            for (int i = 0; i < MaxConnections_SAFE_SIZE; i++)
            {
                //Pre-allocate a set of reusable SocketAsyncEventArgs
                readWriteEventArg = new SocketAsyncEventArgs();

                // assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
                bufferManager.SetBuffer(readWriteEventArg);

                readWriteEventArg.UserToken = new DataHoldingUserToken(readWriteEventArg.Offset,
                                                                        BufferSize,
                                                                        readWriteEventArg.Offset + BufferSize,
                                                                        BufferSize);

                // add SocketAsyncEventArg to the pool
                readWritePool.Enqueue(readWriteEventArg);
            }
        }

        public SocketAsyncEventArgs AcquirerSocketAsyncEventArgs()
        {
            return readWritePool.Dequeue();
        }

        public void ReleaseSocketAsyncEventArgs(SocketAsyncEventArgs arg)
        {
            arg.AcceptSocket = null;
            DataHoldingUserToken userToken = (DataHoldingUserToken)arg.UserToken;
            userToken.ResetReceivedCount();
            userToken.ResetSendSize();

            readWritePool.Enqueue(arg);
        }
    }
}
