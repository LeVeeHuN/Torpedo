namespace Torpedo
{
    internal class Game
    {
        public List<Player> players;
        string nextPlayer;
        bool gameOver;
        string winnerPlayer;
        LevLogger logger;

        string lastEventStr;
        LocationVector lastShotLocation;
        int lastShipSize;
        
        public Game(LevLogger logger)
        {
            this.players = new List<Player>();
            nextPlayer = "none";
            gameOver = false;
            winnerPlayer = null;
            this.logger = logger;

            lastEventStr = null;
            lastShotLocation = null;
            lastShipSize = 0;
        }

        public string ProcessMsg(string msg)
        {
            msg = msg.Replace("<EOF>", string.Empty);
            //logger.AddLog(new LevLog(LogLevel.LogDebug, $"(ProcessMsg) Processing msg: {msg}"));

            string[] content = msg.Split(";");
            switch (content[0])
            {
                case "new_player":
                    ProcessNewPlayer(msg);
                    return "succes;";
                case "shot":
                    return ProcessShoot(msg);
                case "info":
                    return ProcessInfo(msg);
                case "ship_coords":
                    return ProcessShipCoords(msg);
                default:
                    return "fail;";
            }
        }

        private string ProcessShipCoords(string msg)
        {
            string[] content = msg.Split(';');
            int shipSize = int.Parse(content[2]);
            string playerName = content[1];
            foreach (Player player in players)
            {
                if (!player.Name.Equals(playerName))
                {
                    LocationVector[] locations = player.GetShipLocationsBySize(shipSize);
                    string response = $"{locations.Length}";
                    response = response + "{";
                    string xCoords = string.Empty;
                    string yCoords = string.Empty;
                    for (int i = 0; i < locations.Length; i++)
                    {
                        xCoords = xCoords + locations[i].X + ".";
                        yCoords = yCoords + locations[i].Y + ".";
                    }
                    string goodxs = string.Empty;
                    string goodys = string.Empty;
                    for (int i = 0; i < xCoords.Length - 1; i++)
                    {
                        goodxs = goodxs + xCoords[i];
                        goodys = goodys + yCoords[i];
                    }
                    response = response + goodxs + "|" + goodys;
                    return response;
                }
            }
            return "fail;";
        }

        private string ProcessShoot(string msg)
        {
            string[] content = msg.Split(";");
            string shootingPlayer = content[2];
            string[] coords = content[1].Split(",");
            LocationVector shotLocation = new LocationVector(int.Parse(coords[0]), int.Parse(coords[1]));
            lastShotLocation = shotLocation;
            logger.AddLog(new LevLog(LogLevel.LogInfo, $"(ProcessShoot) Player {shootingPlayer} is shooting at x: {shotLocation.X}, y: {shotLocation.Y}"));
            foreach (Player player in this.players)
            {
                if (!player.Name.Equals(shootingPlayer))
                {
                    nextPlayer = player.Name;
                    int targetPlayerShipsCount = player.ShipsAlive;
                    bool niceShot = player.GetShot(shotLocation);
                    if (niceShot)
                    {
                        lastEventStr = "dmg";
                        logger.AddLog(new LevLog(LogLevel.LogInfo, $"(ProcessShoot) {shootingPlayer} eltalalta {player.Name}-t"));
                        if (player.ShipsAlive == 0)
                        {
                            logger.AddLog(new LevLog(LogLevel.LogInfo, $"(ProcessShoot) Vege a jateknak! {shootingPlayer} nyert!"));
                            gameOver = true;
                            winnerPlayer = shootingPlayer;
                        }
                        else if (player.ShipsAlive == targetPlayerShipsCount - 1)
                        {
                            logger.AddLog(new LevLog(LogLevel.LogInfo, $"(ProcessShoot) {player.Name} {player.ShipSizeByCoords(shotLocation)} hosszu hajoja elsullyedt."));
                            lastEventStr = "died_ship";
                            lastShipSize = player.ShipSizeByCoords(shotLocation);
                            return $"shrank;{shotLocation.X},{shotLocation.Y};{player.ShipSizeByCoords(shotLocation)}";
                        }
                        logger.AddLog(new LevLog(LogLevel.LogInfo, $"(ProcessShoot) {nextPlayer} kovetkezik."));
                        return $"nice;{shotLocation.X},{shotLocation.Y}";
                    }
                    else
                    {
                        lastEventStr = "nothing";
                    }
                    logger.AddLog(new LevLog(LogLevel.LogInfo, $"(ProcessShoot) {shootingPlayer} nem talalta el {player.Name}-t"));
                    nextPlayer = player.Name;
                    logger.AddLog(new LevLog(LogLevel.LogInfo, $"(ProcessShoot) {nextPlayer} kovetkezik."));
                }
            }
            return $"empty;{shotLocation.X},{shotLocation.Y}";
        }

        private string ProcessInfo(string msg)
        {
            //logger.AddLog(new LevLog(LogLevel.LogDebug, $"(ProcessInfo) processing info: {msg}"));
            string[] content = msg.Split(";");
            if (gameOver)
            {
                return $"game_over;{winnerPlayer}";
            }
            if (content[1].Equals(nextPlayer))
            {
                if (lastEventStr == null)
                {
                    return "you_next;first";
                }
                else if (lastEventStr.Equals("nothing"))
                {
                    return $"you_next;nothing;{lastShotLocation.X},{lastShotLocation.Y}";
                }
                else if (lastEventStr.Equals("dmg"))
                {
                    return $"you_next;dmg;{lastShotLocation.X},{lastShotLocation.Y}";
                }
                else if (lastEventStr.Equals("died_ship"))
                {
                    return $"you_next;died_ship;{lastShotLocation.X},{lastShotLocation.Y};{lastShipSize}";
                }
            }
            return "wait;";
        }

        private void ProcessNewPlayer(string msg)
        {
            List<Ship> shipsList = new List<Ship>();

            string[] content = msg.Split(";");
            string name = content[2];
            string[] shipsData = content[1].Split(",");
            foreach (string shipData in shipsData)
            {
                string[] tmp = shipData.Split("{");
                int shipSize = int.Parse(tmp[0]);
                string[] x_y_coords = tmp[1].Split("|");
                string[] xCoords = x_y_coords[0].Split(".");
                string[] yCoords = x_y_coords[1].Split(".");
                List<LocationVector> locations = new List<LocationVector>();
                for (int i = 0; i < shipSize; i++)
                {
                    LocationVector loc = new LocationVector(int.Parse(xCoords[i]), int.Parse(yCoords[i]));
                    locations.Add(loc);
                }
                shipsList.Add(new Ship(locations.ToArray()));
            }
            players.Add(new Player(name, shipsList.ToArray()));
            if (players.Count == 2)
            {
                nextPlayer = name;
            }
        }
    }
}
