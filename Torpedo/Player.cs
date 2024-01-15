namespace Torpedo
{
    internal class Player
    {
        private string name;
        private Ship[] ships;
        private bool alreadyInitialized;

        public string Name { get { return name; } }
        public int ShipsAlive {  get { return CheckShipsAliveness(); } }
        public bool AlreadyInitialized { get { return alreadyInitialized; } }
        //public Ship[] Ships { get {  return ships; } }

        public Player(string name, Ship[] ships)
        {
            this.name = name;
            this.ships = ships;
            this.alreadyInitialized = false;
        }

        public void Initialized()
        {
            this.alreadyInitialized = true;
        }

        public bool GetShot(LocationVector shotLocation)
        {
            foreach (Ship ship in ships)
            {
                if (ship.Shoot(shotLocation))
                {
                    return true;
                }
            }
            return false;
        }

        public LocationVector[] GetShipLocationsBySize(int shipSize)
        {
            foreach (Ship ship in ships)
            {
                if (ship.Size == shipSize)
                {
                    return ship.Locations;
                }
            }
            return new LocationVector[0];
        }

        public int ShipSizeByCoords(LocationVector location)
        {
            foreach (Ship ship in ships)
            {
                foreach (LocationVector shipLocation in ship.Locations)
                {
                    if (location.X == shipLocation.X &&  location.Y == shipLocation.Y)
                    {
                        return ship.Size;
                    }
                }
            }
            return 0;
        }

        private int CheckShipsAliveness()
        {
            int shipsAlive = 0;
            foreach (Ship ship in ships)
            {
                if (!ship.IsSink)
                {
                    shipsAlive++;
                }
            }
            return shipsAlive;
        }
    }
}
