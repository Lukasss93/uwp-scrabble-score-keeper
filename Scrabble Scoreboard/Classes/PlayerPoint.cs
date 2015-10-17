using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrabble_Scoreboard.Classes
{
    public class PlayerPoint
    {
        public Players player { get; set; }
        public int index { get; set; }

        public PlayerPoint(Players p, int i)
        {
            player = p;
            index = i;
        }
    }
}
