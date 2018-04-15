using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Кодабра
{
    public class Player
    {
        public int X, Y;
        public int Direction;
        public bool Key;

        public Player(int x, int y, int direction)
        {
            X = x;
            Y = y;
            Direction = direction;
        }
    }
}
