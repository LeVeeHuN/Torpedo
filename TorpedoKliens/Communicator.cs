using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TorpedoKliens
{
    internal class Communicator
    {
        //IPHostEntry host;
        IPAddress ipAddr;
        IPEndPoint remoteEndPoint;

        public Communicator(string serverIp)
        {
            //host = Dns.GetHostEntry("localhost");
            //ipAddr = host.AddressList[0];
            ipAddr = IPAddress.Parse(serverIp);
            remoteEndPoint = new IPEndPoint(ipAddr, 5100);
        }

        public string Communicate(string message)
        {
            byte[] bytes = new byte[1024];

            try
            {
                Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                sender.Connect(remoteEndPoint);

                byte[] msg = Encoding.UTF8.GetBytes(message+"<EOF>");
                int bytesSent = sender.Send(msg);

                int bytesRec = sender.Receive(bytes);
                string responseMsg = Encoding.UTF8.GetString(bytes, 0, bytesRec);

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return responseMsg.Replace("<EOF>", string.Empty);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return "000000000000000000";
        }
    }
}
