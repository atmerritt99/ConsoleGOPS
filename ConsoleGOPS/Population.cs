using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace ConsoleGOPS
{
    public class Population
    {
        private List<EvolutionaryMlpAiPlayer> _population;
        //private List<EvolutionaryMlpAiPlayer> _bestOverallAis;
        private EvolutionaryMlpAiPlayer _bestOverallAi;

        private EvolutionaryMlpAiPlayer CopyAI(EvolutionaryMlpAiPlayer player)
        {
            return new EvolutionaryMlpAiPlayer(player?.Brain ?? new NeuralNetwork());
		}

        private async Task PlayGames(int playerCount)
        {
            await Task.Run(() =>
            {
				for (int i = 0; i < _population.Count; i++)
				{
					var players = new List<Player>() { CopyAI(_population[i]) };

                    for(int j = players.Count; j < playerCount; j++)
                    {
                        players.Add(CopyAI(_bestOverallAi));
                    }

                    var gops = new GOPS(players);
                    gops.Play();

					//Calculate the score
					var bestScoreExcludingMe = gops.IdxOfWinner == 0 ? gops.Rankings.ToList()[1] : gops.Rankings.ToList()[0];
					_population[i].Scores[playerCount - 2] = players[0].GetCurrentScore() - bestScoreExcludingMe;
				}
			});
		}

		public Population(int populationSize, int[] layers)
        {
            _population = new List<EvolutionaryMlpAiPlayer>();

            for(int i = 0; i < populationSize; i++)
            {
                _population.Add(new EvolutionaryMlpAiPlayer(layers));
            }

            _bestOverallAi = CopyAI(_population[0]);
		}

        public MlpAiPlayer Run(int maxGenNum, string path)
        {
            for(int gen = 1; gen <= maxGenNum; gen++)
            {
                Console.WriteLine($"Generation {gen} / {maxGenNum}");

                Console.WriteLine("Playing Games...");
                var task1 = PlayGames(2);
                var task2 = PlayGames(3);
                var task3 = PlayGames(4);
                var task4 = PlayGames(5);
                var task5 = PlayGames(6);
                var task6 = PlayGames(7);

                Task.WaitAll(task1, task2, task3, task4, task5, task6);
                Console.WriteLine("Finished Games");

                //min ensures that the fitness is easier to calculate
                double min = _population.Select(p => p.Score).Min();
                if (min <= 0)
                {
                    min *= -1;
                    min++;
                }

                //Calculate the fitness of each
                double sumOfScores = _population.Sum(p => p.Score + min);
                double previousFitness = 0;
                foreach(var pop in _population)
                {
                    pop.Fitness = ((pop.Score + min) / sumOfScores) + previousFitness;
                    previousFitness = pop.Fitness;
                }

                //Figure out the best individual
                double maxScore = _population.Max(p => p.Score);
                var bestGenerationsAi = _population.Find(p => p.Score == maxScore);

                //If the best AI this generattion beat the best overall AI, set the best overall AI to the best AI this generation
                if (maxScore > 0)
                {
                    _bestOverallAi = CopyAI(bestGenerationsAi ?? new EvolutionaryMlpAiPlayer());
                    _bestOverallAi.Brain.Save(path);
                }

				Console.WriteLine($"Best Score this Generation: {maxScore}");

                //Asexual Reproduction and Mutation
                var newPopulation = new List<EvolutionaryMlpAiPlayer>();
                foreach(var pop in _population)
                {
                    newPopulation.Add(PickOne());
                }

                //Update Population
                _population = newPopulation;

                Console.WriteLine();
            }

            //_bestOverallAis[0].Brain.Save(path);
            _bestOverallAi.Brain.Save(path);
            var mlp = new MlpAiPlayer(path);
            return mlp;
        }

        private EvolutionaryMlpAiPlayer PickOne()
        {
            var random = new Random();
            double r = random.NextDouble();
            var child = _population.Last(); // If the random number is less than every members fitness, assume the last one is selected

            foreach(var pop in _population)
            {
                if (r > pop.Fitness)
                    continue;

                child = new EvolutionaryMlpAiPlayer(pop.Brain);
                break;
            }

            r = random.NextDouble();

            if(r < .4) // Preserve some of previous generation in case I get bad luck with mutations
                child.Mutate(.1);

            return child;
        }
    }
}
