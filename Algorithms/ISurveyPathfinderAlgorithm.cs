namespace DroneSurveyPathfinding.Algorithms;

using Position = (int x, int y);
using ScoredPath = (int score, List<(int x, int y)> path);

public interface ISurveyPathfinderAlgorithm
{
    public ScoredPath CalculatePath(GridWorldModel worldModel, Position droneStartingPosition, int steps, int maxRunTimeMilliseconds);
}