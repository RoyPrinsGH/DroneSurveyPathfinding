namespace DroneSurveyPathfinding.Algorithms;

using Position = (int x, int y);
using Path = List<(int x, int y)>;
using ScoredPath = (int score, List<(int x, int y)> path);
using System.Diagnostics;

public class GreedySurveyPathfinder : ISurveyPathfinderAlgorithm
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

            int max = -1;
            List<Position> targetCandidates = [];

            foreach (Position position in worldModel.GetNeighboringPositions(currentDronePosition))
            {
                int neighborValue = worldModel.ValueOfCellAfterTakingPath(path, position);
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

            if (targetCandidates.Any())
            {
                currentDronePosition = targetCandidates[rng.Next(targetCandidates.Count)];
                totalScore += worldModel.ValueOfCellAfterTakingPath(path, currentDronePosition);
                path.Add(currentDronePosition);
            }
            else
            {
                throw new Exception("No neighbours found, how did this happen?");
            }
        }

        return (totalScore, path);
    }
}