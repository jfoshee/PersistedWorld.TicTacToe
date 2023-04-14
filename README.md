# Persisted.World GameMaster Starter Project

## What is a "GameMaster"?

Paraphrasing multiple dictionaries, a GameMaster is:

> A person who organizes and oversees a role-playing game

The Persisted.World back-end enables implementing a GameMaster in code. 
We will be focused on enforcing the rules of our game
by allowing only legal state changes.


## Getting Started

Setup the development environment

1. [Visual Studio Code](https://code.visualstudio.com/) 
2. [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks)

Clone or copy this repository and open the directory in VS Code.

The `tasks.json` file has been seeded with some helpful tasks that can be launched from VS Code.

Restore dotnet tools via `dotnet tool restore`.

Test your environment by running the Test Task. 

<kbd>ctrl</kbd> + <kbd>shift</kbd> + <kbd>p</kbd>

```
> Tasks: Run Test Task
```

The test project should build and all tests should run and pass.


## Customizing for Your Game

The first thing to do is to select a unique ID for your game.  
The format is not important, although I use kebab-case.  
I recommend the ID is the unique name of the game plus `-game-master`. 
This example uses `tic-tac-toe-game-master`.

Update the following locations with your GameMaster Name and ID.

1. `gamemaster.json`
2. `Constants.cs`

Next rename `TicTacToeTestAttribute` to correspond to your game. 

The existing Tic Tac toe scripts and tests may be adapted as a starting point for simple games.
