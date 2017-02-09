using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DHTSpider.Core.Logging;
using DHTSpider.Core.Common;

namespace DHTSpider.TaskServer.KademliaNetwork.Protocol.Messages
{
    public class MessageQueueProcessor : TaskQueueProcessor<T>, IMessageQueue
    {
        private List<Task> tasks = new List<Task>();
        private Action<IMessage> MessageHandle;
        private Queue<IMessage> msgQueue = new Queue<IMessage>();
        private AutoResetEvent mutex = new AutoResetEvent(false);
        private volatile bool run = false;
        private volatile bool initialized = false;

        public bool Runing
        {
            get
            {
                return run;
            }
        }

        public void Enqueue(IMessage msg)
        {
            if (msg != null)
            {
                lock (msgQueue)
                {
                    msgQueue.Enqueue(msg);
                    mutex.Set();
                }
            }
        }

        public IMessage Dequeue()
        {
            lock (msgQueue)
            {
                if (msgQueue.Count > 0)
                {
                    return msgQueue.Dequeue();
                }

                return null;
            }
        }

        public MessageQueueProcessor()
        {
        }

        public void InitializeQueueProcessor(Action<IMessage> messageHandle, int taskNum = 5)
        {
            MessageHandle = messageHandle;

            for (int i = 0; i < taskNum; i++)
            {
                tasks.Add(new Task(Run));
            }

            initialized = true;
        }

        private void Run()
        {
            while(run)
            {
                mutex.WaitOne();

                IMessage msg = msgQueue.Dequeue();

                if (MessageHandle != null)
                {
                    try
                    {
                        MessageHandle(msg);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Error(ex.Message);
                    }
                }
            }
        }

        public void Start()
        {
            if(!initialized)
            {
                throw new InvalidOperationException("Message Queue Processor is not initialized!");
            }

            run = true;

            foreach (Task task in tasks)
            {
                task.Start();
            }
        }

        public void Stop()
        {
            run = false;

            Task.WaitAll(tasks.ToArray());
        }
    }
}
