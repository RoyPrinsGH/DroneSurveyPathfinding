namespace DroneSurveyPathfinding;

using Position = (int x, int y);

public static class ArgParser
{
    private const string maxTicksOption = "--maxticks";
    private const string maxRunTimeOptions = "--maxruntime";
    private const string droneStartingPositionOption = "--startat";
    private const string animateOption = "--animate";

    public const int defaultMaxTicks = 1000;
    public const int defaultMaxRunTime = 60_000;

    public static (string filePath, Position droneStartingPosition, int maxTicks, int maxRunTimeMilliseconds, bool animate)? Parse(string[] args)
    {
        List<string> argList = [.. args];

        try 
        {
            int maxTicks = defaultMaxTicks;
            if (argList.Contains(maxTicksOption))
            {
                int optIndex = argList.IndexOf(maxTicksOption);
                maxTicks = int.Parse(argList[optIndex + 1]);

                argList.RemoveRange(optIndex, 2);
            }

            int maxRunTimeMilliseconds = defaultMaxRunTime;
            if (argList.Contains(maxRunTimeOptions))
            {
                int optIndex = argList.IndexOf(maxRunTimeOptions);
                maxRunTimeMilliseconds = int.Parse(argList[optIndex + 1]);

                argList.RemoveRange(optIndex, 2);
            }

            Position droneStartingPosition = (0, 0);
            if (argList.Contains(droneStartingPositionOption))
            {
                int optIndex = argList.IndexOf(droneStartingPositionOption);
                droneStartingPosition = (int.Parse(argList[optIndex + 1]), int.Parse(argList[optIndex + 2]));

                argList.RemoveRange(optIndex, 3);
            }

            bool animate = argList.Contains(animateOption);
            string filePath = argList[0];

            return (filePath, droneStartingPosition, maxTicks, maxRunTimeMilliseconds, animate);
        }
        catch
        {
            Console.WriteLine($"Usage: surveypathfinder.exe <gridFilePath> [{maxTicksOption} maxTicks] [{maxRunTimeOptions} maxRunTime] [{droneStartingPositionOption} x y] [{animateOption}]");
            return null;
        }
    }
}