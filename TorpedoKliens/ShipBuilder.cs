using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Torpedo;

namespace TorpedoKliens
{
    public class ShipBuilder
    {
        private static char[,] allSelectedCords;
        private static char[,] currentSelectedCords;
        private static List<LocationVector> currentShipLocations;
        private static List<List<LocationVector>> everyShipLocations;

        public static string BuildShip()
        {
            // Előkészületek
            currentShipLocations = new List<LocationVector>();
            everyShipLocations = new List<List<LocationVector>>();
            allSelectedCords = new char[10, 10];
            currentSelectedCords = new char[10, 10];
            for (int i = 0; i < allSelectedCords.GetLength(0); i++)
            {
                for (int j = 0; j < allSelectedCords.GetLength(1); j++)
                {
                    allSelectedCords[i, j] = '*';
                    currentSelectedCords[i, j] = '*';
                }
            }
            Console.Clear();

            int currentShipLen = 1;
            int currentlySelected = 0;

            int x = 0;
            int y = 0;
            PrintCurrent(x, y);
            while (currentShipLen <= 5)
            {
                ConsoleKeyInfo input = Console.ReadKey();
                ConsoleKey pressed = input.Key;

                switch (pressed)
                {
                    case ConsoleKey.LeftArrow:
                        if (y > 0)
                        {
                            y -= 1;
                            PrintCurrent(x, y);
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (y < 9)
                        {
                            y += 1;
                            PrintCurrent(x, y);
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        if(x > 0)
                        {
                            x -= 1;
                            PrintCurrent(x, y);
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (x < 9)
                        {
                            x += 1;
                            PrintCurrent(x, y);
                        }
                        break;
                    case ConsoleKey.Spacebar:
                        if (CheckPlacement(x, y))
                        {
                            currentlySelected++;
                            LocationVector newLoc = new LocationVector(x, y);
                            allSelectedCords[x, y] = 'X';
                            currentShipLocations.Add(newLoc);
                        }
                        else
                        {
                            Console.WriteLine("\nIllegális hajó hely! Rakd máshova a hajódat.");
                            Console.WriteLine("Nyomj enter-t a folytatáshoz...");
                            Console.ReadLine();
                        }
                        if (currentShipLocations.Count == currentShipLen)
                        {
                            currentShipLen++;
                            List<LocationVector> shipFckingLocs = new List<LocationVector>();
                            foreach (LocationVector location in currentShipLocations)
                            {
                                shipFckingLocs.Add(location);
                            }
                            everyShipLocations.Add(shipFckingLocs);
                            currentShipLocations = new List<LocationVector>();
                        }
                        PrintCurrent(x, y);
                        break;
                    case ConsoleKey.Enter:
                        // Handle done and check if every ship is placed
                        break;
                    default:
                        break;
                }
            }
            Console.WriteLine("\nMinden hajó készen áll.\nNyomj enter-t a folytatáshoz!");
            Console.ReadLine();

            string shipLocationsStr = string.Empty;
            for (int i = 0; i < everyShipLocations.Count; i++)
            {
                List<LocationVector> currentShipLocations = everyShipLocations[i];
                shipLocationsStr = $"{shipLocationsStr}{i+1}{{";

                string xs = string.Empty;
                string ys = string.Empty;
                foreach (LocationVector location in currentShipLocations)
                {
                    xs = xs + location.X + ".";
                    ys = ys + location.Y + ".";
                }
                xs = xs.Remove(xs.Length - 1);
                ys = ys.Remove(ys.Length - 1);
                shipLocationsStr = shipLocationsStr + xs + "|" + ys + ",";
            }
            shipLocationsStr = shipLocationsStr.Remove(shipLocationsStr.Length - 1);
            shipLocationsStr = shipLocationsStr + ";";
            return shipLocationsStr;

        }

        private static bool CheckPlacement(int x, int y)
        {
            LocationVector currentLocation = new LocationVector(x, y);
            
            // Check, h letehető-e a többi miatt
            foreach (List<LocationVector> shipLocs in everyShipLocations)
            {
                foreach (LocationVector shipLoc in shipLocs)
                {
                    if (shipLoc.IsSame(currentLocation))
                    {
                        return false;
                    }

                    if (shipLoc.IsInRange(currentLocation))
                    {
                        return false;
                    }
                }
            }

            // Random safety check
            foreach(LocationVector safetyLocation in currentShipLocations)
            {
                if (safetyLocation.IsSame(currentLocation))
                {
                    return false;
                }
            }

            // Check, hogy a sajátja mellé rakjuk
            if (currentShipLocations.Count == 0)
            {
                return true;
            }
            //      Melyik tengelyre terjeszkedik x vagy y?
            if (currentShipLocations.Count >= 2)
            {
                int xDiff = currentShipLocations[0].X - currentShipLocations[1].X;
                int yDiff = currentShipLocations[0].Y - currentShipLocations[1].Y;

                if (xDiff == 0)
                {
                    // X tengelyen húzódik
                    if (currentLocation.X != currentShipLocations[0].X)
                    {
                        return false;
                    }
                }
                else if (yDiff == 0)
                {
                    // Y tengelyen húzódik
                    if (currentLocation.Y != currentShipLocations[0].Y)
                    {
                        return false;
                    }
                }
            }
            foreach (LocationVector thisShipLocation in currentShipLocations)
            {
                if (thisShipLocation.IsNextTo(currentLocation))
                {
                    return true;
                }
            }
            return false;
        }

        private static void PrintCurrent(int currentx, int currenty)
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
                    //  * X
                    Console.SetCursorPosition(calculatedColumn, calculatedRow);
                    if (!(currentx == row && currenty == column))
                    {
                        switch (allSelectedCords[row, column])
                        {
                            case 'X':
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            case '*':
                                Console.ForegroundColor = ConsoleColor.Gray;
                                break;
                            default:
                                Console.ForegroundColor = ConsoleColor.Gray;
                                break;
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    }
                    Console.Write(allSelectedCords[row, column]);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
        }
    }
}
