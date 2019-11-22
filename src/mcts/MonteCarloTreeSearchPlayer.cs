using System;

namespace mcts  
{
    public class MonteCarloTreeSearchPlayer : IPlayer
    {
        private int millisecondsToMove;

        public string Name { get; set; }

        public MonteCarloTreeSearchPlayer(int millisecondsToMove)
        {
            this.millisecondsToMove = millisecondsToMove;
        }

        public IGame MakeMove(IGame currentState, IGame[] successors)
        {
            // TODO: ideally we verify that the result is in the successors
            var (selectedAction, actionStats, totalSimulationCount) = MonteCarloTreeSearch.Search(currentState, millisecondsToMove);
            Console.WriteLine("totalSimulationCount: " + totalSimulationCount);
            foreach(var (successor, stats) in actionStats)
            {
                Console.WriteLine($"{successor}: {stats.Wins}, {stats.SimiulationsCount}\n     {stats.UpperConfidenceBoundScore(totalSimulationCount)}, {stats.PureMonteCarloScore}");
            }
            return selectedAction;
        }
    }
}