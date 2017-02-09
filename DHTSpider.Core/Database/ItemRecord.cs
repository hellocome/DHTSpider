using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.ActiveRecord;
using NLog;

namespace DHTSpider.Core.Database
{
    [ActiveRecord(Access = PropertyAccess.Property)]
    public class ItemRecord : DBRecord<ItemRecord>
    {
        internal static int GetCount()
        {
            return Count();
        }

        public ItemRecord(string hashInfo)
        {
            HashInfo = hashInfo;
        }

        public ItemRecord()
        {
            HashInfo = string.Empty;
            RequestCount = 0;
            Seed = null ;
            LastRequest = DateTime.Now;
            FirstRequest = DateTime.Now;
        }

        [PrimaryKey(PrimaryKeyType.Assigned, Length = 20)]
        public string HashInfo
        {
            get;
            private set;
        }

        [Property]
        public int RequestCount
        {
            get;
            private set;
        }

        [Property(Length = 65535)]
        public byte[] Seed
        {
            get;
            private set;
        }

        [Property]
        public DateTime LastRequest
        {
            get;
            private set;
        }

        [Property]
        public DateTime FirstRequest
        {
            get;
            private set;
        }

        public void IncreaseCount()
        {
            RequestCount++;
            LastRequest = DateTime.Now;
        }

        public void UpdateSeed(byte[] seed)
        {
            LastRequest = DateTime.Now;
            Seed = seed;
        }

        public string Details
        {
            get
            {
                return string.Format("ItemRecord: {0} FirstAccess: {1}, LastAccess: {2}, RequestCount:{3} Seed: {4}",
                                     HashInfo, FirstRequest, LastRequest, RequestCount.ToString(), (Seed != null ? Seed.Length.ToString() : "Not Available"));
            }
        }

        public override string ToString()
        {
            return "HashInfo: " + HashInfo;
        }

        public static ItemRecord FindItemRecord(string hashInfo, bool createNewOnNotFound = true)
        {
            try
            {
                return Find(hashInfo);
            }
            catch (NHibernate.ObjectNotFoundException)
            {
                ItemRecord ir = new ItemRecord(hashInfo);
                ir.LastRequest = DateTime.Now;
                ir.FirstRequest = DateTime.Now;
                ir.IncreaseCount();
                ir.CreateAndFlush();

                return ir;
            }
        }
    }
}
