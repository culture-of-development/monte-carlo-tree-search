using System;
using System.Collections.Generic;
using mcts;

namespace mcts_cli
{
    class Program
    {
        static void Main(string[] args)
        {
            //new TicTacToeTests().RunTests();
            //new OthelloTests().RunTests();
            PlayGame();
        }

        static void PlayGame()
        {
            IGame state = new Othello();
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
