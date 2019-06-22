using System;
using System.Collections.Generic;

namespace mcts
{
    public class ConsolePlayer : IPlayer
    {
        public string Name { get; set; }

        public ConsolePlayer() { }

        public IGame MakeMove(IGame currentState, IGame[] successors)
        {
            Console.WriteLine("Current State: ");
            Console.WriteLine(GetBoardRepresentation((dynamic)currentState));
            Console.WriteLine("Please select a successor:");
            for(int i = 0; i < successors.Length; i++)
            {
                Console.WriteLine(i + ":");
                Console.WriteLine(GetBoardRepresentation((dynamic)successors[i]));
            }
            int selection = -1;
            while(selection < 0 || selection >= successors.Length)
            {
                Console.Write("Please enter your selection: ");
                var input = Console.ReadLine();
                try
                {
                    selection = int.Parse(input);
                }
                catch {} // try again
            }
            return successors[selection];
        }

        public static string GetBoardRepresentation(TicTacToe state)
        {
            Dictionary<PlayerId, string> rep = new Dictionary<PlayerId, string>()
            {
                { PlayerId.None, " " },
                { PlayerId.Player1, "X" },
                { PlayerId.Player2, "O" },
            };
            var board = state.Board;
            string result = "";
            for(int i = 0; i < 3; i++)
            {
                result += rep[board[i * 3]];
                for(int j = 1; j < 3; j++)
                {
                    result += "|" + rep[board[i * 3 + j]];
                }
                result += "\n";
                if (i < 2) result += "-+-+-\n";
            }
            return result;
        }
    }
}