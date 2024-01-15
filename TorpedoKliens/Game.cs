using Torpedo;

namespace TorpedoKliens
{
    internal class Game
    {
        private char[,] map;
        private string userName;
        private List<LocationVector> triedLocations;

        public string UserName { get { return userName; } }

        public Game(string userName)
        {
            this.userName = userName;
            triedLocations = new List<LocationVector>();
            map = new char[10, 10];
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    map[x, y] = 'U';
                }
            }
        }

        public bool IsInTriedLocations(LocationVector location)
        {
            foreach(LocationVector currentLocation in triedLocations)
            {
                if (location.IsSame(currentLocation))
                {
                    return true;
                }
            }
            return false;
        }

        public void AddTriedLocation(LocationVector location)
        {
            triedLocations.Add(location);
        }

        public void DrawMap()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            int offset = 1;
            Console.WriteLine("  A B C D E F G H I J");
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(i);
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;
            int posFromTop = Console.GetCursorPosition().Top;
            int posFromLeft = Console.GetCursorPosition().Left;

            for (int row = 0; row < 10; row++)
            {
                for (int column = 0; column < 10; column++)
                {
                    int calculatedColumn = column + offset + (column + 1);
                    int calculatedRow = row + offset;
                    // Lehetőségek
                    //  U S X +
                    Console.SetCursorPosition(calculatedColumn, calculatedRow);
                    switch (map[row, column])
                    {
                        case 'U':
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                        case 'S':
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case 'X':
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        case '+':
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                    }
                    Console.Write(map[row, column]);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
            Console.SetCursorPosition(posFromLeft, posFromTop);
        }

        public void AddEmpty(int x, int y)
        {
            map[x, y] = 'X';
        }

        public void AddNiceShot(int x, int y)
        {
            map[x, y] = '+';
        }

        public void AddShrunkShip(LocationVector[] shipLocations)
        {
            foreach (LocationVector shipLocation in shipLocations)
            {
                map[shipLocation.X, shipLocation.Y] = 'S';
            }
        }


    }
}
