// See https://aka.ms/new-console-template for more information
using ConsoleGOPS;

const int GAME_COUNT = 10000;

PlayGame("Mlp_Gops_v6.txt");
//TrainAndTestModel("Mlp_Gops_v6.txt");
//TestModel(new MlpAiPlayer("Mlp_Gops_v6.txt"));

MlpAiPlayer CopyAi(MlpAiPlayer mlp)
{
	return new MlpAiPlayer(mlp.Brain);
}

void TrainAndTestModel(string filepath)
{
	var mlp = TrainNewModel(filepath);
	TestModel(mlp);
}

void TestModel(MlpAiPlayer modelToTest)
{
	for(int playerCount = 2; playerCount <= 7;  playerCount++)
	{
		var task1 = RandomGames(playerCount, modelToTest);
		var task2 = SimpleAiGames(playerCount, modelToTest);

		Task.WaitAll(task1, task2);
		Console.WriteLine();
	}
}

async Task RandomGames(int playerCount, MlpAiPlayer modelToTest)
{
	await Task.Run(() =>
	{
		int wins = 0;
		int ties = 0;
		var players = new List<Player>() { CopyAi(modelToTest) };
		for (int playerCount2 = 1; playerCount2 < playerCount; playerCount2++)
		{
			players.Add(new RandomPlayer());
		}

		for (int i = 0; i < GAME_COUNT; i++)
		{
			var gops = new GOPS(players);
			gops.Play();

			if (gops.IdxOfWinner == 0)
			{
				if (gops.Rankings.ToArray()[0] == gops.Rankings.ToArray()[1])
				{
					ties++;
				}
				else
				{
					wins++;
				}
			}
		}
		Console.WriteLine($"{playerCount}-Player Random: Won: {wins} Tied: {ties}");
	});
}

async Task SimpleAiGames(int playerCount, MlpAiPlayer modelToTest)
{
	await Task.Run(() =>
	{
		int wins = 0;
		int ties = 0;
		var players = new List<Player>() { CopyAi(modelToTest) };
		for (int playerCount2 = 1; playerCount2 < playerCount; playerCount2++)
		{
			players.Add(new SimpleAiPlayer());
		}

		for (int i = 0; i < GAME_COUNT; i++)
		{
			var gops = new GOPS(players);
			gops.Play();

			if (gops.IdxOfWinner == 0)
			{
				if (gops.Rankings.ToArray()[0] == gops.Rankings.ToArray()[1])
				{
					ties++;
				}
				else
				{
					wins++;
				}
			}
		}
		Console.WriteLine($"{playerCount}-Player SimpleAiPlayer: Won: {wins} Tied: {ties}");

	});
}

MlpAiPlayer TrainNewModel(string filepath)
{
	var pop = new Population(500, new int[] { 195, 104, 13 });
	return pop.Run(500, filepath);
}

void PlayGame(string filename)
{
	var players = new List<Player>()
	{
		new HumanPlayer()
	};

	Console.WriteLine("Austin's GOPS Program:");

	bool isUserInputValid = false;
	while (!isUserInputValid)
	{
		Console.WriteLine("Enter 2nd player AI difficulty:");
		Console.WriteLine("\t1 - Easy (AI plays randomly)");
		Console.WriteLine("\t2 - Medium (AI plays using heuristics)");
		Console.WriteLine("\t3 - Hard (AI plays using a pre-trained model and heuristics)");
		Console.Write(">> ");

		var userInput = Console.ReadLine();

		if (userInput == "1")
		{
			isUserInputValid = true;
			players.Add(new RandomPlayer());
		}
		else if (userInput == "2")
		{
			isUserInputValid = true;
			players.Add(new SimpleAiPlayer());
		}
		else if (userInput == "3")
		{
			isUserInputValid = true;
			players.Add(new MlpAiPlayer(filename));
		}

		if (!isUserInputValid)
			Console.WriteLine($"ERROR: {userInput} is not valid input. Please enter 1, 2, or 3");
	}

	string ordinalIndicator = "rd";
	bool isDoneEnteringPlayers = false;
	for (int currentPlayerCount = 2; currentPlayerCount < 7; currentPlayerCount++)
	{
		if (isDoneEnteringPlayers)
			break;

		isUserInputValid = false;
		while (!isUserInputValid)
		{
			Console.WriteLine($"Enter {currentPlayerCount + 1}{ordinalIndicator} player AI difficulty:");
			Console.WriteLine("\t1 - Easy (AI plays randomly)");
			Console.WriteLine("\t2 - Medium (AI plays using heuristics)");
			Console.WriteLine("\t3 - Hard (AI plays using a pre-trained machine learning model and heuristics)");
			Console.WriteLine($"\t0 - Play GOPS with {currentPlayerCount} players");
			Console.Write(">> ");

			var userInput = Console.ReadLine();

			if (userInput == "1")
			{
				isUserInputValid = true;
				players.Add(new RandomPlayer());
			}
			else if (userInput == "2")
			{
				isUserInputValid = true;
				players.Add(new SimpleAiPlayer());
			}
			else if (userInput == "3")
			{
				isUserInputValid = true;
				players.Add(new MlpAiPlayer(filename));
			}
			else if (userInput == "0")
			{
				isUserInputValid = true;
				isDoneEnteringPlayers = true;
			}

			if (!isUserInputValid)
				Console.WriteLine($"ERROR: {userInput} is not valid input. Please enter 1, 2, 3, or 0");
			else
				ordinalIndicator = "th";
		}
	}

	var gops = new GOPS(players);

	gops.Play(true);
}