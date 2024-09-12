using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGOPS
{
    public class HumanPlayer : Player
    {
        public override Card PlaceBet(GOPS gops)
        {
            //Display options to the user
            Console.WriteLine("Your Current Hand:");
            for(int i = 0; i < Hand.Count; i++)
            {
                Console.WriteLine($"{i} - {Hand.Cards[i]}");
            }
            Console.WriteLine();

            //Get User Selection
            Console.Write("Enter your selection >> ");
            string? input = Console.ReadLine();
            bool isValidInput = int.TryParse(input, out int selectionIdx);
            while (!isValidInput)
            {
                Console.Write("Re-enter your selection >> ");
                input = Console.ReadLine();
                isValidInput = int.TryParse(input, out int idx);
                selectionIdx = idx;
            }
            return Hand.SelectCard(selectionIdx);
        }
    }
}
