using System;
using System.Collections.Generic;
using Dalamud.Game.Command;
using KitchenSync.Interfaces;
using KitchenSync.System.Commands;

namespace KitchenSync.System;

internal class CommandManager : IDisposable
{
    private const string SettingsCommand = "/ksink";

    private readonly List<IPluginCommand> commands = new()
    {
        new ConfigurationWindowCommand(),
    };

    public CommandManager()
    {
        Service.Commands.AddHandler(SettingsCommand, new CommandInfo(OnCommand)
        {
            HelpMessage = "open configuration window"
        });
    }

    public void Dispose()
    {
        Service.Commands.RemoveHandler(SettingsCommand);
    }

    private void OnCommand(string command, string arguments)
    {
        var subCommand = GetPrimaryCommand(arguments);
        var subCommandArguments = GetSecondaryCommand(arguments);

        switch (subCommand)
        {
            case null:
                commands[0].Execute(subCommandArguments);
                break;
        }
    }
    private static string? GetSecondaryCommand(string arguments)
    {
        var stringArray = arguments.Split(' ');

        if (stringArray.Length == 1)
        {
            return null;
        }

        return stringArray[1];
    }

    private static string? GetPrimaryCommand(string arguments)
    {
        var stringArray = arguments.Split(' ');

        if (stringArray[0] == string.Empty)
        {
            return null;
        }

        return stringArray[0];
    }
}