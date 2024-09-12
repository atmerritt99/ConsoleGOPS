// See https://aka.ms/new-console-template for more information
using ConsoleGOPS;

List<Player> players = new List<Player>()
{
	new HumanPlayer()
};

Console.WriteLine("Austin's GOPS Program:");

bool isUserInputValid = false;
while(!isUserInputValid)
{
	Console.WriteLine("Enter 2nd player AI difficulty:");
	Console.WriteLine("\t1 - Easy (AI plays randomly)");
	Console.WriteLine("\t2 - Medium (AI plays using heuristics)");
	Console.WriteLine("\t3 - Hard (AI plays using a pre-trained model)");
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
		players.Add(MlpAiPlayer.Create("GopsMlpAiV2.txt"));
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
		Console.WriteLine("\t2 - Medium (AI plays using a simple heuristic)");
		Console.WriteLine("\t3 - Hard (AI plays using a pre-trained machine learning model)");
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
			players.Add(MlpAiPlayer.Create("GopsMlpAiV2.txt"));
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