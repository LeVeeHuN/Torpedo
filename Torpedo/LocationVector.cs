namespace Torpedo
{
    public class LocationVector
    {
        private int x;
        private int y;

        public int X { get { return x; } }
        public int Y { get { return y; } }

        public LocationVector(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool IsSame(LocationVector other)
        {
            if (other.X == x && other.Y == y)
            {
                return true;
            }
            return false;
        }
    }
}
