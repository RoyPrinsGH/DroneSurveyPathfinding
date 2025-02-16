namespace DroneSurveyPathfinding;

public static class GridWorldModelImporter
{
    public static GridWorldModel? TryImportGridWorldModel(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(filePath);
        }

        List<List<int>> gridWorldData = [];

        using StreamReader sr = new(filePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            try {
                List<int> row = [.. line.Split().Select(int.Parse)];
                gridWorldData.Add(row);
            }
            catch (FormatException e)
            {
                Console.WriteLine($"Grid cell data must be integer: {e.Message}");
                return null;
            }
        }

        try 
        {
            return new GridWorldModel(gridWorldData);
        }
        catch (FormatException e)
        {
            Console.WriteLine($"Failed to import grid world data: {e.Message}");
            return null;
        }
    }
}