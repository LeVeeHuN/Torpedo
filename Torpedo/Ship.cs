namespace Torpedo
{
    public class Ship
    {
        private bool isSink;
        private int size;
        private LocationVector[] locations;
        private int shots;

        public bool IsSink {  get { return isSink; } }
        public int Size { get { return size; } }
        public LocationVector[] Locations { get { return locations; } }

        public Ship(LocationVector[] locations)
        {
            this.locations = locations;
            size = locations.Length;
            isSink = false;
            shots = 0;
        }

        public bool Shoot(LocationVector shotLocation)
        {
            foreach (LocationVector shipLocation in locations)
            {
                if (shipLocation.X == shotLocation.X && shipLocation.Y == shotLocation.Y)
                {
                    shots++;
                    if (shots == size)
                    {
                        isSink = true;
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
