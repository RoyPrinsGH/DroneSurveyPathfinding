using System.Text;

using Position = (int x, int y);
using Path = System.Collections.Generic.List<(int x, int y)>;
using ScoredPath = (int score, System.Collections.Generic.List<(int x, int y)> path);

namespace DroneSurveyPathfinding;

public struct CellData
{
    public int value;
    public int maxValue;

    public override string ToString()
    {
        return value.ToString();
    }
}

public class GridWorldModel
{
    public const int TICKS_PER_INCREASE = 5;

    public int Width { get; init; }
    public int Height { get; init; }
    public CellData[,] Grid { get; private init; }

    public GridWorldModel(int width, int height)
    {
        Width = width;
        Height = height;
        Grid = new CellData[width, height];
    }

    public GridWorldModel(List<List<int>> data)
    {
        bool everyRowSameLength = data.Select(row => row.Count).Distinct().Count() == 1;
        if (!everyRowSameLength)
        {
            throw new FormatException("Grid rows must be of consistent length");
        }

        Width = data.Count;
        Height = data[0].Count;

        Grid = new CellData[Width, Height];

        foreach (int x in Enumerable.Range(0, Width))
        {
            foreach (int y in Enumerable.Range(0, Height))
            {
                Grid[x, y] = new CellData
                {
                    value = data[y][x],
                    maxValue = data[y][x]
                };
            }
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new();

        foreach (int y in Enumerable.Range(0, Height))
        {
            sb.AppendLine(string.Join(' ', Enumerable.Range(0, Width).Select(x => Grid[x, y])));
        }

        return sb.ToString();
    }

    public int ValueOfCellAfterTakingPath(Path path, Position position)
    {
        int lastIndex = path.LastIndexOf(position);

        if (lastIndex == -1)
        {
            return Grid[position.x, position.y].value;
        }

        int ticksSinceLastVisit = path.Count - lastIndex - 1;

        return Math.Min(ticksSinceLastVisit / TICKS_PER_INCREASE, Grid[position.x, position.y].maxValue);
    }

    public ScoredPath ScorePath(Path path)
    {
        int score = 0;
        for (int i = 0; i < path.Count; i++)
        {
            score += ValueOfCellAfterTakingPath(path[..i], path[i]);
        }
        return (score, path);
    }

    public IEnumerable<Position> GetNeighboringPositions(Position position)
    {
        (int x, int y) = position;

        if (x > 0)
        {
            yield return (x - 1, y);

            if (y > 0)
            {
                yield return (x - 1, y - 1);
            }

            if (y < Height - 1)
            {
                yield return (x - 1, y + 1);
            }
        }

        if (y > 0)
        {
            yield return (x, y - 1);
        }

        if (y < Height - 1)
        {
            yield return (x, y + 1);
        }

        if (x < Width - 1)
        {
            yield return (x + 1, y);

            if (y > 0)
            {
                yield return (x + 1, y - 1);
            }

            if (y < Height - 1)
            {
                yield return (x + 1, y + 1);
            }
        }
    }
}