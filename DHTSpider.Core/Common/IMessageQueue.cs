using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHTSpider.Core.Common
{
    public interface IMessageQueueProcessor<T> where T : class
    {
        void Enqueue(T msg);
        T Dequeue();
    }
}
