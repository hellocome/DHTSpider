using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace DHTSpider.Core.Network
{
    public class SocketAsyncEventArgsPool
    {
        Queue<SocketAsyncEventArgs> m_pool;

        // Initializes the object pool to the specified size
        //
        // The "capacity" parameter is the maximum number of 
        // SocketAsyncEventArgs objects the pool can hold
        public SocketAsyncEventArgsPool(int capacity)
        {
            m_pool = new Queue<SocketAsyncEventArgs>(capacity);
        }

        // Add a SocketAsyncEventArg instance to the pool
        //
        // The "item" parameter is the SocketAsyncEventArgs instance 
        // to add to the pool
        public void Enqueue(SocketAsyncEventArgs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null");
            }

            lock (m_pool)
            {
                if (!m_pool.Contains(item))
                {
                    m_pool.Enqueue(item);
                }
            }
        }

        // Removes a SocketAsyncEventArgs instance from the pool
        // and returns the object removed from the pool
        public SocketAsyncEventArgs Dequeue()
        {
            lock (m_pool)
            {
                return m_pool.Dequeue();
            }
        }

        // The number of SocketAsyncEventArgs instances in the pool
        public int Count
        {
            get { return m_pool.Count; }
        }
    }
}
