using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

namespace DHTSpider.TaskServer.KademliaNetwork.Protocol
{
    public class Contact
    {
        public DateTime LastUpdated
        {
            get;
            private set;
        }

        public DateTime LastOnline
        {
            get;
            private set;
        }


        public void ForceGood()
        {
            UpdateLastOnline();

            try
            {
                Monitor.Enter(mStatus);

                mStatus = ContactStatus.Good;

            }
            finally
            {
                Monitor.Exit(mStatus);
            }
        }

        public void SetToUpdate()
        {
            try
            {
                Monitor.Enter(mStatus);

                mStatus = ContactStatus.ToUpdate;

            }
            finally
            {
                Monitor.Exit(mStatus);
            }
        }

        public void UpdateLastUpdate()
        {
            LastUpdated = DateTime.Now;
        }

        public void UpdateLastOnline()
        {
            LastOnline = DateTime.Now;
            LastUpdated = DateTime.Now;
        }

        private bool TimeCheck(DateTime timeToCheck, TimeSpan last)
        {
            if (DateTime.Now - timeToCheck >= last)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdateStatus()
        {
            try
            {
                Monitor.Enter(mStatus);

                // Already bad, to remove
                if(mStatus == ContactStatus.Bad)
                {
                    return;
                }

                if (TimeCheck(LastOnline, new TimeSpan(0, 15, 0)))
                {
                    if (mStatus == ContactStatus.ToUpdate)
                    {
                        mStatus = ContactStatus.Bad;
                        return;
                    }
                }

                if(TimeCheck(LastUpdated, new TimeSpan(0, 3, 0)))
                {
                    mStatus = ContactStatus.ToUpdate;
                }
            }
            finally
            {
                Monitor.Exit(mStatus);
            }
        }

        public ContactType ContactType
        {
            get;
            set;
        }


        private ContactStatus mStatus;
        public ContactStatus Status
        {
            get
            {
                return mStatus;
            }
        }

        public ID ID
        {
            get;
            private set;
        }

        public IPAddress IP
        {
            get;
            private set;
        }

        public UInt16 Port
        {
            get;
            private set;
        }

        public string URL
        {
            get;
            private set;
        }

        public Contact(ContactType type, ID nid, IPAddress ip, UInt16 port)
        {
            LastUpdated = DateTime.Now;
            LastOnline = DateTime.Now;

            this.ID = nid;
            this.IP = ip;
            this.Port = port;

            ContactType = type;
            mStatus = ContactStatus.Good;
        }

        public Contact(ContactType type, ID nid, string uri)
        {
            LastUpdated = DateTime.Now;
            LastOnline = DateTime.Now;

            this.ID = nid;
            URL = uri;

            ContactType = type;
            mStatus = ContactStatus.Good;
        }

        public byte[] ToIDIPPortBytes()
        {
            List<byte> bytes = new List<byte>(26);

            bytes.AddRange(ID.IdBytes);
            bytes.AddRange(IP.MapToIPv4().GetAddressBytes());
            bytes.AddRange(BitConverter.GetBytes(Port));

            return bytes.ToArray();
        }
    }
}
