using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGOPS
{
    public class GOPS
    {

        private Deck _prizeDeck;

        /// <summary>
        /// Prize cards current available to bid on
        /// </summary>
        public List<Card> Prizes { get; }

        public List<Player> Players { get; }
        public int IdxOfWinner { get; private set; }

        public IEnumerable<int> Rankings { get; private set; }

        public int GetTotalValueOfPrizes()
        {
            int totalPrizeValue = 0;

            foreach (Card prize in Prizes)
            {
                totalPrizeValue += prize.GetCardValue();
            }

            return totalPrizeValue;
        }

        public GOPS(IEnumerable<Player> players)
        {
            Players = players.ToList();

            if (Players.Count < 2)
                throw new Exception($"{this.Players.Count} is less than 2, the minimum number of players that can play GOPS");

            if (Players.Count > 7)
                throw new Exception($"{this.Players.Count} is greater than 7, the maximum number of players that can play GOPS");

            //Create each players hand
            var deck = new Deck();
            int suitNum = (int)Suit.CLUB;
            foreach(Player player in players)
            {
                if(suitNum > (int)Suit.DIAMOND)
                {
                    suitNum = (int)Suit.CLUB;
                }
                var cardsOfASuit = deck.GetCardsOfSuit((Suit)suitNum);
                player.Hand = new Deck(cardsOfASuit);
                player.CollectedCards = new Deck(true);
                suitNum++;
            }

            var prizeDeck = deck.GetCardsOfSuit(Suit.DIAMOND);
            _prizeDeck = new Deck(prizeDeck);
            _prizeDeck.Shuffle();

            Prizes = new List<Card>();
            Rankings = new List<int>();
        }

        /// <summary>
        /// Plays the game of GOPS
        /// </summary>
        /// <param name="display">if true display results of the game, default is false</param>
        public void Play(bool display = false)
        {
            while(!_prizeDeck.IsEmpty)
            {
                //Display Prize Cards
                Prizes.Add(_prizeDeck.Draw());

                if(display)
                {
                    Console.WriteLine("Prizes:");

                    foreach (Card prize in Prizes)
                    {
                        Console.WriteLine(prize.ToString());
                    }
                }


                //Players place bids
                List<Card> cardBids = new List<Card>();
                foreach (Player player in Players)
                {
                    cardBids.Add(player.PlaceBet(this));
                }

                //Calculate who won the bid
                //Also Display all of the bids
                int maxBid = 0;
                int playerIdxOfMaxBid = -1;
                bool isTie = false;
                int playerIdx = 0;

                if(display)
                    Console.WriteLine("Bids:");

                foreach(Card card in cardBids)
                {
                    if(display)
                        Console.WriteLine($"Player {playerIdx} bid {card}");

                    int bid = card.GetCardValue();

                    if (bid == maxBid)
                    {
                        isTie = true;
                    }

                    if (bid > maxBid)
                    {
                        isTie = false;
                        maxBid = bid;
                        playerIdxOfMaxBid = playerIdx;
                    }

                    playerIdx++;
                }
                
                if(!isTie)
                {
                    if(display)
                        Console.WriteLine($"Player {playerIdxOfMaxBid} won the Prize Cards!");
                    Players[playerIdxOfMaxBid].CollectedCards.AddCards(Prizes);
                    Prizes.Clear();
                }
                else
                {
                    if(display)
                        Console.WriteLine("There was a tie!");
                }
            }

            //Game has ended, determine winner
            int idx = 0;
            int maxIdx = -1;
            int maxScore = 0;
            if(display)
                Console.WriteLine("Final Score Board:");
            foreach (Player player in Players)
            {
                int score = player.GetCurrentScore();
                if(display)
                    Console.WriteLine($"Player {idx} has a score of {score}");
                if (score > maxScore)
                {
                    maxIdx = idx;
                    maxScore = score;
                }
                idx++;
            }

            if(display)
                Console.WriteLine($"Player {maxIdx} has the highest score of {maxScore}");

            var temp = new List<Player>(Players.ToArray()).Select(p => p.GetCurrentScore()).ToList();
            temp.Sort();
            temp.Reverse();
            Rankings = temp;
            IdxOfWinner = maxIdx;
        }
    }
}
