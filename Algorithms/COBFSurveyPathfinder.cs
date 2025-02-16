namespace DroneSurveyPathfinding.Algorithms;

using Position = (int x, int y);
using Path = List<(int x, int y)>;
using ScoredPath = (int score, List<(int x, int y)> path);

// Cut Off Breadth First
public class COBFSurveyPathfinder : ISurveyPathfinderAlgorithm
{
    private const int CutOff = 100;

    public ScoredPath CalculatePath(GridWorldModel worldModel, Position droneStartingPosition, int steps, int maxRunTimeMilliseconds)
    {
        List<ScoredPath> paths = [(0, [droneStartingPosition])];

        for (int i = 0; i < steps; i++)
        {
            List<ScoredPath> newPaths = [];

            foreach (ScoredPath scoredPath in paths)
            {
                foreach ((Position position, CellData data) neighbor in worldModel.GetNeighbors(scoredPath.path[^1]))
                {
                    Path appendedPath = [.. scoredPath.path, neighbor.position];
                    ScoredPath newPath = (scoredPath.score + worldModel.ValueOfCellAfterTakingPath(scoredPath.path, neighbor.position), appendedPath);
                    newPaths.Add(newPath);
                }
            }

            paths = newPaths.OrderByDescending(path => path.score)
                            .Take(CutOff)
                            .ToList();
        }

        return paths.OrderByDescending(path => path.score).First();
    }
}