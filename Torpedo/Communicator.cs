using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Torpedo
{
    public class Communicator
    {
        //IPHostEntry host;
        IPAddress ipAddr;
        IPEndPoint localEndPoint;
        List<Game> games;
        public LevLogger logger;

        public Communicator()
        {
            //host = Dns.GetHostEntry("localhost");
            //ipAddr = host.AddressList[0];
            ipAddr = IPAddress.Parse("0.0.0.0");
            localEndPoint = new IPEndPoint(ipAddr, 5100);
            games = new List<Game>();
        }

        public Communicator(int port)
        {
            ipAddr = IPAddress.Parse("0.0.0.0");
            localEndPoint = new IPEndPoint(ipAddr, port);
            games = new List<Game>();
        }

        public void StartServerLoop(LevLogger logger)
        {
            this.logger = logger;
            StartServerLoop();
        }

        public void StartServerLoop()
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

                    data = data.Replace("<EOF>", string.Empty);

                    // Ha új játék parancs
                    if (data.Split(";")[0].Equals("new_game"))
                    {
                        string roomCode = data.Split(";")[1];
                        bool activeGame = false;
                        foreach(Game g in games)
                        {
                            if (g.RoomCode.Equals(roomCode))
                            {
                                activeGame = true;
                            }
                        }
                        if (activeGame)
                        {
                            logger.AddLog(new LevLog(LogLevel.LogInfo, $"(Communicator) Megpróbáltak új szobát létrehozni, de már létezik: {roomCode}"));
                            handler.Send(Encoding.UTF8.GetBytes("fail<EOF>"));
                        }
                        else
                        {
                            logger.AddLog(new LevLog(LogLevel.LogInfo, $"(Communicator) Új szoba létrehozva: {roomCode}"));
                            games.Add(new Game(logger, roomCode));
                            handler.Send(Encoding.UTF8.GetBytes("success<EOF>"));
                        }
                    }
                    else if (data.Split(";")[0].Equals("check_room"))
                    {
                        // szoba ellenőrzése
                        string roomCode = data.Split(";")[1];
                        bool exists = false;
                        bool full = false;
                        Console.WriteLine($"játékok száma: {games.Count}");
                        foreach (Game g in games)
                        {
                            Console.WriteLine(g.RoomCode);
                            if (g.RoomCode.Equals(roomCode))
                            {
                                exists = true;
                                if (g.PlayersCount == 2)
                                {
                                    full = true;
                                }
                            }
                        }

                        if (!exists)
                        {
                            logger.AddLog(new LevLog(LogLevel.LogInfo, $"(Communicator) Szoba ellenőrzés. Nem létező szoba: {roomCode}"));
                            handler.Send(Encoding.UTF8.GetBytes("non_existent<EOF>"));
                        }
                        else if (full)
                        {
                            logger.AddLog(new LevLog(LogLevel.LogInfo, $"(Communicator) Szoba ellenőrzés. Szoba tele: {roomCode}"));
                            handler.Send(Encoding.UTF8.GetBytes("full<EOF>"));
                        }
                        else
                        {
                            logger.AddLog(new LevLog(LogLevel.LogInfo, $"(Communicator) Szoba ellenőrzés. Szoba szabad: {roomCode}"));
                            handler.Send(Encoding.UTF8.GetBytes("ok<EOF>"));
                        }
                    }
                    else
                    {
                        // VESZÉLYES!!!
                        // Ha később valami argumentumot rakok még a parancsok végére EL FOG TÖRNI
                        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        string roomCode = data.Split(';')[data.Split(";").Length - 1];
                        bool roomExists = false;
                        foreach (Game g in games)
                        {
                            if (g.RoomCode.Equals(roomCode))
                            {
                                roomExists = true;
                                string response = g.ProcessMsg(data)+"<EOF>";
                                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                                handler.Send(responseBytes);
                                handler.Shutdown(SocketShutdown.Both);
                                handler.Close();
                                break;
                            }
                        }
                        if (!roomExists)
                        {
                            handler.Send(Encoding.UTF8.GetBytes("error<EOF>"));
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                        }
                    }
                    CheckGamesForDeletablesAndDeleteThem(logger);
                }
            }
            catch (Exception ex)
            {
                logger.AddLog(new LevLog(LogLevel.LogError, $"(Communicator) {ex.ToString()}"));
            }
        }

        public string[] GetPlayerNames(string roomcode)
        {
            string[] players = new string[2];
            foreach (Game g in games)
            {
                if (g.RoomCode.Equals(roomcode))
                {
                    for (int i = 0; i < g.players.Count; i++)
                    {
                        players[i] = g.players[i].Name;
                    }
                }
            }
            return players;
        }

        public string[] GameRooms()
        {
            List<string> rooms = new List<string>();
            foreach (Game g in games)
            {
                rooms.Add(g.RoomCode);
            }
            return rooms.ToArray();
        }

        private void CheckGamesForDeletablesAndDeleteThem(LevLogger logger)
        {
            List<Game> newGames = new List<Game>();
            int oldsize = games.Count;
            foreach (Game game in games)
            {
                if (!game.Deletable)
                {
                    newGames.Add(game);
                }
            }
            int newsize = newGames.Count;
            int difference = oldsize - newsize;
            if (difference > 0)
            {
                logger.AddLog(new LevLog(LogLevel.LogInfo, $"(CheckGamesForDeletablesAndDeleteThem) Deleted: {difference} games"));
            }
            games = newGames;
        }

        public void DeleteRoom(string roomcode)
        {
            List<Game> newGames = new List<Game>();
            foreach (Game game in games)
            {
                if (!game.RoomCode.Equals(roomcode))
                {
                    newGames.Add(game);
                }
            }
            int diff = games.Count - newGames.Count;
            if (diff > 0)
            {
                logger.AddLog(new LevLog(LogLevel.LogInfo, $"(DeleteRoom) Manually deleted: {roomcode}"));
            }
            else
            {
                logger.AddLog(new LevLog(LogLevel.LogError, $"(DeleteRoom) Manual room deletion failed: {roomcode}"));
            }
            games = newGames;
        }
    }
}
