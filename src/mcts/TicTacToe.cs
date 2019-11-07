using System.Linq;

namespace mcts
{
    public class TicTacToe : IGame
    {
        public PlayerId CurrentPlayersTurn { get; private set; }
        public PlayerId LastPlayersTurn { get; private set; }
        public PlayerId Winner { get; private set; }

        private int pieces;

        public TicTacToe()
        {
            pieces = 0;
            CurrentPlayersTurn = PlayerId.Player1;
            LastPlayersTurn = PlayerId.None;
            Winner = PlayerId.None;
        }

        private static bool HasFlags(int value, int flag)
        {
            return (value & flag) == flag;
        }

        public PlayerId[] GetBoard() 
        {
            var result = new PlayerId[9];
            for(int i = 0; i < 9; i++)
            {
                result[i] = HasFlags(pieces & 0x1FF, 1 << i) ? PlayerId.Player1
                    : HasFlags(pieces >> 16, 1 << i) ? PlayerId.Player2
                    : PlayerId.None;
            }
            return result;
        }

        public TicTacToe ApplyMove(int cellNumber)
        {
            var nextState = new TicTacToe
            {
                pieces = pieces,
                LastPlayersTurn = CurrentPlayersTurn,
            };
            if (CurrentPlayersTurn == PlayerId.Player1)
            {
                nextState.pieces |= 1 << cellNumber;
                nextState.Winner = DetermineWinner(nextState.pieces & 0x1FF) ? PlayerId.Player1 : PlayerId.None;
            }
            else
            {
                nextState.pieces |= 1 << (cellNumber + 16);
                nextState.Winner = DetermineWinner(nextState.pieces >> 16) ? PlayerId.Player2 : PlayerId.None;
            }
            int allTaken = (nextState.pieces & 0x1FF) | (nextState.pieces >> 16);
            const int allSlots = 0x1FF;
            if (nextState.Winner == PlayerId.None && !HasFlags(allTaken, allSlots))
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
        private static readonly int[] winningBatches = new[]
        {
            1 << 0 | 1 << 1 | 1 << 2,// new[] { 0, 1, 2 },
            1 << 3 | 1 << 4 | 1 << 5,// new[] { 3, 4, 5 },
            1 << 4 | 1 << 7 | 1 << 8,// new[] { 6, 7, 8 },
            1 << 0 | 1 << 3 | 1 << 6,// new[] { 0, 3, 6 },
            1 << 1 | 1 << 4 | 1 << 7,// new[] { 1, 4, 7 },
            1 << 2 | 1 << 5 | 1 << 8,// new[] { 2, 5, 8 },
            1 << 0 | 1 << 4 | 1 << 8,// new[] { 0, 4, 8 },
            1 << 2 | 1 << 4 | 1 << 6,// new[] { 2, 4, 6 },
        };
        private static bool DetermineWinner(int pieces)
        {
            foreach(var cellGroup in winningBatches)
            {
                if (HasFlags(pieces, cellGroup))
                {
                    return true;
                }
            }
            return false;
        }

        public IGame[] ExpandSuccessors()
        {
            int allTaken = (pieces & 0x1FF) | (pieces >> 16);
            var successors = Enumerable.Range(0, 9)
                .Where(m => ((allTaken >> m) & 1) == 0) // todo: make this easier to understand
                .Select(ApplyMove)
                .ToArray();
            return successors;
        }

        public bool IsTerminal(out PlayerId winningPlayerNumber)
        {
            winningPlayerNumber = Winner;
            return CurrentPlayersTurn == PlayerId.None;
        }

        // it's not safe to cache the hash code, but we're going to anyway or this is going to be really slow
        public override int GetHashCode()
        {
            return pieces.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, null)) return false;
            if (object.ReferenceEquals(obj, this)) return true;
            var actual = (TicTacToe)obj;
            return this.pieces == actual.pieces;
        }

        public static bool operator ==(TicTacToe a, TicTacToe b)
        {
            if (object.ReferenceEquals(a, null)) return false;
            return a.Equals(b);
        }

        public static bool operator !=(TicTacToe a, TicTacToe b)
        {
            if (object.ReferenceEquals(a, null)) return false;
            return !a.Equals(b);
        }
    }
}