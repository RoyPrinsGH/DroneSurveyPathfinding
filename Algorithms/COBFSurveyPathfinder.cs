namespace DroneSurveyPathfinding.Algorithms;

using Position = (int x, int y);
using Path = List<(int x, int y)>;
using ScoredPath = (int score, List<(int x, int y)> path);
using System.Diagnostics;

// Cut Off Breadth First
public class COBFSurveyPathfinder : ISurveyPathfinderAlgorithm
{
    private const int cutOff = 100;

    public ScoredPath CalculatePath(GridWorldModel worldModel, Position droneStartingPosition, int steps, int maxRunTimeMilliseconds)
    {
        Stopwatch timer = Stopwatch.StartNew();

        List<ScoredPath> paths = [(0, [droneStartingPosition])];

        for (int i = 0; i < steps; i++)
        {
            List<ScoredPath> newPaths = [];

            foreach (ScoredPath scoredPath in paths)
            {
                foreach (Position position in worldModel.GetNeighboringPositions(scoredPath.path[^1]))
                {
                    Path appendedPath = [.. scoredPath.path, position];
                    ScoredPath newPath = (scoredPath.score + worldModel.ValueOfCellAfterTakingPath(scoredPath.path, position), appendedPath);
                    newPaths.Add(newPath);

                    if (timer.ElapsedMilliseconds > maxRunTimeMilliseconds) break;
                }

                if (timer.ElapsedMilliseconds > maxRunTimeMilliseconds) break;
            }

            if (timer.ElapsedMilliseconds > maxRunTimeMilliseconds) break;

            paths = newPaths.OrderByDescending(path => path.score)
                            .Take(cutOff)
                            .ToList();
        }

        if (timer.ElapsedMilliseconds > maxRunTimeMilliseconds)
        {
            Console.WriteLine("Time exceeded.");
        }

        return paths.OrderByDescending(path => path.score).First();
    }
}