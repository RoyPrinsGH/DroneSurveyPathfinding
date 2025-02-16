using System.Reflection;
using DroneSurveyPathfinding.Algorithms;
using Spectre.Console;

using Position = (int x, int y);
using ScoredPath = (int score, System.Collections.Generic.List<(int x, int y)> path);

namespace DroneSurveyPathfinding;

public static class Program
{
    public static void Main(string[] args)
    {
        GridWorldModel? worldModel = GridWorldModelImporter.TryImportGridWorldModel("Grids/100.txt");

        if (worldModel is null)
        {
            return;
        }

        var algorithms = Assembly.GetExecutingAssembly().GetTypes()
            .Where(at => at.GetInterfaces().Contains(typeof(ISurveyPathfinderAlgorithm)))
            .Select(at => (ISurveyPathfinderAlgorithm)Activator.CreateInstance(at)!)
            .ToList();

        if (algorithms.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] No runnable algorithms found.");
            return;
        }

        var runnableAlgorithmsPrompt = new MultiSelectionPrompt<ISurveyPathfinderAlgorithm>()
            .Title("Select algorithm(s) to run")
            .AddChoices(algorithms)
            .UseConverter(alg => alg.ToString()!)
            .NotRequired();

        var algorithmsToRun = AnsiConsole.Prompt(runnableAlgorithmsPrompt);

        if (algorithmsToRun.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No algorithms selected.[/]");
            return;
        }

        foreach (ISurveyPathfinderAlgorithm algorithm in algorithmsToRun)
        {
            ScoredPath result = algorithm.CalculatePath(worldModel, (36, 78), 1000, 1000);

            if (worldModel.Width <= 100)
            {
                Display.DisplayState(worldModel, result.path[^1], result.path);
            }

            Console.WriteLine($"{algorithm} - Final score: {result.score}");
        }
    }
}