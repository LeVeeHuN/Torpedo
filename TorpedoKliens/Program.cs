using System.ComponentModel;
using Torpedo;

namespace TorpedoKliens
{
    internal class Program
    {

        enum Columns { A, B, C, D, E, F, G, H, I, J }

        static bool CheckInput(string input)
        {
            if (input.Length == 2)
            {
                string first_num = input[0].ToString();
                string second_char = input[1].ToString();
                try
                {
                    int.Parse(first_num);
                    Enum.Parse(typeof(Columns), second_char);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        static LocationVector GetLocation(Game game, bool useTriedShoots)
        {
            string input = string.Empty;
            while (true)
            {
                input = Console.ReadLine().ToUpper();
                if (CheckInput(input))
                {
                    break;
                }
                else
                {
                    Console.Write("Érvénytelen kordináta! Próbáld újra: ");
                }
            }
            string column = input[input.Length - 1].ToString();
            string row = input.Replace(column, string.Empty);
            int columnNum = (int)Enum.Parse(typeof(Columns), column);
            LocationVector selectedLocation = new LocationVector(int.Parse(row), columnNum);

            if (useTriedShoots)
            {
                if (!game.IsInTriedLocations(selectedLocation))
                {
                    game.AddTriedLocation(selectedLocation);
                    return selectedLocation;
                }
                Console.Write("Ide már lőttél! Próbáld újra: ");
                return GetLocation(game, true);
            }
            return selectedLocation;
        }

        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Helyi hálózaton belül a szerver hálózaton belüli ip címét kell megadni.");
            Console.WriteLine("Távoli eléréshez, használd a publikus ip-d és forwardold a 5100-as portot a szervert futtató gépre a routeredből.");
            Console.Write("Add meg a szerver ip-címét: ");
            string serverIp = Console.ReadLine();
            Console.Clear();
            Console.Write("Add meg a neved: ");
            string name = Console.ReadLine();
            Console.Clear();
            Game game = new Game(name);

            Communicator communicator = new Communicator(serverIp);

            string newPlayerStr = "new_player;";
            for (int i = 1; i <= 5; i++)
            {
                Console.Clear();

                newPlayerStr = newPlayerStr + i.ToString() + "{";

                string xS = string.Empty;
                string yS = string.Empty;

                for (int j = 1; j <= i; j++)
                {
                    Console.Write($"Add meg az {i}. hajo {j}. koordinatajat: ");
                    LocationVector hajoReszLocation = GetLocation(game, false);

                    if (j == i)
                    {
                        xS = xS + hajoReszLocation.X;
                        yS = yS + hajoReszLocation.Y;
                    }
                    else
                    {
                        xS = xS + hajoReszLocation.X + ".";
                        yS = yS + hajoReszLocation.Y + ".";
                    }
                }
                newPlayerStr = newPlayerStr + xS + "|" + yS;
                if (i != 5)
                {
                    newPlayerStr = newPlayerStr + ",";
                }
            }
            newPlayerStr = newPlayerStr + ";" + name;
            communicator.Communicate(newPlayerStr);

            //game.DrawMap();
            //Console.ReadLine();

            string infostring = $"info;{name}";
            Console.ForegroundColor = ConsoleColor.Gray;
            game.DrawMap();

            // main loop
            while (true)
            {
                // info kerese
                System.Threading.Thread.Sleep(1500);
                string response = communicator.Communicate(infostring);
                response = response.Replace("<EOF>", string.Empty);
                string[] content = response.Split(";");
                string command = content[0];
                // info valasz feldolgozasa
                if (command.Equals("wait"))
                {

                }
                else if (command.Equals("game_over"))
                {
                    Console.Clear();
                    if (name.Equals(content[1]))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Nyertél!");
                        Console.ReadLine();
                        break;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Vesztettél! {content[1]} nyert.");
                        Console.ReadLine();
                        break;
                    }
                }
                else if (command.Equals("you_next"))
                {
                    string subcommand = content[1];
                    if (subcommand.Equals("first"))
                    {
                        game.DrawMap();
                        Console.WriteLine("Add meg a lövésed helyét: ");
                        LocationVector shotTarget = GetLocation(game, true);
                        string shotResponse = communicator.Communicate($"shot;{shotTarget.X},{shotTarget.Y};{name}");
                        ProcessShotResponse(shotResponse, game, communicator);
                    }
                    else if (subcommand.Equals("nothing"))
                    {
                        string[] cords = content[2].Split(",");
                        Console.WriteLine($"Lövés érkezett: x: {cords[0]}, y: {(Columns)int.Parse(cords[1])}\nNem talált el semmit.");
                        Console.WriteLine("Nyomj entert a folytatáshoz!");
                        Console.ReadLine();
                        Console.WriteLine("Add meg a lövésed helyét: ");
                        LocationVector shotTarget = GetLocation(game, true);
                        string shotResponse = communicator.Communicate($"shot;{shotTarget.X},{shotTarget.Y};{name}");
                        ProcessShotResponse(shotResponse, game, communicator);
                    }
                    else if (subcommand.Equals("dmg"))
                    {
                        string[] cords = content[2].Split(",");
                        Console.WriteLine($"Lövés érkezett: x: {cords[0]}, y: {(Columns)int.Parse(cords[1])}\nEltalált egy hajót..");
                        Console.WriteLine("Nyomj entert a folytatáshoz!");
                        Console.ReadLine();
                        Console.WriteLine("Add meg a lövésed helyét: ");
                        LocationVector shotTarget = GetLocation(game, true);
                        string shotResponse = communicator.Communicate($"shot;{shotTarget.X},{shotTarget.Y};{name}");
                        ProcessShotResponse(shotResponse, game, communicator);
                    }
                    else if (subcommand.Equals("died_ship"))
                    {
                        string[] cords = content[2].Split(",");
                        Console.WriteLine($"Lövés érkezett: x: {cords[0]}, y: {(Columns)int.Parse(cords[1])}\nElsüllyedt a {content[3]} hosszú hajód!");
                        Console.WriteLine("Nyomj entert a folytatáshoz!");
                        Console.ReadLine();
                        Console.WriteLine("Add meg a lövésed helyét: ");
                        LocationVector shotTarget = GetLocation(game, true);
                        string shotResponse = communicator.Communicate($"shot;{shotTarget.X},{shotTarget.Y};{name}");
                        ProcessShotResponse(shotResponse, game, communicator);
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        static void ProcessShotResponse(string response, Game game, Communicator communicator)
        {
            string[] content = response.Split(';');
            // shot responseok feldolgozása és a gameben beállítása
            // Ha elsullyedt akkor ship_coords kérés és gameben átállítás
            if (content[0].Equals("empty"))
            {
                string[] cords = content[1].Split(",");
                string x = cords[0];
                string y = cords[1];
                game.AddEmpty(int.Parse(x), int.Parse(y));
                game.DrawMap();
            }
            else if (content[0].Equals("nice"))
            {
                string[] cords = content[1].Split(",");
                string x = cords[0];
                string y = cords[1];
                game.AddNiceShot(int.Parse(x), int.Parse(y));
                game.DrawMap();
            }
            else if (content[0].Equals("shrank"))
            {
                int shrunkShipSize = int.Parse(content[2]);
                string shipCordsResponse = communicator.Communicate($"ship_coords;{game.UserName};{shrunkShipSize}");
                string cordsToProcess = shipCordsResponse.Split("{")[1];
                string[] x_y_cords = cordsToProcess.Split("|");
                string[] xCords = x_y_cords[0].Split(".");
                string[] yCords = x_y_cords[1].Split(".");
                List<LocationVector> shrunkCords = new List<LocationVector>();
                for (int i = 0; i < xCords.Length; i++)
                {
                    LocationVector currentLocation = new LocationVector(int.Parse(xCords[i]), int.Parse(yCords[i]));
                    shrunkCords.Add(currentLocation);
                }
                game.AddShrunkShip(shrunkCords.ToArray());
                game.DrawMap();
            }
        }
        
    }
}
