using System.Text;
using Spectre.Console;

namespace DroneSurveyPathfinding;

public static class Display
{
    public static void DisplayState(GridWorldModel worldModel, (int x, int y) dronePosition, List<(int x, int y)> path)
    {
        StringBuilder sb = new();

        Func<(int x, int y), string> formatCell = ((int x, int y) position) =>
        {
            string cellValueString = worldModel.Grid[position.x, position.y].ToString().PadRight(2);

            if (position == dronePosition)
            {
                return "[red]" + cellValueString + "[/]";
            }
            if (path.Contains(position))
            {
                return "[yellow]" + cellValueString + "[/]";
            }

            return cellValueString;
        };

        foreach (int y in Enumerable.Range(0, worldModel.Height))
        {
            sb.AppendLine(string.Join(' ', Enumerable.Range(0, worldModel.Width).Select(x => formatCell((x, y)))));
        }

        AnsiConsole.Markup(sb.ToString());
    }
}