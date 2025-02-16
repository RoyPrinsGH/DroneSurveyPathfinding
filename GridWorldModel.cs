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
    public int MaxValue { get; init;}
    public CellData[,] Grid { get; private init; }

    public GridWorldModel(int width, int height)
    {
        Width = width;
        Height = height;
        MaxValue = 0;
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
                int cellValue = data[y][x];

                Grid[x, y] = new CellData
                {
                    value = cellValue,
                    maxValue = cellValue
                };

                if (cellValue > MaxValue)
                {
                    MaxValue = cellValue;
                }
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
        CellData cellData = Grid[position.x, position.y];

        for (int i = path.Count - 1; i >= Math.Max(0, path.Count - 1 - cellData.maxValue * TICKS_PER_INCREASE); i--)
        {
            if (path[i] == position)
            {
                return Math.Min((path.Count - 1 - i) / TICKS_PER_INCREASE, Grid[position.x, position.y].maxValue);
            }
        }

        return cellData.value;
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