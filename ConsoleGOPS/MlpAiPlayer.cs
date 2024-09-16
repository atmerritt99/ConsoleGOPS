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

        private MlpAiPlayer(string path)
        {
            Brain = new NeuralNetwork(path);
        }

        public static MlpAiPlayer Create(string path)
        {
            return new MlpAiPlayer(path);
        }

        protected MlpAiPlayer()
        {
            Brain = new NeuralNetwork();
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

			for (int i = 0; i < outputs.Length; i++)
			{
                // Cube the output for better AI selection
                outputs[i] *= outputs[i];
				outputs[i] *= outputs[i];
			}

			float sum = outputs.Sum();
			var filteredNormalizedOutputs = new List<float>();

            for(int i = 0; i < outputs.Length; i++)
            {
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

            return Hand.SelectCardByValue((Value)(nnSelection + (int)Value.TWO)); //The outputs are length 13 but the values are 2 - 14. Add 2 to account for that.
        }
    }
}
