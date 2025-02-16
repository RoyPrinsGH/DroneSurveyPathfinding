namespace DroneSurveyPathfinding.Algorithms;

using Position = (int x, int y);
using Path = List<(int x, int y)>;
using ScoredPath = (int score, List<(int x, int y)> path);
using System.Diagnostics;

// Monte carlo ~ish
public class MCSurveyPathfinder : ISurveyPathfinderAlgorithm
{
    private const int exhaustiveDepth = 4;

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
                    foreach ((Position position, CellData _) neighbor in worldModel.GetNeighbors(pathToExpand.path[^1]))
                    {
                        int cellValue = worldModel.ValueOfCellAfterTakingPath(pathToExpand.path, neighbor.position);
                        expandedPaths.Add((pathToExpand.score + cellValue, [.. pathToExpand.path, neighbor.position]));

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

                for (int i = stepsTaken + exhaustiveDepth; i < steps; i++)
                {
                    int max = -1;
                    List<Position> targetCandidates = [];

                    foreach ((Position position, CellData _) neighborData in worldModel.GetNeighbors(simulatedPath.path[^1]))
                    {
                        int neighborValue = worldModel.ValueOfCellAfterTakingPath(simulatedPath.path, neighborData.position);
                        if (neighborValue == max)
                        {
                            targetCandidates.Add(neighborData.position);
                        }
                        if (neighborValue > max)
                        {
                            max = neighborValue;
                            targetCandidates = [neighborData.position];
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