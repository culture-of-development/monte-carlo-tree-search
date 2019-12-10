using System;
using System.Collections.Generic;

namespace mcts
{
    public class TicTacToeTests : Tests
    {
        static void TestIsTerminal()
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
    }
}