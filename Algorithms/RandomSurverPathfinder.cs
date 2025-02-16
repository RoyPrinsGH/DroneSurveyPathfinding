namespace DroneSurveyPathfinding.Algorithms;

using Position = (int x, int y);
using Path = List<(int x, int y)>;
using ScoredPath = (int score, List<(int x, int y)> path);
using System.Diagnostics;

public class RandomSurveyPathfinder : ISurveyPathfinderAlgorithm
{
    public ScoredPath CalculatePath(GridWorldModel worldModel, Position droneStartingPosition, int steps, int maxRunTimeMilliseconds)
    {
        Stopwatch timer = Stopwatch.StartNew();
        
        Random rng = new();

        Position currentDronePosition = droneStartingPosition;
        Path path = [currentDronePosition];
        
        int totalScore = 0;
        
        for (int i = 0; i < steps; i++)
        {
            if (timer.ElapsedMilliseconds > maxRunTimeMilliseconds)
            {
                break;
            }
            
            var neighbors = worldModel.GetNeighboringPositions(currentDronePosition).ToList();
            currentDronePosition = neighbors[rng.Next(neighbors.Count)];
            totalScore += worldModel.ValueOfCellAfterTakingPath(path, currentDronePosition);
            path.Add(currentDronePosition);
        }

        return (totalScore, path);
    }
}