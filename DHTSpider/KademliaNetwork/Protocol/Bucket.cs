using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHTSpider.TaskServer.KademliaNetwork.Protocol
{
    public class Bucket
    {
        private List<Contact> contactList = null;

        public int BucketSize
        {
            get;
            private set;
        }

        public int Count
        {
            get
            {
                return contactList.Count;
            }
        }

        public int Index
        {
            get;
            private set;
        }

        public Contact this[int index]
        {
            get
            {
                return contactList[index];
            }
        }

        public Bucket(int index)
        {
            Index = index;
            BucketSize = DefaultSettings.BUCKET_SIZE;
            contactList = new List<Contact>(BucketSize);
        }

        public void AddOrUpdateContact(Contact newContact, bool promote = true)
        {
            if (promote)
            {
                Promote(newContact.ID);
            }

            if (contactList.Count < BucketSize)
            {
                contactList.Add(newContact);
            }
            else
            {
                // First one is always the most inactive
                if (CheckActivity(contactList[0]) == false)
                {
                    contactList[0] = newContact;
                }
            }
        }


        // Todo
        private bool CheckActivity(Contact newContact)
        {
            if(newContact.Status == ContactStatus.Bad)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Contact GetContact(ID toGet)
        {
            lock (contactList)
            {
                for (int i = 0; i < contactList.Count; i++)
                {
                    if (contactList[i].ID == toGet)
                    {
                        return contactList[i];
                    }
                }
            }

            return null;
        }

        public void RemoveContact(ID toRemove)
        {
            lock (contactList)
            {
                for (int i = 0; i < contactList.Count; i++)
                {
                    if (contactList[i].ID == toRemove)
                    {
                        contactList.Remove(contactList[i]);
                    }
                }
            }
        }

        public void RemoveContact(Contact toRemove)
        {
            lock (contactList)
            {
                for (int i = 0; i < contactList.Count; i++)
                {
                    if (contactList[i].ID == toRemove.ID)
                    {
                        contactList.Remove(contactList[i]);
                    }
                }
            }
        }

        public void Promote(ID toPromote)
        {
            lock (contactList)
            {
                Contact contact = GetContact(toPromote);

                if(contact != null)
                {
                    contact.ForceGood();
                    contactList.Remove(contact);
                    contactList.Add(contact);
                }
            }
        }
    }
}
