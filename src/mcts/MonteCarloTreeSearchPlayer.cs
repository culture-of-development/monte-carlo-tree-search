namespace mcts  
{
    public class MonteCarloTreeSearchPlayer : IPlayer
    {
        private int numRollouts;

        public string Name { get; set; }

        public MonteCarloTreeSearchPlayer(int numRollouts)
        {
            this.numRollouts = numRollouts;
        }

        public IGame MakeMove(IGame currentState, IGame[] successors)
        {
            // TODO: ideally we verify that the result is in the successors
            return MonteCarloTreeSearch.Search(currentState, numRollouts);
        }
    }
}