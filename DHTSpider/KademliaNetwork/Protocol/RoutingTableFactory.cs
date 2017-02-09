using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHTSpider.TaskServer.KademliaNetwork.Protocol
{
    public class RoutingTableFactory
    {
        private static RoutingTableFactory instance = new RoutingTableFactory();
        public static RoutingTableFactory Instance
        {
            get
            {
                return instance;
            }
        }

        //public static Dictionary<ID, RoutingTable> rTable = new Dictionary<ID, RoutingTable>();


        private RoutingTable rt = null;

        public void CreateRoutingTable(ID id, ProtocolSource source)
        {
            rt = new RoutingTable(id, source);
        }


        public RoutingTable DefaultRoutingTable
        {
            get
            {
                return rt;
            }
        }

        private RoutingTableFactory()
        {
        }        
    }
}
