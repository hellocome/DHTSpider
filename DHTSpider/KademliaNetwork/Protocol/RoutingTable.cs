using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DHTSpider.Core.Logging;

namespace DHTSpider.TaskServer.KademliaNetwork.Protocol
{
    public sealed class RoutingTable : IDisposable
    {
        private List<Bucket> bucketList = null;
        private RoutingTableProcessor processor;

        public int TotalBuckets
        {
            get;
            private set;
        }

        public string RoutingFile;
        public string RoutingDirectory;

        public ID OwnID
        {
            get;
            private set;
        }


        public ProtocolSource ProtocolSource
        {
            get;
            set;
        }

        public RoutingTable(ID ownID, ProtocolSource psource)
        {
            processor = new RoutingTableProcessor(this);
            processor.InitializeQueueProcessor();
            processor.Start();

            OwnID = ownID;
            ProtocolSource = psource;

            RoutingDirectory = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "RoutingTable";
            RoutingFile = RoutingDirectory + Path.DirectorySeparatorChar + ownID.GetHexString();

            TotalBuckets = DefaultSettings.BUCKET_NUM;

            bucketList = new List<Bucket>(TotalBuckets);

            for (int i = 0; i < TotalBuckets; i++)
            {
                Bucket bucket = new Bucket(i);
                bucketList.Add(bucket);
            }

            List<Contact> contacks = RoutingTableXML.LoadContacts(RoutingFile);

            foreach(Contact con in contacks)
            {
                con.SetToUpdate();
                AddOrUpdateContact(con, false);
            }
        }

        public Bucket FindBucket(ID id)
        {
            return bucketList[OwnID.DifferingBit(id)];
        }

        public void AddOrUpdateContact(Contact newContact, bool promote = true)
        {
            FindBucket(newContact.ID).AddOrUpdateContact(newContact, promote);
        }

        public Contact GetContact(ID toGet)
        {
            return FindBucket(toGet).GetContact(toGet);
        }

        public void RemoveContact(ID toRemove)
        {
            FindBucket(toRemove).RemoveContact(toRemove);
        }

        public void RemoveContact(Contact toRemove)
        {
            FindBucket(toRemove.ID).RemoveContact(toRemove);
        }

        public void Promote(ID toPromote)
        {
            FindBucket(toPromote).Promote(toPromote);
        }

        public List<Contact> CloseContacts(int count, ID target, ID excluded)
        {
            List<Contact> found = new List<Contact>();
            List<ID> distances = new List<ID>();

            // For every Contact we have
            for (int i = 0; i < bucketList.Count; i++)
            {
                lock (bucketList[i])
                {
                    for (int j = 0; j < bucketList[i].Count; j++)
                    {
                        Contact applicant = bucketList[i][j];

                        // Exclude excluded contact
                        if (applicant.ID == excluded)
                        {
                            continue;
                        }

                        // Add the applicant at the right place
                        ID distance = applicant.ID ^ target;
                        int addIndex = 0;

                        while (addIndex < distances.Count && distances[addIndex].AsDistance() < distance.AsDistance())
                        {
                            addIndex++;
                        }

                        distances.Insert(addIndex, distance);
                        found.Insert(addIndex, applicant);

                        // Remove the last entry if we've grown too big
                        if (distances.Count >= count)
                        {
                            distances.RemoveAt(distances.Count - 1);
                            found.RemoveAt(found.Count - 1);
                        }
                    }
                }
            }

            // Give back the list of closest.
            return found;
        }

        public List<Contact> Close8Contacts(ID target, ID excluded)
        {
            return CloseContacts(8, target, excluded);
        }

        public Contact Close1Contacts(ID target, ID excluded)
        {
            List<Contact> cons = CloseContacts(1, target, excluded);

            if(cons.Count > 0)
            {
                return cons[0];
            }

            return null;
        }

        public int NodesToKey(ID key)
        {
            Bucket bucket = FindBucket(key);

            int inActualBucket = 0;
            lock (bucket)
            {
                for (int i = 0; i < bucket.Count; i++)
                {
                    if ((bucket[i].ID ^ OwnID).AsDistance() < (key ^ OwnID).AsDistance())
                    {
                        inActualBucket++;
                    }
                }
            }

            return bucket.Index + inActualBucket;
        }

        public List<Contact> GetContacts()
        {
            List<Contact> contacts = new List<Contact>();

            for (int i = 0; i < bucketList.Count; i++)
            {
                lock (bucketList[i])
                {
                    for (int j = 0; j < bucketList[i].Count; j++)
                    {
                        contacts.Add(bucketList[i][j]);
                    }
                }
            }

            return contacts;
        }

        private bool Load()
        {
            try
            {
                if (!Directory.Exists(RoutingDirectory))
                {
                    Directory.CreateDirectory(RoutingDirectory);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(OwnID + ": " + ex.Message);
                return false;
            }
        }

        public bool Save()
        {
            try
            {
                List<Contact> contacts = GetContacts();

                if (!Directory.Exists(RoutingDirectory))
                {
                    Directory.CreateDirectory(RoutingDirectory);
                }

                return RoutingTableXML.SaveContacts(RoutingFile, contacts);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(OwnID + ": " + ex.Message);
                return false;
            }
        }

        public void Dispose()
        {
            if (processor != null)
            {
                processor.Stop();
            }

            GC.SuppressFinalize(this);
        }
    }
}
