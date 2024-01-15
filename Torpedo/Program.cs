namespace Torpedo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //LevLogger logger = new LevLogger(LogLevel.LogDebug, "none");
            //logger.AddLog(new LevLog(LogLevel.LogDebug, "Debug message"));
            //logger.AddLog(new LevLog(LogLevel.LogInfo, "Info that smth happened"));
            //logger.AddLog(new LevLog(LogLevel.LogError, "Spmething went wrong!"));
            //Console.ReadLine();

            LevLogger logger = new LevLogger(LogLevel.LogDebug, @"C:\Users\bajno.DESKTOP-RHO2542\Desktop\torpedolog.txt");
            Game game = new Game(logger);

            Communicator communicator = new Communicator();
            communicator.StartServerLoop(logger, game);
        }
    }
}
