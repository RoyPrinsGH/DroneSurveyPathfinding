using System.Text;

using Position = (int x, int y);

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
    public const int CELL_GROWTH_PER_TICK = 1;

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

    public void Update()
    {
        foreach (int y in Enumerable.Range(0, Height))
        {
            foreach (int x in Enumerable.Range(0, Width))
            {
                Grid[x, y].value = Math.Min(Grid[x, y].value + CELL_GROWTH_PER_TICK, Grid[x, y].maxValue);
            }
        }
    }

    public int ValueOfCellAfterTakingPath(List<Position> path, Position position)
    {
        if (!path.Contains(position))
        {
            return Grid[position.x, position.y].value;
        }

        int ticksSinceLastVisit = path.LastIndexOf(position);
        return Math.Min(ticksSinceLastVisit, Grid[position.x, position.y].maxValue);
    }

    public IEnumerable<(Position, CellData)> GetNeighbors(Position position)
    {
        (int x, int y) = position;

        if (x > 0)
        {
            yield return ((x - 1, y), Grid[x - 1, y]);

            if (y > 0)
            {
                yield return ((x - 1, y - 1), Grid[x - 1, y - 1]);
            }

            if (y < Height - 1)
            {
                yield return ((x - 1, y + 1), Grid[x - 1, y + 1]);
            }
        }

        if (y > 0)
        {
            yield return ((x, y - 1), Grid[x, y - 1]);
        }

        if (y < Height - 1)
        {
            yield return ((x, y + 1), Grid[x, y + 1]);
        }

        if (x < Width - 1)
        {
            yield return ((x + 1, y), Grid[x + 1, y]);

            if (y > 0)
            {
                yield return ((x + 1, y - 1), Grid[x + 1, y - 1]);
            }

            if (y < Height - 1)
            {
                yield return ((x + 1, y + 1), Grid[x + 1, y + 1]);
            }
        }
    }
}