
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace mcts
{
    public static class MonteCarloTreeSearch
    {
        private static Random random = new Random();

        // NOTE: assumes IGame is immutable
        // TODO: alternatively pass in an allotment of time instead of number of simulations
        public static IGame Search(IGame current, int millisecondsToMove)
        {
            var tree = new TreeSearch(current);
            var stopwatch = Stopwatch.StartNew();
            for(long i = 0; ; i++)
            {
                if (i % 10 == 0)
                {
                    if (stopwatch.ElapsedMilliseconds > millisecondsToMove) break;
                }
                tree.PerformMonteCarloRound(current);
            }
            var successors = current.ExpandSuccessors();
            double mostSimulations = -1d;
            double currentSimulations = tree.states[current].SimiulationsCount;
            Console.WriteLine("currentSimulations: " + currentSimulations);
            var bestScoreStates = new List<IGame>();
            foreach(var successor in successors)
            {
                var stats = tree.states[successor];
                Console.WriteLine($"{successor.DescribeLastMove()}: {stats.Wins}, {stats.SimiulationsCount}\n     {stats.UpperConfidenceBoundScore(currentSimulations)}, {stats.PureMonteCarloScore}");
                if (stats.SimiulationsCount > mostSimulations)
                {
                    bestScoreStates.Clear();
                    bestScoreStates.Add(successor);
                    mostSimulations = stats.SimiulationsCount;
                }
                else if (stats.SimiulationsCount == mostSimulations)
                {
                    bestScoreStates.Add(successor);
                }
            }
            var nextMove = bestScoreStates[random.Next(bestScoreStates.Count)];
            return nextMove;
        }

        private class TreeSearch
        {
            public Dictionary<IGame, StateStats> states;

            public TreeSearch(IGame current)
            {
                states = new Dictionary<IGame, StateStats>();
                states.Add(current, new StateStats());
            }

            public void PerformMonteCarloRound(IGame current)
            {
                var path = new List<IGame>() { current };
                // step 1: selection - find a node which does not have all children in the states database
                PerformSelectionPureMonteCarlo(path);
                // step 2: expansion - make sure that node has all children in the states database
                Expand(path);
                // step 3: simulation
                RunSimulation(path);
                // step 4: backprop
                BackPropagation(path);
            }

            private void PerformSelectionPureMonteCarlo(List<IGame> path)
            {
                var state = path.Last();
                while(!state.IsTerminal(out PlayerId dummy))
                {
                    var successors = state.ExpandSuccessors();
                    var parentSimulations = states[state].SimiulationsCount;
                    if (successors.Any(m => !states.ContainsKey(m))) break;
                    state = successors
                        //.OrderByDescending(m => states[m].PureMonteCarloScore)
                        .OrderByDescending(m => states[m].UpperConfidenceBoundScore(parentSimulations))
                        .First();
                    path.Add(state);
                }
            }

            private void Expand(List<IGame> path)
            {
                // TODO: is this how you actually implement expand?
                // this forcefully adds nodes to the tree that do not have simulations
                var state = path.Last();
                var successors = state.ExpandSuccessors();
                foreach(var successor in successors)
                {
                    if (!states.ContainsKey(successor))
                    {
                        states.Add(successor, new StateStats());
                    }
                }
            }

            private void RunSimulation(List<IGame> path)
            {
                var state = path.Last();
                // TODO: limit maximum depth, same in BackPropagation
                while(!state.IsTerminal(out PlayerId dummy))
                {
                    var successors = state.ExpandSuccessors();
                    state = successors[random.Next(successors.Length)];
                    path.Add(state);
                }
            }

            private void BackPropagation(List<IGame> path)
            {
                PlayerId winner;
                path.Last().IsTerminal(out winner);
                foreach(var state in path)
                {
                    if (!states.ContainsKey(state)) continue;
                    var stats = states[state];
                    stats.SimiulationsCount += 1; // TODO: if you can visit the same state twice, it's counting twice, so don't do that
                    if (winner == PlayerId.None)
                    {
                        stats.Wins += 0.5d;
                    }
                    else if (winner == state.LastPlayersTurn)
                    {
                        stats.Wins += 1d;
                    }
                }
            }
        }

        private class StateStats
        {
            public double SimiulationsCount { get; set; }
            public double Wins { get; set; }

            static readonly double c = Math.Sqrt(2);

            public double PureMonteCarloScore => SimiulationsCount == 0 ? 1d : Wins / SimiulationsCount;
            public double UpperConfidenceBoundScore(double parentSimulations) => 
                SimiulationsCount == 0 ? 1d :
                PureMonteCarloScore + c * Math.Sqrt(Math.Log(parentSimulations) / SimiulationsCount);
        }
    }
}