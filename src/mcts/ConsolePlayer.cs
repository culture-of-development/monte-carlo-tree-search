using System;
using System.Collections.Generic;
using System.Linq;

namespace mcts
{
    public class ConsolePlayer : IPlayer
    {
        public string Name { get; set; }

        public ConsolePlayer() { }

        public IGame MakeMove(IGame currentState, IGame[] successors)
        {
            if (successors[0].DescribeLastMove() == "64") return successors[0];
            var options = successors.ToDictionary(m => m.DescribeLastMove());
            while(true)
            {
                Console.Write("Please enter your selection: ");
                var input = Console.ReadLine();
                if (options.ContainsKey(input)) return options[input];
                Console.Write("Negative... ");
            }
        }

        public static void GetBoardRepresentation(TicTacToe state, System.IO.TextWriter writer)
        {
            Dictionary<PlayerId, string> rep = new Dictionary<PlayerId, string>()
            {
                { PlayerId.None, " " },
                { PlayerId.Player1, "X" },
                { PlayerId.Player2, "O" },
            };
            var board = state.GetBoard();
            for(int i = 0; i < 3; i++)
            {
                writer.Write(rep[board[i * 3]]);
                for(int j = 1; j < 3; j++)
                {
                    writer.Write("|" + rep[board[i * 3 + j]]);
                }
                writer.WriteLine();
                if (i < 2) writer.WriteLine("-+-+-");
            }
        }

        public static void GetBoardRepresentation(Othello state, System.IO.TextWriter writer)
        {
            Dictionary<PlayerId, string> rep = new Dictionary<PlayerId, string>()
            {
                { PlayerId.None, " " },
                { PlayerId.Player1, " X" },
                { PlayerId.Player2, " O" },
            };
            Console.ForegroundColor = ConsoleColor.White;
            var board = state.GetBoard();
            for(int i = 0; i < 8; i++)
            {
                var player = board[i * 8];
                for(int j = 0; j < 8; j++)
                {
                    player = board[i * 8 + j];
                    if (j > 0) {
                        Console.ForegroundColor = ConsoleColor.White;
                        writer.Write("|");
                    }
                    if (player == PlayerId.None) {
                        if (state.IsValidMove(1ul << (i*8+j))) {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            writer.Write((i * 8 + j).ToString().PadLeft(2));
                        }
                        else {
                            writer.Write("  ");
                        }
                    } else {
                        var cellPlayer = board[i * 8 + j];
                        Console.ForegroundColor = cellPlayer == PlayerId.Player1 ? ConsoleColor.Red : ConsoleColor.Blue;
                        writer.Write(rep[cellPlayer]);
                    }
                }
                writer.WriteLine();
                if (i < 7) {
                    Console.ForegroundColor = ConsoleColor.White;
                    writer.WriteLine("--+--+--+--+--+--+--+--");
                }
            }
            Console.ResetColor();
        }
    }
}