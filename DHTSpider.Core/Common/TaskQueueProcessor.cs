using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DHTSpider.Core.Logging;

namespace DHTSpider.Core.Common
{
    public class TaskQueueProcessor<T> : IMessageQueueProcessor<T> where T : class
    {
        protected List<Task> tasks = new List<Task>();
        protected Action<T> Handle;
        protected Queue<T> objectQueue = new Queue<T>();
        protected AutoResetEvent mutex = new AutoResetEvent(false);
        protected volatile bool run = false;
        protected volatile bool initialized = false;

        public bool Runing
        {
            get
            {
                return run;
            }
        }

        public void Enqueue(T msg)
        {
            if (msg != null)
            {
                lock (objectQueue)
                {
                    objectQueue.Enqueue(msg);
                    mutex.Set();
                }
            }
        }

        public T Dequeue()
        {
            lock (objectQueue)
            {
                if (objectQueue.Count > 0)
                {
                    return objectQueue.Dequeue();
                }

                return null;
            }
        }

        public virtual void InitializeQueueProcessor(Action<T> handle, int taskNum = 5)
        {
            Handle = handle;

            for (int i = 0; i < taskNum; i++)
            {
                tasks.Add(new Task(Run));
            }

            initialized = true;
        }

        protected virtual void Run()
        {
            while (run)
            {
                mutex.WaitOne();

                T msg = objectQueue.Dequeue();

                if (Handle != null)
                {
                    try
                    {
                        Handle(msg);
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
            if (!initialized)
            {
                throw new InvalidOperationException("Queue Processor is not initialized!");
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
