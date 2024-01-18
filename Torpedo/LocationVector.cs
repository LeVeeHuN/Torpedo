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

        public bool IsInRange(LocationVector other)
        {
            if (IsNextTo(other))
            {
                return true;
            }

            if (Math.Abs(other.X - x) == 1 && Math.Abs(other.Y - y) == 1)
            {
                return true;
            }
            return false;
        }

        public bool IsNextTo(LocationVector other)
        {
            if (other.X == x && Math.Abs(other.Y - y) == 1)
            {
                return true;
            }
            else if (other.Y == y && Math.Abs(other.X - x) == 1)
            {
                return true;
            }
            return false;
        }
    }
}
