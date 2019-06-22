using System.Linq;

namespace mcts
{
    public class TicTacToe : IGame
    {
        public PlayerId[] Board { get; private set; }
        public PlayerId CurrentPlayersTurn { get; private set; }
        public PlayerId Winner { get; private set; }

        public TicTacToe()
        {
            Board = new PlayerId[9];
            CurrentPlayersTurn = PlayerId.Player1;
            Winner = PlayerId.None;
        }

        public TicTacToe ApplyMove(int cellNumber)
        {
            var nextState = new TicTacToe
            {
                Board = this.Board.ToArray(),
            };
            nextState.Board[cellNumber] = CurrentPlayersTurn;
            nextState.Winner = DetermineWinner(nextState.Board);
            if (nextState.Winner == PlayerId.None && nextState.Board.Any(m => m == PlayerId.None))
            {
                nextState.CurrentPlayersTurn = CurrentPlayersTurn == PlayerId.Player2 
                    ? PlayerId.Player1 
                    : PlayerId.Player2;
            }
            else
            {
                nextState.CurrentPlayersTurn = PlayerId.None;
            }
            return nextState;
        }
        // 0, 1, 2
        // 3, 4, 5
        // 6, 7, 8
        private static readonly int[][] winningBatches = new[]
        {
            new[] { 0, 1, 2 },
            new[] { 3, 4, 5 },
            new[] { 6, 7, 8 },
            new[] { 0, 3, 6 },
            new[] { 1, 4, 7 },
            new[] { 2, 5, 8 },
            new[] { 0, 4, 8 },
            new[] { 2, 4, 6 },
        };
        private static PlayerId DetermineWinner(PlayerId[] board)
        {
            foreach(var cellGroup in winningBatches)
            {
                var possibleWinner = board[cellGroup[0]];
                if (possibleWinner != PlayerId.None
                    && possibleWinner == board[cellGroup[1]] 
                    && possibleWinner == board[cellGroup[2]])
                {
                    return possibleWinner;
                }
            }
            return PlayerId.None;
        }

        public IGame[] ExpandSuccessors()
        {
            var successors = Enumerable.Range(0, Board.Length)
                .Where(m => Board[m] == PlayerId.None)
                .Select(ApplyMove)
                .ToArray();
            return successors;
        }

        public bool IsTerminal(out PlayerId winningPlayerNumber)
        {
            winningPlayerNumber = Winner;
            return CurrentPlayersTurn == PlayerId.None;
        }
    }
}