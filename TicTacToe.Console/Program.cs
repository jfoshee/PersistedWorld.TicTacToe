﻿using System.Collections.Immutable;
using static System.Console;
using static TicTacToe.Console.Constants;
using static TicTacToe.Console.ConsoleUi;

// Setup services
var baseUrl = "https://localhost:7264";
var services = new ServiceCollection();
services.AddGameStateClientServices(GameMasterId, baseUrl);
var serviceProvider = services.BuildServiceProvider();
var game = serviceProvider.GetService<IGameTestHarness>();
var gameStateClient = serviceProvider.GetRequiredService<IGameStateClient>();
var changeUserService = serviceProvider.GetRequiredService<IChangeUserService>();

// Login
var playerId = Input("Player ID");
playerId = playerId.Trim();
await changeUserService.ChangeUserAsync(playerId);

// Enumerate the player's boards
var owned = await gameStateClient.GetEntitiesOwnedAsync()
            ?? throw new Exception("Failed to fetch owned entities");
var boards = owned.Where(entity => entity.IsTicTacToeBoard())
                  .ToImmutableList();
WriteLine("Your game boards: ");
for (int i = 0; i < boards.Count; i++)
{
    var board = boards[i];
    WriteLine($"{i} {board.Id}");
}