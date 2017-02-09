using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DHTSpider.Core.Common;
using DHTSpider.Core.Logging;
using DHTSpider.TaskServer.KademliaNetwork.Protocol.Operators;

namespace DHTSpider.TaskServer.KademliaNetwork.Protocol
{
    public class RoutingTableProcessor : TaskQueueProcessor<Contact>
    {
        private RoutingTable mTable;

        public RoutingTableProcessor(RoutingTable table)
        {
            mTable = table;
        }
        
        public void InitializeQueueProcessor()
        {
            this.tasks.Add(new Task(StatusRun));

            base.InitializeQueueProcessor(new Action<Contact>(PingHandle), 1);
        }

        private void StatusRun()
        {
            while (run)
            {
                try
                {
                    List<Contact> contacts = mTable.GetContacts();

                    foreach (Contact con in contacts)
                    {
                        Process(con);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.Error(ex.Message);
                }
                finally
                {
                    Thread.Sleep(1000 * 60);
                }
            }
        }

        private void Process(Contact con)
        {
            try
            {
                con.UpdateStatus();

                if (con.Status == ContactStatus.Bad)
                {
                    mTable.RemoveContact(con);
                }
                else if (con.Status == ContactStatus.ToUpdate)
                {
                    switch(mTable.ProtocolSource)
                    {
                        case ProtocolSource.BIT_TORRENT_KRPC:
                            Enqueue(con);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
            }
        }


        private void PingHandle(Contact con)
        {
            MessageOperator.Instance.PingRequestHandle(mTable.ProtocolSource, con);
        }
    }
}
