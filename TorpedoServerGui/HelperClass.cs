using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Torpedo;

namespace TorpedoServerGui
{
    internal class HelperClass
    {
        public static Form1 form1;

        public static int serverport = 0;
        public static Communicator communicator = null;
        public static volatile LevLogger logger = null;

        public static ServerForm sf;

        public static void LogFeeder()
        {
            while (true)
            {
                Thread.Sleep(1500);
                List<string> logsString = new List<string>();
                foreach (LevLog log in logger.Logs)
                {
                    if (log.LogLevel != LogLevel.LogOff || log.LogLevel != LogLevel.LogDebug)
                    {
                        logsString.Add(log.ToString());
                    }
                }
                Action a = () => { sf.UpdateLogs(logsString.ToArray()); };
                a.Invoke();
            }
        }

        public static void GamesFeeder()
        {
            while (true)
            {
                Thread.Sleep(1500);
                List<string> mainDataSource = new List<string>();
                foreach (string roomcode in communicator.GameRooms())
                {
                    if (!mainDataSource.Contains(roomcode))
                    {
                        mainDataSource.Add(roomcode);
                    }
                }
                Action a = () => { sf.UpdateGames(mainDataSource.ToArray()); };
                a.Invoke();
            }
        }
    }
}
