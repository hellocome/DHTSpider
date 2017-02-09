using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHTSpider.TaskServer.KademliaNetwork.Protocol;

namespace DHTSpider.TaskServer
{
    public static class DefaultSettings
    {
        public const int BTDEFAULT_K = 8;
        public const int BTIDIPPORT_LENGTH = 26;

        public const int BUCKET_SIZE = 20;
        public const int BUCKET_NUM = 20 * 8;

        public static ID KADEMLIA_SERVER_ID = new ID("sc8dq5tqdqrlepkaw744");
        public static ProtocolSource KADEMLIA_SERVER_DEFAULT_PROTOCOL_SOURCE = ProtocolSource.BIT_TORRENT_KRPC;

        public const string TOKEN = "aoeusnthe2";

        public static string DBType = "mysql5";

        public static string DBConnectionString = @"Server=127.0.0.1;Port=3306;Database=DHTSpider;CharSet=utf8;Uid=root;Pwd=haijiecome;";

        public static Dictionary<string, int> BT_BootStrap = new Dictionary<string, int>() {
            { "router.bittorrent.com", 6881 },
            { "dht.transmissionbt.com", 6881 } };
    }
}
