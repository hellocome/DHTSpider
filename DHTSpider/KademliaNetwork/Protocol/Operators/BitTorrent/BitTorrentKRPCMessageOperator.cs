using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using BencodeNET.Parsing;
using BencodeNET.Objects;
using DHTSpider.TaskServer.KademliaNetwork.Protocol.Messages;
using DHTSpider.Core.Logging;

using DHTSpider.TaskServer.KademliaNetwork.Servers;

namespace DHTSpider.TaskServer.KademliaNetwork.Protocol.Operators.BitTorrent
{
    public partial class BitTorrentKRPCMessageOperator
    {
        public ID OwnID
        {
            get;
            private set;
        }

        public BitTorrentKRPCMessageOperator(IKademliaPeer peer)
        {
            OwnID = peer.NodeID;
        }

        public void HandleMessage(IMessage msg)
        {
            BencodeParser parser = new BencodeParser();
            BDictionary dic = parser.Parse<BDictionary>(msg.Content);

            HandleMessage(msg, dic);
        }
        
        public void HandleMessage(IMessage msg, BDictionary dic)
        {
            IBObject tValue;
            IBObject yValue;

            if (dic.TryGetValue("t", out tValue) && dic.TryGetValue("y", out yValue))
            {
                string yStr = yValue.ToString();

                if (yStr.Equals("q"))
                {
                    QueryHandler(msg, dic);
                }
                else if (yStr.Equals("r"))
                {
                    ResponseHandler(msg, dic);
                }
                else if (yStr.Equals("e"))
                {
                    ErrorHandle(msg, dic);
                }
            }
        }

        #region Handle Error
        public void ErrorHandle(IMessage msg, BDictionary dic)
        {
            IBObject eValue;

            if(dic.TryGetValue("e", out eValue))
            {
                BList list = eValue as BList;

                if(list != null)
                {
                    if (list.Count == 1)
                    {
                        Logger.Instance.Info("KRPC Error: {0} = {1}", list[0], GetError(list[0]));
                    }
                    else if(list.Count > 1)
                    {
                        Logger.Instance.Info("KRPC Error: {0} = {1}", list[0], list[1]);
                    }
                }
            }
        }

        private string GetError(IBObject code)
        {
            string error = (code != null ? code.ToString() : "Unknown Error");

            switch(error)
            {
                case "201":
                    return "Generic Error";
                case "202":
                    return "Server Error";
                case "203":
                    return "Protocol Error, such as a malformed packet, invalid arguments, or bad token";
                case "204":
                    return "Method Unknown";
                default:
                    return error;
            }
        }
        #endregion

        #region Handle Query
        public void QueryHandler(IMessage msg, BDictionary dic)
        {
            IBObject idValue;

            if (dic.TryGetValue("q", out idValue))
            {
                string qMethold = idValue.ToString();

                switch (qMethold)
                {
                    case "ping":
                        this.PingResponseHandle(msg, dic);
                        break;
                    case "find_node":
                        this.FindNodeResponseHandle(msg, dic);
                        break;
                    case "get_peers":
                        this.GetPeersResponseHandle(msg, dic);
                        break;
                    case "announce_peer":
                        this.AnnouncePeerResponseHandle(msg, dic);
                        break;
                }
            }
        }

        #endregion

        #region Handle Responses
        public void ResponseHandler(IMessage msg, BDictionary dic)
        {
            BDictionary arguDic = GetArgumentDic(dic, "r");

            if(arguDic != null)
            { 
                this.ResponseForRequestHandle(msg, arguDic);
            }
        }

        #endregion


        #region BootStrap
        public bool BootStrap()
        {
            bool result = false;
            foreach (string key in DefaultSettings.BT_BootStrap.Keys)
            {
                try
                {
                    IPHostEntry ipHost = Dns.GetHostEntry(key);

                    IPEndPoint endPoint = new IPEndPoint(ipHost.AddressList[0], DefaultSettings.BT_BootStrap[key]);
                    FindNodeRequestHandle(ProtocolSource.BIT_TORRENT_KRPC, endPoint);

                    result = true;
                }
                catch (Exception ex)
                {
                    Logger.Instance.Error(ex.Message);
                }
            }

            return result;
        }

        #endregion
    }
}
