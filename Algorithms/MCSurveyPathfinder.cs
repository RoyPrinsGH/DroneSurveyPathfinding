namespace DroneSurveyPathfinding.Algorithms;

using Position = (int x, int y);
using Path = List<(int x, int y)>;
using ScoredPath = (int score, List<(int x, int y)> path);
using System.Diagnostics;

// Monte carlo ~ish
public class MCSurveyPathfinder : ISurveyPathfinderAlgorithm
{
    private const int exhaustiveDepth = 3;
    private const int searchForwardFactor = 1;

    public ScoredPath CalculatePath(GridWorldModel worldModel, Position droneStartingPosition, int steps, int maxRunTimeMilliseconds)
    {
        Stopwatch timer = Stopwatch.StartNew();

        Random rng = new();
        
        Position head = droneStartingPosition;
        ScoredPath path = (0, [head]);

        int stepsTaken = 0;

        while (timer.ElapsedMilliseconds < maxRunTimeMilliseconds && stepsTaken < steps)
        {
            List<ScoredPath> pathsToSimulate = [path];

            for (int i = 0; i < exhaustiveDepth; i++)
            {
                List<ScoredPath> expandedPaths = [];

                foreach (ScoredPath pathToExpand in pathsToSimulate)
                {
                    foreach (Position position in worldModel.GetNeighboringPositions(pathToExpand.path[^1]))
                    {
                        int cellValue = worldModel.ValueOfCellAfterTakingPath(pathToExpand.path, position);
                        expandedPaths.Add((pathToExpand.score + cellValue, [.. pathToExpand.path, position]));

                        if (timer.ElapsedMilliseconds > maxRunTimeMilliseconds)
                        {
                            return path;
                        }
                    }
                }

                pathsToSimulate = expandedPaths;
            }

            Dictionary<ScoredPath, int> simulationScores = [];

            foreach (ScoredPath pathToSimulate in pathsToSimulate)
            {
                ScoredPath simulatedPath = (pathToSimulate.score, [.. pathToSimulate.path]);

                for (int i = 0; i < (steps - stepsTaken + exhaustiveDepth) / searchForwardFactor; i++)
                {
                    int max = -1;
                    List<Position> targetCandidates = [];

                    foreach (Position position in worldModel.GetNeighboringPositions(simulatedPath.path[^1]))
                    {
                        int neighborValue = worldModel.ValueOfCellAfterTakingPath(simulatedPath.path, position);
                        if (neighborValue == max)
                        {
                            targetCandidates.Add(position);
                        }
                        if (neighborValue > max)
                        {
                            max = neighborValue;
                            targetCandidates = [position];
                        }
                    }

                    if (targetCandidates.Count != 0)
                    {
                        Position next = targetCandidates[rng.Next(targetCandidates.Count)];
                        simulatedPath.score += worldModel.ValueOfCellAfterTakingPath(simulatedPath.path, next);
                        simulatedPath.path.Add(next);
                    }
                    else
                    {
                        throw new Exception("No neighbours found, how did this happen?");
                    }

                    if (timer.ElapsedMilliseconds > maxRunTimeMilliseconds)
                    {
                        Console.WriteLine("Time exceeded.");
                        return path;
                    }
                }

                simulationScores[pathToSimulate] = simulatedPath.score;
            }

            path = simulationScores.OrderByDescending(pair => pair.Value).First().Key;
            stepsTaken += exhaustiveDepth;
        }

        return path;
    }
}