using System.Linq;

namespace mcts
{
    public abstract class Tests
    {
        public void RunTests()
        {
            var tests = this.GetType().GetMethods().Where(m => m.Name.StartsWith("Test"));
            foreach(var test in tests)
            {
                test.Invoke(this, null);
            }
        }
    }
}