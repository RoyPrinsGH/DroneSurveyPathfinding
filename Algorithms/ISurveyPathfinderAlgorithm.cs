namespace DroneSurveyPathfinding.Algorithms;

using Position = (int x, int y);

public interface ISurveyPathfinderAlgorithm
{
    public (List<Position> path, int totalScore) CalculatePath(GridWorldModel worldModel, Position droneStartingPosition, int steps, int maxRunTimeMilliseconds);
}