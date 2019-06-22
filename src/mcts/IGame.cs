
namespace mcts
{
    public interface IGame
    {
        IGame[] ExpandSuccessors();
        bool IsTerminal(out PlayerId winningPlayerNumber);
        PlayerId CurrentPlayersTurn { get; }
    }
}