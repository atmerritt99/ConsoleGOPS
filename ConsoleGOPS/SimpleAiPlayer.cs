using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGOPS
{
    public class SimpleAiPlayer : Player
    {
        private List<Card> _previousBids = new List<Card>();
        public override Card PlaceBet(GOPS gops)
        {
            // I know the first card in my hand will be the lowest
            // If I cannot select a better card then play my lowest card
            int idxOfBid = 0;

            int prizeTotal = gops.GetTotalValueOfPrizes();

            //If only 1 prize card is up for grabs then ignore previous plays
            if (gops.Prizes.Count == 1)
                _previousBids.Clear();

            //Determine how much I need to bid to match the prize total evenly
            int match = prizeTotal;
            foreach (Card pbid in _previousBids)
            {
                match -= pbid.GetCardValue();
            }
            
            //Select all bids I can make that will match the total or higher
            List<int> choices = new List<int>();
            int idx = -1;
            foreach(Card card in Hand.Cards)
            {
                idx++;
                int cardValue = card.GetCardValue();

                if (cardValue < match)
                    continue;

                choices.Add(idx);
            }

            //Randomly select bid if I found good options above
            if(choices.Count > 0)
            {
                Random random = new Random();
                int r = random.Next(choices.Count);
                idxOfBid = choices[r];
            }

            Card bid = Hand.SelectCard(idxOfBid);
            _previousBids.Add(bid);
            return bid;
        }
    }
}
