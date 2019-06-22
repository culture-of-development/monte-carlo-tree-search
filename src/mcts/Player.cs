namespace mcts
{
    public enum PlayerId : byte
    {
        None,
        Player1,
        Player2,
    }

    public interface IPlayer
    {
        string Name { get; set; }
        IGame MakeMove(IGame currentState, IGame[] successors);
    }
}