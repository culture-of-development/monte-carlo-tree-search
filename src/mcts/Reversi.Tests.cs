using System;
using System.Collections.Generic;
using System.Linq;

namespace mcts
{
    public class ReversiTests
    {
        public void RunTests()
        {
            var tests = this.GetType().GetMethods().Where(m => m.Name.StartsWith("Test"));
            foreach(var test in tests)
            {
                test.Invoke(this, null);
            }
        }
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

            var state = new Reversi();
            var captures = state.Captures(state.player1Pieces, state.player2Pieces, 1ul << 19);
            if (captures != noCaptures) throw new Exception("expected no captures, got " + string.Join(", ", GetLocations(captures)));

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