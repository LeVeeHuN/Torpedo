using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorpedoKliensGUI
{
    internal class InfoGetter
    {
        public static string username;
        public static string roomcode;
        public static TorpedoKliens.Communicator communicator;
        public static Form1 form1;

        public static void GetInfo()
        {
            string infoString = $"info;{username};{roomcode}";
            while (true)
            {
                string response = communicator.Communicate(infoString);
                // info responseok feldolgozása
                if (response.Equals("error"))
                {
                    form1.ProcessInfoResponse(response);
                    break;
                }
                string[] responseContent = response.Split(";");
                if (responseContent[0].Equals("game_over"))
                {
                    form1.ProcessInfoResponse(response);
                    break;
                }
                form1.ProcessInfoResponse(response);
                Thread.Sleep(1500);
            }
        }
    }
}
