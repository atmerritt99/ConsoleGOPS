using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGOPS
{
    public class EvolutionaryMlpAiPlayer : Player
    {
        public double[] Scores = new double[6];
        public double Score => Scores.Sum() / Scores.Length;
        public double Fitness = 0;
        public NeuralNetwork Brain;

        public EvolutionaryMlpAiPlayer()
        {
			Brain = new NeuralNetwork();
        }

        public EvolutionaryMlpAiPlayer(int[] layers)
        {
            Brain = new NeuralNetwork(layers, .01f);
        }

        public EvolutionaryMlpAiPlayer(NeuralNetwork brain)
        {
            Brain = brain;
        }

        public void Mutate(double mutationRate)
        {
            Brain.Mutate(mutationRate);
        }

		private IEnumerable<float> GetBinaryRepresentationOfHand(Deck hand)
		{
			var binaryRepresentationOfHand = new List<float>();

			for (int i = (int)Value.TWO; i <= (int)Value.ACE; i++)
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
			foreach (var player in players)
			{
				if (player.Equals(this))
					continue;

				inputsList.AddRange(GetBinaryRepresentationOfHand(player.Hand));
				inputsList.AddRange(GetBinaryRepresentationOfHand(player.CollectedCards));
			}

			//If theres less than 7 players in the game, fill the remaining inputs with 0s
			for (int i = players.Count; i < 7; i++)
			{
				inputsList.AddRange(new float[26]);
			}

			inputsList.AddRange(GetBinaryRepresentationOfHand(new Deck(gops.Prizes)));

			var inputs = inputsList.ToArray();
			var outputs = Brain.Predict(inputs);

			float sum = outputs.Sum();
			var filteredNormalizedOutputs = new List<float>();

			for (int i = 0; i < outputs.Length; i++)
			{
				filteredNormalizedOutputs.Add((outputs[i] * mask[i]) / sum);
			}

			int nnSelection = filteredNormalizedOutputs.IndexOf(filteredNormalizedOutputs.Max()); // Select the max by default
			return Hand.SelectCardByValue((Value)(nnSelection + (int)Value.TWO)); //The outputs are length 13 but the values are 2 - 14. Add 2 to account for that.
		}
	}
}
