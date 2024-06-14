using System.Drawing;

namespace PokeHelper.util
{
    public class Pos
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
            
        public Pos(int x, int y, string name)
        {
            Name = name;
            X = x;
            Y = y;
        }
            
        public bool IsSet() => X != 0 && Y != 0;
            
        public override string ToString()
        {
            return X + "," + Y;
        }

        public Point ToPoint()
        {
            return new Point(X, Y);
        }
            
    }
}