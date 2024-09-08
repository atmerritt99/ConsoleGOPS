using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGOPS
{
    public class EvolutionaryMlpAiPlayer : MlpAiPlayer
    {
        public double[] Scores = new double[6];
        public double Score => Scores.Sum() / Scores.Length;
        public double Fitness = 0;

        public EvolutionaryMlpAiPlayer()
        {

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
    }
}
