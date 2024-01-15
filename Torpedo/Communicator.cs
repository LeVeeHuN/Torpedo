using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Torpedo
{
    internal class Communicator
    {
        //IPHostEntry host;
        IPAddress ipAddr;
        IPEndPoint localEndPoint;

        public Communicator()
        {
            //host = Dns.GetHostEntry("localhost");
            //ipAddr = host.AddressList[0];
            ipAddr = IPAddress.Parse("0.0.0.0");
            localEndPoint = new IPEndPoint(ipAddr, 5100);
        }

        public void StartServerLoop(LevLogger logger, Game game)
        {
            try
            {
                Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(10);
                logger.AddLog(new LevLog(LogLevel.LogDebug, "(Communicator) Waiting for connections..."));
                while (true)
                {
                    Socket handler = listener.Accept();

                    // Incoming data
                    string data = null;
                    byte[] bytes = null;
                    while (true)
                    {
                        bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }

                    string response = game.ProcessMsg(data)+"<EOF>";
                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    handler.Send(responseBytes);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                logger.AddLog(new LevLog(LogLevel.LogError, $"(Communicator) {ex.ToString()}"));
            }
        }
    }
}
