using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHTSpider.Core.Network
{
    public interface IServer : IDisposable
    {
        bool Start();
        bool Stop();
    }
}
