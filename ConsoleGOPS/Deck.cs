using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGOPS
{
    public class Deck
    {
        Random _random;
        public List<Card> Cards { get; }
        public bool IsEmpty => Cards.Count <= 0;
        public int Count => Cards.Count;

        /// <summary>
        /// Creates a 52 card deck in new deck order (2 through Ace in CHaSeD order)
        /// </summary>
        /// <param name="empty">if true, then create an empty deck. Otherwise, create a full 52 card deck</param>
        public Deck(bool empty = false)
        {
            Cards = new List<Card>();
            _random = new Random();

            if (empty)
                return;

            for(int suitNum = (int)Suit.CLUB; suitNum <= (int)Suit.DIAMOND; suitNum++)
            {
                for(int valueNum = (int)Value.TWO; valueNum <= (int)Value.ACE; valueNum++)
                {
                    Cards.Add(new Card((Value)valueNum, (Suit)suitNum));
                }
            }
        }

        /// <summary>
        /// Creates a new deck of cards from the cards given
        /// </summary>
        public Deck(IEnumerable<Card> cards)
        {
            Cards = cards.ToList();
            _random = new Random();
        }

        /// <summary>
        /// Shuffles the order of the cards
        /// </summary>
        public void Shuffle()
        {
            for(int i = 0; i < 100000; i++)
            {
                int idxCard1 = _random.Next(Cards.Count);
                int idxCard2 = _random.Next(Cards.Count);

                Card swap = Cards[idxCard1];
                Cards[idxCard1] = Cards[idxCard2];
                Cards[idxCard2] = swap;
            }
        }

        /// <summary>
        /// Draw a card from the top of the deck
        /// </summary>
        public Card Draw()
        {
            Card card = Cards[0];
            Cards.RemoveAt(0);
            return card;
        }

        /// <summary>
        /// Return card to the top of the deck
        /// </summary>
        public void Return(Card card)
        {
            Cards.Insert(0, card);
        }

		public void RemoveCard(Card card)
		{
			Cards.Remove(card);
		}

		public Card SelectCard(int idxOfCard)
        {
            Card selectedCard = Cards[idxOfCard];
            return selectedCard;
        }

        /// <summary>
        /// Add Cards to deck
        /// </summary>
        /// <param name="cards"></param>
        public void AddCards(IEnumerable<Card> cards)
        {
            Cards.AddRange(cards);
        }

        /// <summary>
        /// Get cards of a suit
        /// </summary>
        public IEnumerable<Card> GetCardsOfSuit(Suit suit)
        {
            return Cards.Where(c => c.Suit == suit);
        }

        public Card SelectCardByValue(Value value)
        {
            int selectedIdx = Cards.FindIndex(c => c.Value == value);
            return SelectCard(selectedIdx);
        }

        public bool IsValuePresent(Value value)
        {
            return Cards.Any(c => c.Value == value);
        }
    }
}
