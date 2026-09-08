using Microsoft.Extensions.Hosting;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Text;
using CatFactManager.Clients;
using CatFactManager.Models;
using CatFactManager.Services;

namespace CatFactManager
{
    public class Worker : BackgroundService
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ICatFactApiClient _catFactApiClient;
        private readonly ICatFactService _catFactService;
        private readonly string _filePath = "cat-facts.txt";

        public Worker(IHostApplicationLifetime applicationLifetime, ICatFactApiClient catFactApiClient, ICatFactService catFactService)
        {
            _applicationLifetime = applicationLifetime;
            _catFactApiClient = catFactApiClient;
            _catFactService = catFactService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("Cat Fact Manager").LeftJustified());

            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine();
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Options")
                        .PageSize(10)
                        .HighlightStyle(new Style(foreground: Color.Green))
                        .MoreChoicesText("[grey](Move up and down to see more)[/]")
                        .AddChoices(new[] {
                            " - Get a fact",
                            " - Read all facts",
                            " - Update a fact",
                            " - Delete a fact",
                            " - Delete all facts",
                            "<- Exit program"
                        }));
                switch (choice)
                {
                    case " - Get a fact":
                        await GetFactAndSaveAsync();
                        break;
                    case " - Read all facts":
                        await ReadFactsAsync();
                        break;
                    case " - Update a fact":
                        await UpdateFactAsync();
                        break;
                    case " - Delete a fact":
                        await DeleteFactAsync();
                        break;
                    case " - Delete all facts":
                        await DeleteAllFactsAsync();
                        break;
                    case "<- Exit program":
                        _applicationLifetime.StopApplication();
                        return;
                    default:
                        break;
                }
            }
        }

        private async Task GetFactAndSaveAsync()
        {
            var catFact = await _catFactApiClient.GetCatFactAsync();
            if (catFact == null)
            {
                AnsiConsole.MarkupLine("[red]Failed to get a fact.[/]");
                return;
            }
            var response = await _catFactService.AppendFactAsync(catFact, _filePath);

            if (response.Success)
            {
                AnsiConsole.MarkupLine($"[green]Fact saved to {_filePath}[/]\n");
                var panel = new Panel($"{catFact.Fact}\n\nLength: {catFact.Length}")
                {
                    Header = new PanelHeader($"Fact"),
                    Padding = new Padding(2, 1, 2, 1),
                    Expand = false
                };
                AnsiConsole.Write(panel);
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]{response.Message}[/]");
            }
        }

        private async Task ReadFactsAsync()
        {
            var response = await _catFactService.ReadFactsAsync(_filePath);
            if (!response.Success)
            {
                AnsiConsole.MarkupLine($"[red]{response.Message}[/]");
                return;
            }
            else if (response.Facts == null)
            {
                AnsiConsole.MarkupLine($"[yellow]{response.Message}[/]");
                return;
            }

            AnsiConsole.MarkupLine($"[blue]Facts read from {_filePath}[/]\n");
            int counter = 1;
            foreach (var fact in response.Facts)
            {
                var panel = new Panel($"{fact.Fact}\n\nLength: {fact.Length}")
                {
                    Header = new PanelHeader($"Fact {counter}"),
                    Padding = new Padding(2, 1, 2, 1),
                    Expand = false
                };
                AnsiConsole.Write(panel);
                counter++;
            }
        }

        private async Task UpdateFactAsync()
        {
            var response = await _catFactService.ReadFactsAsync(_filePath);
            if (!response.Success)
            {
                AnsiConsole.MarkupLine($"[red]{response.Message}[/]");
                return;
            }
            else if (response.Facts == null)
            {
                AnsiConsole.MarkupLine($"[yellow]{response.Message}[/]");
                return;
            }

            List<CatFact> facts = response.Facts;

            var choices = facts
                .Select((fact, index) => (Index: index + 1, Fact: fact))
                .ToList();

            choices.Insert(0, (Index: 0, Fact: null!));

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<(int Index, CatFact Fact)>()
                    .Title("Select a fact to update:")
                    .PageSize(10)
                    .HighlightStyle(new Style(foreground: Color.Green))
                    .MoreChoicesText("[grey](Move up and down to see more)[/]")
                    .UseConverter(item =>
                    {
                        if (item.Fact == null)
                        {
                            return "<- Return to main menu";
                        }

                        if (string.IsNullOrEmpty(item.Fact.Fact))
                        {
                            return $"{item.Index}. [red]Fact is empty[/]\n";
                        }

                        string preview = item.Fact.Length > 50 ? item.Fact.Fact.Substring(0, 50) + "..." : item.Fact.Fact;

                        return $"{item.Index}. {preview}";
                    })
                    .AddChoices(choices));

            if (choice.Fact == null)
            {
                return;
            }

            var factToUpdate = choice.Fact;

            CatFact? newFact = await _catFactApiClient.GetCatFactAsync();
            if (newFact == null)
            {
                AnsiConsole.MarkupLine("[red]Failed to get a new fact.[/]");
                return;
            }

            factToUpdate.Fact = newFact.Fact;
            factToUpdate.Length = newFact.Length;

            var saveResponse = await _catFactService.SaveFactsAsync(facts, _filePath);

            if (saveResponse.Success)
            {
                AnsiConsole.MarkupLine($"[green]Fact updated successfully in {_filePath}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]{saveResponse.Message}[/]");
            }
        }

        private async Task DeleteFactAsync()
        {
            var response = await _catFactService.ReadFactsAsync(_filePath);
            if (!response.Success)
            {
                AnsiConsole.MarkupLine($"[red]{response.Message}[/]");
                return;
            }
            else if (response.Facts == null)
            {
                AnsiConsole.MarkupLine($"[yellow]{response.Message}[/]");
                return;
            }

            List<CatFact> facts = response.Facts;

            var choices = facts
                .Select((fact, index) => (Index: index + 1, Fact: fact))
                .ToList();

            choices.Insert(0, (Index: 0, Fact: null!));

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<(int Index, CatFact Fact)>()
                    .Title("Select a fact to delete:")
                    .PageSize(10)
                    .HighlightStyle(new Style(foreground: Color.Green))
                    .MoreChoicesText("[grey](Move up and down to see more)[/]")
                    .UseConverter(item =>
                    {
                        if (item.Fact == null)
                        {
                            return "<- Return to main menu";
                        }

                        if (string.IsNullOrEmpty(item.Fact.Fact))
                        {
                            return $"{item.Index}. [red]Fact is empty[/]";
                        }

                        string preview = item.Fact.Length > 50 ? item.Fact.Fact.Substring(0, 50) + "..." : item.Fact.Fact;

                        return $"{item.Index}. {preview}";
                    })
                    .AddChoices(choices));

            if (choice.Fact == null)
            {
                return;
            }

            var factToDelete = choice.Fact;

            facts.Remove(factToDelete);

            var saveResponse = await _catFactService.SaveFactsAsync(facts, _filePath);

            if (saveResponse.Success)
            {
                AnsiConsole.MarkupLine($"[green]Fact deleted successfully in {_filePath}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]{saveResponse.Message}[/]");
            }
        }

        private async Task DeleteAllFactsAsync()
        {
            var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Are you sure you want to delete all facts?")
                        .PageSize(10)
                        .HighlightStyle(new Style(foreground: Color.Green))
                        .MoreChoicesText("[grey](Move up and down to see more)[/]")
                        .AddChoices(new[] {
                            "No",
                            "Yes"
                        }));

            if (choice == "Yes")
            {
                var saveResponse = await _catFactService.SaveFactsAsync(new List<CatFact>(), _filePath);
                if (saveResponse.Success)
                {
                    AnsiConsole.MarkupLine($"[green]All facts deleted successfully in {_filePath}[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[red]{saveResponse.Message}[/]");
                }
            }
        }
    }
}
