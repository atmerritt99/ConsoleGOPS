using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGOPS
{
    public class Population
    {
        private List<EvolutionaryMlpAiPlayer> _population;
        public Population(int populationSize, int[] layers)
        {
            _population = new List<EvolutionaryMlpAiPlayer>();

            for(int i = 0; i < populationSize; i++)
            {
                _population.Add(new EvolutionaryMlpAiPlayer(layers));
            }
        }

        public MlpAiPlayer Run(int maxGenNum, int earlyStop, string path)
        {
            //var bestOverallAi = _population[0];
            //Initialize the copies
            var bestOverallAis = new List<EvolutionaryMlpAiPlayer>();
            for(int i = 0; i < 6; i++)
            {
                bestOverallAis.Add(new EvolutionaryMlpAiPlayer(_population[0].Brain));
            }

            int earlyStopCounter = 0;
            for(int gen = 1; gen <= maxGenNum; gen++)
            {
                Console.WriteLine($"Generation {gen} / {maxGenNum}");

                for(int playerCount = 2; playerCount <= 7; playerCount++)
                {
                    Console.WriteLine($"Playing {playerCount} player games!");
                    for (int i = 0; i < _population.Count; i++)
                    {
                        var players = new List<EvolutionaryMlpAiPlayer>() { _population[i] };

                        //Initialize Players
                        for (int j = 0; j < playerCount - 1; j++)
                        {
                            players.Add(bestOverallAis[j]);
                        }

                        var gops = new GOPS(players);
                        gops.Play();

                        //Calculate the score
                        var bestScoreExcludingMe = gops.IdxOfWinner == 0 ? gops.Rankings.ToList()[1] : gops.Rankings.ToList()[0];
                        _population[i].Scores[playerCount - 2] = _population[i].GetCurrentScore() - bestScoreExcludingMe;

                    }
                }

                //min ensures that the fitness is easier to calculate
                double min = _population.Select(p => p.Score).Min();
                if(min <= 0)
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
                    bestOverallAis.Clear();
                    for (int i = 0; i < 6; i++)
                    {
                        bestOverallAis.Add(new EvolutionaryMlpAiPlayer(bestGenerationsAi?.Brain ?? new NeuralNetwork()));
                    }
                    earlyStopCounter = 0;
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
                earlyStopCounter++;

                if (earlyStopCounter >= earlyStop)
                {
                    Console.WriteLine($"No improvements after {earlyStop} generations. Stopping training.");
                    break;
                }
                Console.WriteLine();
            }

            bestOverallAis[0].Brain.Save(path);
            var mlp = MlpAiPlayer.Create(path);
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
