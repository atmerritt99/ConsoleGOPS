using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGOPS
{
    public class MlpAiPlayer : Player
    {
        public NeuralNetwork Brain;
		private List<Card> _previousBids = new List<Card>();

		public MlpAiPlayer(string path)
        {
            Brain = new NeuralNetwork(path);
        }

        public MlpAiPlayer(NeuralNetwork brain)
        {
            Brain = brain;
        }

        private IEnumerable<float> GetBinaryRepresentationOfHand(Deck hand)
        {
            var binaryRepresentationOfHand = new List<float>();

            for(int i = (int)Value.TWO; i <= (int)Value.ACE; i++)
            {
                int num = hand.IsValuePresent((Value)i) ? 1 : 0;
                binaryRepresentationOfHand.Add(num);
            }

            return binaryRepresentationOfHand;
        }

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

			var inputsList = new List<float>();

            var maskList = GetBinaryRepresentationOfHand(Hand);
            var mask = maskList.ToArray();
            inputsList.AddRange(maskList);
            inputsList.AddRange(GetBinaryRepresentationOfHand(CollectedCards));

            var players = gops.Players;
            foreach(var player in players)
            {
                if (player.Equals(this))
                    continue;

                inputsList.AddRange(GetBinaryRepresentationOfHand(player.Hand));
                inputsList.AddRange(GetBinaryRepresentationOfHand(player.CollectedCards));
            }

            //If theres less than 7 players in the game, fill the remaining inputs with 0s
            for(int i = players.Count; i < 7; i++)
            {
                inputsList.AddRange(new float[26]);
            }

            inputsList.AddRange(GetBinaryRepresentationOfHand(new Deck(gops.Prizes)));

            var inputs = inputsList.ToArray();
            var outputs = Brain.Predict(inputs);

            float sum = outputs.Sum();
			var filteredNormalizedOutputs = new List<float>();

            for(int i = 0; i < outputs.Length; i++)
            {
                //If card value is above a reasonable limit, card should not be an option
                int offset = 2;
                if (i + 2 > match + offset)
                {
                    int x = i + 2 - (match + offset);
					outputs[i] /= x;
				}

                int offset2 = 1;
				if (i + 2 < match - offset2)
				{
					int x = match - offset2 - (i + 2);
					outputs[i] /= x;
				}

				filteredNormalizedOutputs.Add((outputs[i] * mask[i]) / sum);
            }

			int nnSelection = filteredNormalizedOutputs.IndexOf(filteredNormalizedOutputs.Max()); // Select the max by default

            //Randomly select a card to bid based on how the cards are weighted
            //Doing this so that player can play against multiple instances of this model, and the models will all make slightly different picks
            var random = new Random();
            float r = random.NextSingle();
            float previousOutput = 0;
            for (int i = 0; i < filteredNormalizedOutputs.Count; i++)
            {
                if (filteredNormalizedOutputs[i] == 0)
                    continue;

                if (r < filteredNormalizedOutputs[i] + previousOutput)
                {
                    nnSelection = i;
                    break;
                }

                previousOutput += filteredNormalizedOutputs[i];
            }

            var bid = Hand.SelectCardByValue((Value)(nnSelection + (int)Value.TWO)); //The outputs are length 13 but the values are 2 - 14. Add 2 to account for that.

			if (nnSelection + (int)Value.TWO < match)
            {
                bid = Hand.SelectCard(idxOfBid);
            }

            _previousBids.Add(bid);

            return bid; 
        }
    }
}
