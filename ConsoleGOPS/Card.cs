using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGOPS
{
    public class Card
    {
        public Suit Suit { get; set; }
        public Value Value { get; set; }

        public Card(Value _value, Suit _suit)
        {
            Suit = _suit;
            Value = _value;
        }

        public int GetCardValue()
        {
            return (int)Value;
        }

        public override string ToString()
        {
            return $"{Value} OF {Suit}S";
        }
    }
}
