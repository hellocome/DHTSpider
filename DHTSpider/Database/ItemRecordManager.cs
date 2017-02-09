using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHTSpider.Core.Database;
using DHTSpider.TaskServer.KademliaNetwork.Protocol;
using DHTSpider.Core.Logging;

namespace DHTSpider.TaskServer.Database
{
    public class ItemRecordManager
    {
        public static ItemRecord FindItemRecord(string hashInfo, bool createNewOnNotFound = true)
        {
            return ItemRecord.FindItemRecord(hashInfo, createNewOnNotFound);
        }

        public static string DefaultCharset = "UTF8";

        public static void OnDBError(Exception e)
        {
            Logger.Instance.Error("Error:  " + e.Message);
        }

        public static bool Initialize()
        {
            try
            {
                DatabaseUtil.DBType = DefaultSettings.DBType;
                DatabaseUtil.ConnectionString = DefaultSettings.DBConnectionString;
                DatabaseUtil.DefaultCharset = DefaultCharset;

                var asm = typeof(ItemRecord).Assembly;

                try
                {
                    if (!DatabaseUtil.InitAR(asm))
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    OnDBError(e);

                    // repeat init
                    DatabaseUtil.InitAR(asm);
                }


                DatabaseUtil.CreateSchema();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Error:  " + ex.Message);
                return true;
            }
        }
    }
}
