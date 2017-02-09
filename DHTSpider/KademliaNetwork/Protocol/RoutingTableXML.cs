using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net;

using DHTSpider.Core.Logging;

namespace DHTSpider.TaskServer.KademliaNetwork.Protocol
{
    public class RoutingTableXML
    {
        public static List<Contact> LoadContacts(string file)
        {
            List<Contact> allContact = new List<Contact>();

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(file);

                XmlNodeList nodeList = xmlDoc.SelectNodes("RoutingTable/Contacts/Contact");

                foreach (XmlNode node in nodeList)
                {
                    XmlElement contactsEle = node as XmlElement;

                    string type = contactsEle.GetAttribute("type");
                    string id = contactsEle.GetAttribute("id");

                    Contact contact = null;

                    switch (type)
                    {
                        case "SOAP":
                            string url = contactsEle.GetAttribute("url");

                            if (!string.IsNullOrEmpty(url) && ID.Validate(id))
                            {
                                contact = new Contact(ContactType.SOAP, new ID(id), url);
                            }
                            break;
                        case "UDP":
                            string sIp = contactsEle.GetAttribute("ip");
                            string sPort = contactsEle.GetAttribute("port");
                            IPAddress ip;
                            int port;

                            if (IPAddress.TryParse(sIp, out ip) && int.TryParse(sPort, out port))
                            {
                                contact = new Contact(ContactType.SOAP, new ID(id), ip, (UInt16)port);
                            }
                            break;
                    }

                    if (contact != null)
                    {
                        allContact.Add(contact);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
            }

            return allContact;
        }
        
        public static bool SaveContacts(string file, List<Contact> list)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml("<RoutingTable><Contacts></Contacts></RoutingTable>");

                XmlElement contactsEle = xmlDoc.SelectSingleNode("RoutingTable/Contacts") as XmlElement;

                foreach (Contact contact in list)
                {
                    XmlElement contactEle = xmlDoc.CreateElement("Contact");

                    contactEle.SetAttribute("id", contact.ID.ToString());
                    
                    switch (contact.ContactType)
                    {
                        case ContactType.SOAP:
                            contactEle.SetAttribute("type", "SOAP");
                            contactEle.SetAttribute("url", contact.URL);

                            break;
                        case ContactType.UDP:
                            contactEle.SetAttribute("type", "UDP");
                            contactEle.SetAttribute("ip", contact.IP.ToString());
                            contactEle.SetAttribute("port", contact.Port.ToString());
                            break;
                    }

                    contactsEle.AppendChild(contactEle);
                }

                xmlDoc.Save(file);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);

                return false;
            }
        }
    }
}
