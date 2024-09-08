using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGOPS
{
    public abstract class Player
    {
        public Deck Hand;
        public Deck CollectedCards;
        public abstract Card PlaceBet(GOPS gops);
        public int GetCurrentScore()
        {
            int score = 0;

            foreach(Card card in CollectedCards.Cards)
            {
                score += card.GetCardValue();
            }

            return score;
        }
    }
}
