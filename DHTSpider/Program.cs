using System;
using System.Threading;
using System.Windows.Forms;
using DHTSpider.Core.Logging;

namespace DHTSpider
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string [] args)
        {
            Logger.InitLogger("DHTSpider");

            if (!AcquireMutex())
            {
                Logger.Instance.Info("An instance of SysAgent is already running. Terminating.");
                return;
            }

            if (args != null && args.Length == 1 && args[0] == "/db")
            {
                DHTSpider.TaskServer.Database.ItemRecordManager.Initialize();
                return;
            }

            Thread program = new Thread(RunAsApplication);
            program.SetApartmentState(ApartmentState.MTA);
            program.Start();

            Application.ApplicationExit += new EventHandler(ApplicationExit);
            Application.Run();

            m_Mutex.ReleaseMutex();

        }

        private static Mutex m_Mutex = new Mutex(false, "DHTSpider.TaskServer.Running");
        private static EventWaitHandle m_EventExit = new EventWaitHandle(false, EventResetMode.ManualReset, "SysAgentService.Exit");


        public static bool AcquireMutex()
        {
            return m_Mutex.WaitOne(TimeSpan.Zero, true);
        }

        public static void RunAsApplication()
        {
            try
            {
                DHTSpider.TaskServer.TaskServerEntry.Instance.Start();

                m_EventExit.Reset();
                m_EventExit.WaitOne();
            }
            catch (System.Exception ex)
            {
                Logger.Instance.Info("Caught exception: " + ex);
            }
        }

        static void ApplicationExit(object sender, EventArgs e)
        {
            m_EventExit.Set();
        }
    }
}
