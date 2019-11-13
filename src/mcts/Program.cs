using System;
using System.Collections.Generic;

namespace mcts
{
    class Program
    {
        static void Main(string[] args)
        {
            //DebuggingTests();
            //IsTerminalTests();
            //new ReversiTests().RunTests();
            PlayGame();
        }

        static void DebuggingTests()
        {
            {
                var moves = new[] { 0, 4, 1, 2 };
                var state = new TicTacToe();
                foreach(var move in moves)
                {
                    state = state.ApplyMove(move);
                }
                for(int i = 0; i < 10; i++)
                {
                    MonteCarloTreeSearch.Search(state, 1000);
                    Console.WriteLine("------");
                }
            }
        }

        static void IsTerminalTests()
        {
            {
                PlayerId expected = PlayerId.Player1;
                var moves = new[] { 0, 3, 1, 4, 2 };
                var state = new TicTacToe();
                foreach(var move in moves)
                {
                    state = state.ApplyMove(move);
                }
                PlayerId winner;
                if (!state.IsTerminal(out winner)) throw new Exception("should have been a been " + expected);
                if (winner != expected) throw new Exception("Expected winner of tie game should be " + expected + ", actual was " + winner);
            }

            {
                PlayerId expected = PlayerId.Player2;
                var moves = new[] { 0, 3, 1, 4, 6, 5 };
                var state = new TicTacToe();
                foreach(var move in moves)
                {
                    state = state.ApplyMove(move);
                }
                PlayerId winner;
                if (!state.IsTerminal(out winner)) throw new Exception("should have been a been " + expected);
                if (winner != expected) throw new Exception("Expected winner of tie game should be " + expected + ", actual was " + winner);
            }

            {
                PlayerId expected = PlayerId.None;
                                  //X  O  X  O  X  O  X  O  X
                var moves = new[] { 0, 1, 2, 4, 3, 6, 5, 8, 7 };
                var state = new TicTacToe();
                foreach(var move in moves)
                {
                    state = state.ApplyMove(move);
                }
                PlayerId winner;
                if (!state.IsTerminal(out winner)) throw new Exception("should have been a been " + expected);
                if (winner != expected) throw new Exception("Expected winner of tie game should be " + expected + ", actual was " + winner);
            }
        }

        static void PlayGame()
        {
            IGame state = new Reversi();
            var player1 = new ConsolePlayer() { Name = "Player 1" };
            //var player1 = new MonteCarloTreeSearchPlayer(1000) { Name = "Player 1" };
            var player2 = new MonteCarloTreeSearchPlayer(1000) { Name = "Player 2" };
            var playerLookup = new Dictionary<PlayerId, IPlayer>() 
            {
                { PlayerId.Player1, player1 },
                { PlayerId.Player2, player2 },
            };
            PlayerId winner;
            while(!state.IsTerminal(out winner))
            {
                ConsolePlayer.GetBoardRepresentation((dynamic)state, Console.Out);
                var currentPlayer = playerLookup[state.CurrentPlayersTurn];
                state = currentPlayer.MakeMove(state, state.ExpandSuccessors());
                Console.WriteLine(currentPlayer.Name + " selected " + state.DescribeLastMove() + ".");
                Console.WriteLine();
            }
            Console.WriteLine("THE GAME IS OVER!");
            ConsolePlayer.GetBoardRepresentation((dynamic)state, Console.Out);
            if (winner == PlayerId.None)
            {
                Console.WriteLine("THE GAME WAS A TIE!");
            }
            else
            {
                Console.WriteLine("THE WINNER IS " + playerLookup[winner].Name + "!");
            }
        }
    }
}
