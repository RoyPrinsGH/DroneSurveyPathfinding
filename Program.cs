using System.Reflection;
using DroneSurveyPathfinding.Algorithms;
using Spectre.Console;

using Position = (int x, int y);
using ScoredPath = (int score, System.Collections.Generic.List<(int x, int y)> path);

#pragma warning disable CS0162

namespace DroneSurveyPathfinding;

public static class Program
{
    private const bool DEBUG = false;

    public static void Main(string[] args)
    {
        if (DEBUG)
        {
            RunNonInteractive();
        }
        else 
        {
            RunInteractive(args);
        }
    }

    private static void RunInteractive(string[] args)
    {
        var options = ArgParser.Parse(args);

        if (options is null)
        {
            return;
        }

        (string filePath, Position droneStartingPosition, int maxTicks, int maxRunTimeMilliseconds, bool animate) = options.Value;

        Console.WriteLine($"Running on file {filePath} with maxTicks = {maxTicks}, maxRunTime = {maxRunTimeMilliseconds} starting at {droneStartingPosition}");

        GridWorldModel? worldModel = GridWorldModelImporter.TryImportGridWorldModel(filePath);

        if (worldModel is null)
        {
            return;
        }

        if (droneStartingPosition.x > worldModel.Width || droneStartingPosition.y > worldModel.Height)
        {
            AnsiConsole.MarkupLine("[red]Starting position must be in bounds of the grid.[/]");
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
            ScoredPath result = algorithm.CalculatePath(worldModel, droneStartingPosition, maxTicks, maxRunTimeMilliseconds);

            if (animate)
            {
                for (int i = 0; i < result.path.Count; i++)
                {
                    Display.DisplayState(worldModel, result.path[i], result.path[..i]);
                    Thread.Sleep(100);
                }
            }

            if (worldModel.Width <= 100)
            {
                Display.DisplayState(worldModel, result.path[^1], result.path);
            }

            Console.WriteLine($"{algorithm} - Final score: {result.score}");
        }
    }

    private static void RunNonInteractive()
    {
        GridWorldModel? worldModel = GridWorldModelImporter.TryImportGridWorldModel("Grids/100.txt");

        if (worldModel is null)
        {
            return;
        }

        ISurveyPathfinderAlgorithm algToDebug = new MCSurveyPathfinder();

        ScoredPath result = algToDebug.CalculatePath(worldModel, (49, 49), 100, 10000);

        Display.DisplayState(worldModel, result.path[^1], result.path);
        Console.WriteLine($"{algToDebug} - Final score: {result.score}");
    }
}