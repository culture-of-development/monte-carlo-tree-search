using System;
using System.Collections.Generic;

namespace mcts
{
    public class OthelloTests : Tests
    {
        public void TestCaptures()
        {
            //  0  1  2  3  4  5  6  7
            //  8  9 10 11 12 13 14 15
            // 16 17 18 19 20 21 22 23
            // 24 25 26  X  O 29 30 31
            // 32 33 34  O  X 37 38 39
            // 40 41 42 43 44 45 46 47
            // 48 49 50 51 52 53 54 55
            // 56 57 58 59 60 61 62 63

            const ulong noCaptures = 0ul;

            var state = new Othello();
            var captures = state.Captures(state.player1Pieces, state.player2Pieces, 1ul << 19);
            if (captures != noCaptures) throw new Exception("expected no captures, got " + string.Join(", ", GetLocations(captures)));

        }

        public void TestEndCaptures()
        {
            //  0  1  2  3  4  5  6  7
            //  8  9 10 11 12 13 14 15
            // 16 17 18 19 20 21 22 23
            // 24 25 26  X  O 29 30 31
            // 32 33 34  O  X 37 38 39
            // 40 41 42 43 44 45 46 47
            // 48 49 50 51 52 53 54 55
            // 56 57 58 59 60 61 62 63

            var state = new Othello();
            state.player1Pieces = ulong.MaxValue & ~(1ul << 57);
            state.player2Pieces = 1ul << 57;
            var successors = state.ExpandSuccessors();
            if (successors.Length != 1) throw new Exception("should only be 1 successor");
            var next = (Othello)successors[0];
            if (next.player1Pieces != state.player1Pieces || next.player2Pieces != state.player2Pieces)
                throw new Exception("pieces are not the same");
            if (next.skips != (state.skips + 1))
                throw new Exception("skips should be 1");
            if (next.IsTerminal(out PlayerId _))
                throw new Exception("should have to skip more before terminal");

            state = next;
            successors = state.ExpandSuccessors();
            if (successors.Length != 1) throw new Exception("should only be 1 successor");
            next = (Othello)successors[0];
            if (next.player1Pieces != state.player1Pieces || next.player2Pieces != state.player2Pieces)
                throw new Exception("pieces are not the same 2");
            if (next.skips != (state.skips + 1))
                throw new Exception("skips should be 2");
            if (!next.IsTerminal(out PlayerId _))
                throw new Exception("should be terminal");
        }

        private int[] GetLocations(ulong locations)
        {
            var results = new List<int>();
            for(int i = 0; locations > 0; i++, locations >>= 1)
            {
                if ((locations & 1ul) == 1ul) results.Add(i);
            }
            return results.ToArray();
        }
    }
}