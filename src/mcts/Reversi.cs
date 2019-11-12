using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;


namespace mcts
{
    // reversi is played on an 8x8 board so there are 64 locations
    // we're going to store a player's pieces as an unsigned long (64 bits)
    // we track the number of skipped moves with a short

    public class Reversi : IGame
    {
        public ulong player1Pieces;
        public ulong player2Pieces;
        public short skips;
        public ulong lastMove = 0ul;
        public string DescribeLastMove() => BitOperations.TrailingZeroCount(lastMove).ToString();

        public PlayerId CurrentPlayersTurn { get; private set; }
        public PlayerId LastPlayersTurn { get; private set; }

        public Reversi()
        {
            player1Pieces = (1ul << 27) | (1ul << 36);
            player2Pieces = (1ul << 28) | (1ul << 35);
            skips = 0;
            CurrentPlayersTurn = PlayerId.Player1;
            LastPlayersTurn = PlayerId.None;
        }

        const ulong skipMoveLocation = 0;
        public IGame[] ExpandSuccessors()
        {
            var validMoves = GetValidMoves();
            if (validMoves == 0ul)
            {
                // you can only skip when you cannot place a new piece
                return new[] { ApplyMove(skipMoveLocation) };
            }
            var successors = Enumerable.Range(0, 64)
                .Select(m => 1ul << m)
                .Where(m => HasFlags(validMoves, m))
                .Select(ApplyMove)
                .ToArray();
            return successors;
        }

        public ulong GetValidMoves()
        {
            ulong validMoves = 0ul;
            ulong currentLocation = 1ul;
            while(currentLocation > 0)
            {
                if (IsValidMove(currentLocation))
                {
                    validMoves |= currentLocation;
                }
                currentLocation <<= 1;
            }
            return validMoves;
        }

        public bool IsValidMove(ulong location)
        {
            // TODO: consider that it is valid to skip when no move is possible
            // can you quickly check when no move is possible?
            // dont have to worry about this if we never pass zero into here
            ulong openCells = ~(player1Pieces | player2Pieces);
            if (!HasFlags(openCells, location)) return false;
            ulong currentPlayerPieces = CurrentPlayersTurn == PlayerId.Player1 ? player1Pieces : player2Pieces;
            ulong opponentPlayerPieces = CurrentPlayersTurn == PlayerId.Player1 ? player2Pieces : player1Pieces;
            ulong captures = Captures(currentPlayerPieces, opponentPlayerPieces, location);
            bool isValid = captures > 0;
            return isValid;
        }

        public ulong Captures(ulong currentPieces, ulong opponentPieces, ulong location)
        {
            // from our current location, we can move in any direction
            // as long as we continually encounter opponent pieces
            // we capture them all as long as the first non opponent piece contains a current player piece
            ulong captures = 0ul;
            captures |= Look(currentPieces, opponentPieces, location, -1,  0); // up
            captures |= Look(currentPieces, opponentPieces, location,  1,  0); // down
            captures |= Look(currentPieces, opponentPieces, location,  0, -1); // left
            captures |= Look(currentPieces, opponentPieces, location,  0,  1); // right
            captures |= Look(currentPieces, opponentPieces, location, -1, -1); // up left
            captures |= Look(currentPieces, opponentPieces, location, -1,  1); // up right
            captures |= Look(currentPieces, opponentPieces, location,  1, -1); // down left
            captures |= Look(currentPieces, opponentPieces, location,  1,  1); // down right
            return captures;
        }

        public ulong Look(ulong currentPieces, ulong opponentPieces, ulong location, int rowShift, int colShift)
        {
            int loc_index = BitOperations.TrailingZeroCount(location);
            int row = loc_index / 8 + rowShift;
            if (row < 0 || row >= 8) return 0ul;
            int col = loc_index % 8 + colShift;
            if (col < 0 || col >= 8) return 0ul;
            ulong current = 1ul << (row * 8 + col);
            ulong visited = 0;
            while (HasFlags(opponentPieces, current))
            {
                visited |= current;
                row += rowShift;
                col += colShift;
                if (row < 0 || row >= 8 || col < 0 || col >= 8) {
                    current = 0ul;
                    break;
                }
                current = 1ul << (row * 8 + col);
            }
            return HasFlags(currentPieces, current) ? visited : 0ul;
        }

        public Reversi ApplyMove(ulong location)
        {
            var nextState = new Reversi
            {
                player1Pieces = player1Pieces,
                player2Pieces = player2Pieces,
                skips = skips,
                lastMove = location,
                LastPlayersTurn = CurrentPlayersTurn,
                CurrentPlayersTurn = CurrentPlayersTurn == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1,
            };
            if (location == skipMoveLocation)
            {
                nextState.skips += 1;
            }
            else 
            {
                nextState.skips = 0;
            }
            if (CurrentPlayersTurn == PlayerId.Player1)
            {
                ulong captures = Captures(player1Pieces, player2Pieces, location);
                nextState.player1Pieces |= captures | location;
                nextState.player2Pieces &= ~captures;
            }
            else
            {
                ulong captures = Captures(player2Pieces, player1Pieces, location);
                nextState.player2Pieces |= captures | location;
                nextState.player1Pieces &= ~captures;
            }
            return nextState;
        }

        public bool IsTerminal(out PlayerId winningPlayerNumber)
        {
            winningPlayerNumber = PlayerId.None;
            if (skips >= 2)
            {
                int player1Score = BitOperations.PopCount(player1Pieces);
                int player2Score = BitOperations.PopCount(player2Pieces);
                if (player1Score > player2Score) winningPlayerNumber = PlayerId.Player1;
                if (player1Score < player2Score) winningPlayerNumber = PlayerId.Player2;
                return true;
            };
            return false;
        }


        /// <summary>
        /// Always returns false when flags is 0.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasFlags(ulong value, ulong flags)
        {
            return flags != 0ul && (value & flags) == flags;
        }

        public PlayerId[] GetBoard()
        {
            var pieces = Enumerable.Range(0, 64)
                .Select(m => 1ul << m)
                .Select(m => HasFlags(player1Pieces, m) ? PlayerId.Player1 : HasFlags(player2Pieces, m) ? PlayerId.Player2 : PlayerId.None)
                .ToArray();
            return pieces;
        }

        // it's not safe to cache the hash code, but we're going to anyway or this is going to be really slow
        public override int GetHashCode()
        {
            var hashCode = (player1Pieces.GetHashCode() + 23) * 4133;
            hashCode ^= (player2Pieces.GetHashCode() + 43) * 9697;
            hashCode ^= (skips.GetHashCode() + 71) * 21317;
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, null)) return false;
            if (object.ReferenceEquals(obj, this)) return true;
            var actual = (Reversi)obj;
            return this.player1Pieces == actual.player1Pieces 
                && this.player2Pieces == actual.player2Pieces 
                && this.skips == actual.skips;
        }

        public static bool operator ==(Reversi a, Reversi b)
        {
            if (object.ReferenceEquals(a, null)) return false;
            return a.Equals(b);
        }

        public static bool operator !=(Reversi a, Reversi b)
        {
            if (object.ReferenceEquals(a, null)) return false;
            return !a.Equals(b);
        }
    }
}