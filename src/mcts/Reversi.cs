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
        private ulong player1Pieces;
        private ulong player2Pieces;
        private short skips;

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

        private ulong GetValidMoves()
        {
            ulong openCells = ~(player1Pieces | player2Pieces);
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

        private bool IsValidMove(ulong location)
        {
            // TODO: consider that it is valid to skip when no move is possible
            // can you quickly check when no move is possible?
            // dont have to worry about this if we never pass zero into here
            ulong currentPlayerPieces = CurrentPlayersTurn == PlayerId.Player1 ? player1Pieces : player2Pieces;
            ulong opponentPlayerPieces = CurrentPlayersTurn == PlayerId.Player1 ? player2Pieces : player1Pieces;
            ulong captures = Captures(currentPlayerPieces, opponentPlayerPieces, location);
            bool isValid = captures > 0;
            return isValid;
        }

        private ulong Captures(ulong currentPieces, ulong opponentPieces, ulong location)
        {
            // from our current location, we can move in any direction
            // as long as we continually encounter opponent pieces
            // we capture them all as long as the first non opponent piece contains a current player piece
            ulong captures = 0ul;

            // look up
            ulong current = location;
            ulong visited = 0;
            do
            {
                current <<= 8;
                visited |= current;
            } while (HasFlags(opponentPieces, current));
            if (HasFlags(currentPieces, current))
            {
                captures |= visited;
            }

            // look down
            current = location;
            visited = 0;
            do
            {
                current >>= 8;
                visited |= current;
            } while (HasFlags(opponentPieces, current));
            if (HasFlags(currentPieces, current))
            {
                captures |= visited;
            }

            // 0  1  2  3
            // 4  5  6  7
            // 8  9 10 11
            //12 13 14 15

            // look left
            ulong row_mask = (location >> (BitOperations.LeadingZeroCount(location) % 8)) * 255;
            current = location;
            visited = 0;
            do
            {
                current >>= 1;
                current &= row_mask;
                visited |= current;
            } while (HasFlags(opponentPieces, current));
            if (HasFlags(currentPieces, current))
            {
                captures |= visited;
            }

            // look left
            current = location;
            visited = 0;
            do
            {
                current <<= 1;
                current &= row_mask;
                visited |= current;
            } while (HasFlags(opponentPieces, current));
            if (HasFlags(currentPieces, current))
            {
                captures |= visited;
            }

            int loc_index = BitOperations.LeadingZeroCount(location);
            int row, col;
            
            // look up right
            row = loc_index / 8;
            col = loc_index % 8;
            current = location;
            visited = 0;
            do
            {
                row--;
                if (row < 0) break;
                col++;
                if (col >= 8) break;
                visited |= 1ul << (row * 8 + col);
            } while (HasFlags(opponentPieces, current));
            if (HasFlags(currentPieces, current))
            {
                captures |= visited;
            }

            // look up left
            row = loc_index / 8;
            col = loc_index % 8;
            current = location;
            visited = 0;
            do
            {
                row--;
                if (row < 0) break;
                col--;
                if (col < 0) break;
                visited |= 1ul << (row * 8 + col);
            } while (HasFlags(opponentPieces, current));
            if (HasFlags(currentPieces, current))
            {
                captures |= visited;
            }

            // look down left
            row = loc_index / 8;
            col = loc_index % 8;
            current = location;
            visited = 0;
            do
            {
                row++;
                if (row >= 8) break;
                col--;
                if (col < 0) break;
                visited |= 1ul << (row * 8 + col);
            } while (HasFlags(opponentPieces, current));
            if (HasFlags(currentPieces, current))
            {
                captures |= visited;
            }

            // look down right
            row = loc_index / 8;
            col = loc_index % 8;
            current = location;
            visited = 0;
            do
            {
                row++;
                if (row >= 8) break;
                col++;
                if (col >= 8) break;
                visited |= 1ul << (row * 8 + col);
            } while (HasFlags(opponentPieces, current));
            if (HasFlags(currentPieces, current))
            {
                captures |= visited;
            }

            return captures;
        }

        public Reversi ApplyMove(ulong location)
        {
            var nextState = new Reversi
            {
                player1Pieces = player1Pieces,
                player2Pieces = player2Pieces,
                skips = skips,
                LastPlayersTurn = CurrentPlayersTurn,
                CurrentPlayersTurn = CurrentPlayersTurn == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1,
            };
            if (location == skipMoveLocation)
            {
                nextState.skips += 1;
            }
            if (CurrentPlayersTurn == PlayerId.Player1)
            {
                ulong captures = Captures(player1Pieces, player2Pieces, location);
                nextState.player1Pieces |= captures | location;
                nextState.player2Pieces ^= captures;
                nextState.skips = 0;
            }
            else
            {
                ulong captures = Captures(player2Pieces, player1Pieces, location);
                nextState.player2Pieces |= captures | location;
                nextState.player1Pieces ^= captures;
                nextState.skips = 0;
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
            var hashCode = player1Pieces.GetHashCode() * 4133;
            hashCode ^= player2Pieces.GetHashCode() * 9697;
            hashCode ^= skips.GetHashCode() * 21317;
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