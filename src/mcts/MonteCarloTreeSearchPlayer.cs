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
            return MonteCarloTreeSearch.Search(currentState, millisecondsToMove);
        }
    }
}