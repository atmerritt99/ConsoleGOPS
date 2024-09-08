using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGOPS
{
    public class RandomPlayer : Player
    {
        public override Card PlaceBet(GOPS gops)
        {
            Random random = new Random();
            int r = random.Next(Hand.Count);
            return Hand.SelectCard(r);
        }
    }
}
