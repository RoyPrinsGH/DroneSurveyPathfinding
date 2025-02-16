namespace DroneSurveyPathfinding.Algorithms;

using Position = (int x, int y);
using Path = List<(int x, int y)>;
using ScoredPath = (int score, List<(int x, int y)> path);
using System.Diagnostics;

public class GeneticSurveyPathfinder : ISurveyPathfinderAlgorithm
{
    private const int populationSize = 50;

    private readonly Random rng = new();

    private ScoredPath GenerateRandomPath(GridWorldModel worldModel, Position start, int length)
    {
        Position head = start;
        int score = 0;
        Path path = [head];

        for (int i = 0; i < length; i++)
        {
            var neighbors = worldModel.GetNeighbors(head).ToList();
            (Position randomNeighbor, CellData data) = neighbors[rng.Next(neighbors.Count)];
            path.Add(randomNeighbor);
            score += data.value;
            head = randomNeighbor;
        }

        return (score, path);
    }

    private ScoredPath GenerateMutation(GridWorldModel worldModel, ScoredPath scoredPath)
    {
        Path path = scoredPath.path;
        Path newPath = [.. path];

        int index = rng.Next(path.Count - 1);

        if (index == path.Count - 2)
        {
            var neighbors = worldModel.GetNeighbors(newPath[^2]).ToList();
            newPath[^1] = neighbors[rng.Next(neighbors.Count)].position;
        }
        else
        {
            Position start = path[index];
            Position toMove = path[index + 1];
            Position positionToStayConnectedTo = path[index + 2];

            Func<Position, Position, int> DiagonalTaxiCab = (position1, position2) => Math.Max(Math.Abs(position1.x - position2.x), Math.Abs(position1.y - position2.y));

            List<(Position position, CellData data)> possibleMoveLocations = worldModel.GetNeighbors(toMove)
                                                                                       .Where(cell => DiagonalTaxiCab(cell.position, positionToStayConnectedTo) <= 1)
                                                                                       .ToList();

            if (possibleMoveLocations.Count == 0)
            {
                return scoredPath;
            }

            int newIndex = rng.Next(possibleMoveLocations.Count);
            newPath[index + 1] = possibleMoveLocations[newIndex].position;
        }

        return worldModel.ScorePath(newPath);
    }

    private ScoredPath MutateNTimes(GridWorldModel worldModel, ScoredPath scoredPath, int n)
    {
        for (int i = 0; i < n; i++)
        {
            scoredPath = GenerateMutation(worldModel, scoredPath);
        }

        return scoredPath;
    }

    public ScoredPath CalculatePath(GridWorldModel worldModel, Position droneStartingPosition, int steps, int maxRunTimeMilliseconds)
    {
        Stopwatch timer = Stopwatch.StartNew();

        List<ScoredPath> population = Enumerable.Range(0, populationSize)
                                                .Select(p => GenerateRandomPath(worldModel, droneStartingPosition, steps))
                                                .ToList();

        while (timer.ElapsedMilliseconds < maxRunTimeMilliseconds)
        {
            List<ScoredPath> newPopulation = population.Select(p => MutateNTimes(worldModel, p, 10)).ToList();
            population.AddRange(newPopulation);

            population = population.OrderByDescending(p => p.score).Take(populationSize).ToList();
        }

        return population.OrderByDescending(p => p.score).First();
    }
}