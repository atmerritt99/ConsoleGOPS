// See https://aka.ms/new-console-template for more information
using ConsoleGOPS;

//var pops = new Population(300, new int[] { 195, 104, 13 });
//var mlpPlayer = pops.Run(200, 50, "GopsMlpAiV2.txt");

var mlpPlayer = MlpAiPlayer.Create("GopsMlpAiV2.txt");

List<Player> players = new List<Player>()
{
    mlpPlayer,
    new SimpleAiPlayer(),
};

const float MAX_GAMES = 10000;
float wins = 0;
for (int i = 0; i < MAX_GAMES; i++)
{
    if (i % 100 == 0)
        Console.WriteLine(i);
    GOPS gops = new GOPS(players);
    gops.Play();
    if (gops.IdxOfWinner == 0)
        wins++;
}

Console.WriteLine($"Win percentage {wins / MAX_GAMES}");