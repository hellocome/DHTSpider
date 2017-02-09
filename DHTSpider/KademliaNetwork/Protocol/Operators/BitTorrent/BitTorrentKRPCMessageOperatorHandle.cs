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
using DHTSpider.Core.Common;

namespace DHTSpider.TaskServer.KademliaNetwork.Protocol.Operators.BitTorrent
{
    //http://www.bittorrent.org/beps/bep_0005.html
    partial class BitTorrentKRPCMessageOperator
    {
        public static BDictionary GetArgumentDic(BDictionary dic)
        {
            IBObject objValue = null;

            if (dic.TryGetValue("a", out objValue))
            {
                return objValue as BDictionary;
            }

            return null;
        }

        public static BDictionary GetArgumentDic(BDictionary dic, string name)
        {
            IBObject objValue = null;

            if (dic.TryGetValue(name, out objValue))
            {
                return objValue as BDictionary;
            }

            return null;
        }

        public static string GetStringArgumentDic(BDictionary dic, string name)
        {
            IBObject objValue = null;

            if (dic.TryGetValue(name, out objValue))
            {
                return objValue.ToString();
            }

            return string.Empty;
        }

        public static void UpdateContact(IMessage msg, BDictionary dic)
        {
            IBObject objValue = null;
            if (dic.TryGetValue("id", out objValue))
            {
                ID id = new ID(objValue.ToString());
                IPEndPoint endPoint = msg.EndPoint as IPEndPoint;
                Contact contact = new Contact(ContactType.UDP, id, endPoint.Address, (UInt16)endPoint.Port);

                RoutingTableFactory.Instance.DefaultRoutingTable.AddOrUpdateContact(contact);
            }
        }

        #region Ping Response
        public IMessage CreatePingResponseMessage(IMessage message)
        {
            BDictionary resDic = new BDictionary();
            BDictionary resList = new BDictionary();
            resList.Add("id", OwnID.ToString());

            resDic.Add("t", "aa");
            resDic.Add("y", "r");
            resDic.Add("r", resList);

            return new RawMessage(message.ProtocolSource, message.MessageType, message.EndPoint, resDic.EncodeAsBytes());
        }

        public void PingResponseHandle(IMessage msg, BDictionary dic)
        {
            BDictionary dicArgument = GetArgumentDic(dic);

            if (dicArgument != null)
            {
                UpdateContact(msg, dicArgument);
                IMessage responseMsg = CreatePingResponseMessage(msg);
                MessageOutProcessor.Instance.Enqueue(responseMsg);
            }
        }

        #endregion

        #region Find Node Response
        public IMessage CreateFindNodeResponseMessage(IMessage message, string target, List<Contact> contacts)
        {
            BDictionary resDic = new BDictionary();
            BDictionary resArgumentDic = new BDictionary();

            byte[] nodes = new byte[DefaultSettings.BTIDIPPORT_LENGTH * DefaultSettings.BTDEFAULT_K];

            for (int i = 0; i < contacts.Count; i++)
            {
                Array.Copy(contacts[i].ToIDIPPortBytes(), 0, nodes, i * DefaultSettings.BTIDIPPORT_LENGTH, DefaultSettings.BTIDIPPORT_LENGTH);
            }

            resArgumentDic.Add("id", target);
            resArgumentDic.Add("nodes", BitConverter.ToString(nodes));

            resDic.Add("t", "aa");
            resDic.Add("y", "r");
            resDic.Add("r", resArgumentDic);

            return new RawMessage(message.ProtocolSource, message.MessageType, message.EndPoint, resDic.EncodeAsBytes());
        }

        public void FindNodeResponseHandle(IMessage msg, BDictionary dic)
        {
            try
            {
                BDictionary dicArgument = GetArgumentDic(dic);

                if (dicArgument != null)
                {
                    UpdateContact(msg, dicArgument);
                    string remoteId = GetStringArgumentDic(dicArgument, "id");
                    string targetId = GetStringArgumentDic(dicArgument, "target");

                    List<Contact> contacts = RoutingTableFactory.Instance.DefaultRoutingTable.Close8Contacts(new ID(targetId), new ID(remoteId));

                    IMessage responseMsg = CreateFindNodeResponseMessage(msg, targetId, contacts);
                    MessageOutProcessor.Instance.Enqueue(responseMsg);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
            }
        }

        #endregion

        #region Get Peers Response

        // We won't maintain a storage table
        public static IMessage CreateGetPeersResponseMessage__ALWAYS_NODES(IMessage message, string target, List<Contact> contacts)
        {
            BDictionary resDic = new BDictionary();
            BDictionary resArgumentDic = new BDictionary();


            byte[] nodes = new byte[DefaultSettings.BTIDIPPORT_LENGTH * DefaultSettings.BTDEFAULT_K];

            for (int i = 0; i < contacts.Count; i++)
            {
                Array.Copy(contacts[i].ToIDIPPortBytes(), 0, nodes, i * DefaultSettings.BTIDIPPORT_LENGTH, DefaultSettings.BTIDIPPORT_LENGTH);
            }

            resArgumentDic.Add("id", target);
            resArgumentDic.Add("token", DefaultSettings.TOKEN);
            resArgumentDic.Add("nodes", BitConverter.ToString(nodes));

            resDic.Add("t", "aa");
            resDic.Add("y", "r");
            resDic.Add("r", resArgumentDic);

            return new RawMessage(message.ProtocolSource, message.MessageType, message.EndPoint, resDic.EncodeAsBytes());
        }

        public void GetPeersResponseHandle(IMessage msg, BDictionary dic)
        {
            try
            {
                BDictionary dicArgument = GetArgumentDic(dic);

                if (dicArgument != null)
                {
                    UpdateContact(msg, dicArgument);
                    string remoteId = GetStringArgumentDic(dicArgument, "id");
                    string targetId = GetStringArgumentDic(dicArgument, "info_hash");

                    List<Contact> contacts = RoutingTableFactory.Instance.DefaultRoutingTable.Close8Contacts(new ID(targetId), new ID(remoteId));

                    IMessage responseMsg = CreateGetPeersResponseMessage__ALWAYS_NODES(msg, targetId, contacts);
                    MessageOutProcessor.Instance.Enqueue(responseMsg);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
            }
        }

        #endregion

        #region Announce Peer Response

        // This is main function we need to deal
        // We won't maintain a storage table
        public static IMessage CreateAnnouncePeerResponseMessage(IMessage message, string infoHashId)
        {
            BDictionary resDic = new BDictionary();
            BDictionary resArgumentDic = new BDictionary();


            resArgumentDic.Add("id", infoHashId);

            resDic.Add("t", "aa");
            resDic.Add("y", "r");
            resDic.Add("r", resArgumentDic);

            return new RawMessage(message.ProtocolSource, message.MessageType, message.EndPoint, resDic.EncodeAsBytes());
        }

        public void AnnouncePeerResponseHandle(IMessage msg, BDictionary dic)
        {
            try
            {
                BDictionary dicArgument = GetArgumentDic(dic);

                if (dicArgument != null)
                {
                    UpdateContact(msg, dicArgument);

                    string remoteId = GetStringArgumentDic(dicArgument, "id");
                    string infoHashId = GetStringArgumentDic(dicArgument, "info_hash");

                    if (ID.Validate(infoHashId))
                    {
                        DHTSpider.TaskServer.Database.ItemRecordManager.FindItemRecord(infoHashId);
                    }

                    IMessage responseMsg = CreateAnnouncePeerResponseMessage(msg, infoHashId);
                    MessageOutProcessor.Instance.Enqueue(responseMsg);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
            }
        }

        #endregion

        public void ResponseForRequestHandle(IMessage msg, BDictionary dic)
        {
            try
            {
                UpdateContact(msg, dic);

                IBObject nodesObj = null;

                if (dic.TryGetValue("nodes", out nodesObj) && nodesObj != null)
                {
                    string nodes = nodesObj.ToString();

                    byte[] nodesBytes = Encoding.ASCII.GetBytes(nodes);
                    byte[] nodeByte = null;
                    int index = 0;
                    int count = 0;

                    while ((nodeByte = nodesBytes.SubArray(index, DefaultSettings.BTIDIPPORT_LENGTH)) != null && count < DefaultSettings.BTDEFAULT_K)
                    {
                        ID id = new ID(nodeByte.SubArray(0, 20));
                        IPAddress ip = new IPAddress(nodeByte.SubArray(20, 4));
                        UInt16 port = BitConverter.ToUInt16(nodeByte.SubArray(24, 2), 0);

                        Contact contact = new Contact(ContactType.UDP, id, ip, port);
                        contact.SetToUpdate();

                        RoutingTableFactory.Instance.DefaultRoutingTable.AddOrUpdateContact(contact, false);

                        index += DefaultSettings.BTIDIPPORT_LENGTH;
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
            }
        }

        private IMessage CreatePingRequestMessage(ProtocolSource ps,  Contact contact)
        {
            BDictionary resDic = new BDictionary();

            BDictionary resList = new BDictionary();
            resList.Add("id", OwnID.ToString());

            resDic.Add("t", "aa");
            resDic.Add("y", "q");
            resDic.Add("q", "ping");
            resDic.Add("a", resList);

            IPEndPoint endPoint = new IPEndPoint(contact.IP, contact.Port);

            return new RawMessage(ps, MessageType.PING_REQUEST, endPoint, resDic.EncodeAsBytes());
        }

        public void PingRequestHandle(ProtocolSource ps, Contact contact)
        {
            IMessage msg = CreatePingRequestMessage(ps, contact);
            MessageOutProcessor.Instance.Enqueue(msg);
        }

        private IMessage CreateFindNodeRequestMessage(ProtocolSource ps, IPEndPoint endPoint)
        {
            BDictionary resDic = new BDictionary();
            BDictionary resList = new BDictionary();
            resList.Add("id", OwnID.ToString());
            resList.Add("target", OwnID.ToString());

            resDic.Add("t", "aa");
            resDic.Add("y", "q");
            resDic.Add("q", "find_node");
            resDic.Add("a", resList);
            
            return new RawMessage(ps, MessageType.PING_REQUEST, endPoint, resDic.EncodeAsBytes());
        }

        public void FindNodeRequestHandle(ProtocolSource ps, IPEndPoint endPoint)
        {
            IMessage msg = CreateFindNodeRequestMessage(ps, endPoint);
            MessageOutProcessor.Instance.Enqueue(msg);
        }
        
    }
}
