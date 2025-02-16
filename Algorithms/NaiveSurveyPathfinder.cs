namespace DroneSurveyPathfinding.Algorithms;

using Position = (int x, int y);
using Path = List<(int x, int y)>;
using ScoredPath = (int score, List<(int x, int y)> path);

public class NaiveSurveyPathfinder : ISurveyPathfinderAlgorithm
{
    public ScoredPath CalculatePath(GridWorldModel worldModel, Position droneStartingPosition, int steps, int maxRunTimeMilliseconds)
    {
        Random rng = new();

        Position currentDronePosition = droneStartingPosition;
        Path path = [currentDronePosition];
        
        int totalScore = 0;
        
        for (int i = 0; i < steps; i++)
        {
            int max = -1;
            Path targetCandidates = [];

            foreach ((Position position, CellData cellData) neighborData in worldModel.GetNeighbors(currentDronePosition))
            {
                if (neighborData.cellData.value == max)
                {
                    targetCandidates.Add(neighborData.position);
                }
                if (neighborData.cellData.value > max)
                {
                    max = neighborData.cellData.value;
                    targetCandidates = [neighborData.position];
                }
            }

            if (targetCandidates.Any())
            {
                currentDronePosition = targetCandidates[rng.Next(targetCandidates.Count)];
                path.Add(currentDronePosition);

                worldModel.Update();

                totalScore += worldModel.Grid[currentDronePosition.x, currentDronePosition.y].value;
                worldModel.Grid[currentDronePosition.x, currentDronePosition.y].value = 0;
            }
            else
            {
                throw new Exception("No neighbours found, how did this happen?");
            }
        }

        return (totalScore, path);
    }
}