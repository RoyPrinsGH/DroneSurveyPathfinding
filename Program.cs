using System.Reflection;
using DroneSurveyPathfinding.Algorithms;
using Spectre.Console;

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
            .Title("Select project(s) to build")
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
            var (path, totalScore) = algorithm.CalculatePath(worldModel, (49, 49), 100, 1000);
            Display.DisplayState(worldModel, path[^1], path);
            Console.WriteLine($"{algorithm} - Final score: {totalScore}");
        }
    }
}