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
            Console.WriteLine("Please select a successor:");
            for(int i = 0; i < successors.Length; i++)
            {
                Console.WriteLine($"{i}: ({successors[i].DescribeLastMove()})");
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
            var board = state.GetBoard();
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

        public static string GetBoardRepresentation(Reversi state)
        {
            Dictionary<PlayerId, string> rep = new Dictionary<PlayerId, string>()
            {
                { PlayerId.None, " " },
                { PlayerId.Player1, " X" },
                { PlayerId.Player2, " O" },
            };
            var board = state.GetBoard();
            string result = "";
            for(int i = 0; i < 8; i++)
            {
                var player = board[i * 8];
                if (player == PlayerId.None)
                    result += (i * 8).ToString().PadLeft(2);
                else
                    result += rep[board[i * 8]];
                for(int j = 1; j < 8; j++)
                {
                    player = board[i * 8 + j];
                    if (player == PlayerId.None)
                        result += "|" + (i * 8 + j).ToString().PadLeft(2);
                    else
                        result += "|" + rep[board[i * 8 + j]];
                }
                result += "\n";
                if (i < 7) result += "--+--+--+--+--+--+--+--\n";
            }
            return result;
        }
    }
}