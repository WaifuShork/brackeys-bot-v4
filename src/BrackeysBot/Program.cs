using System.Threading.Tasks;

namespace BrackeysBot
{
    internal static class Program
    {
        private static async Task<int> Main()
        {
            return await BrackeysBotHost.RunAsync();
        }
    }
}