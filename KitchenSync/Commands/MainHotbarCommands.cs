using System.Collections.Generic;
using KamiLib.ChatCommands;
using KamiLib.Interfaces;
using KitchenSync.Utilities;

namespace KitchenSync.Commands;

public class MainHotbarCommands : IPluginCommand
{
    public string CommandArgument => "hotbar";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = "enable",
            Aliases = new List<string>{"on"},
            ParameterAction = delegate(string?[]? strings)
            {
                switch (strings)
                {
                    case null:
                    case [ null ]:
                        Chat.PrintError("Hotbar Number Missing");
                        break;
                    
                    case [ { } param ] when int.Parse(param) is >= (int)HotbarName.Hotbar1 + 1 and <= (int)HotbarName.Hotbar10 + 1:
                        SetHotbar(int.Parse(param), true);
                        Chat.Print("Command", $"Enabled Hotbar{int.Parse(param)}");
                        break;
                }
            },
            GetHelpText = () => "Enables a hotbar"
        },
        new SubCommand
        {
            CommandKeyword = "disable",
            Aliases = new List<string>{"off"},
            ParameterAction = delegate(string?[]? strings)
            {
                switch (strings)
                {
                    case null:
                    case [ null ]:
                        Chat.PrintError("Hotbar Number Missing");
                        break;
                    
                    case [ { } param ] when int.Parse(param) is >= (int)HotbarName.Hotbar1 + 1 and <= (int)HotbarName.Hotbar10 + 1:
                        SetHotbar(int.Parse(param), false);
                        Chat.Print("Command", $"Disabled Hotbar{int.Parse(param)}");
                        break;
                }
            },
            GetHelpText = () => "Disables a hotbar"
        },
        new SubCommand
        {
            CommandKeyword = "toggle",
            Aliases = new List<string>{"t"},
            ParameterAction = delegate(string?[]? strings)
            {
                switch (strings)
                {
                    case null:
                    case [ null ]:
                        Chat.PrintError("Hotbar Number Missing");
                        break;
                    
                    case [ { } param ] when int.Parse(param) is >= (int)HotbarName.Hotbar1 + 1 and <= (int)HotbarName.Hotbar10 + 1:
                        SetHotbar(int.Parse(param), !GetHotbar(int.Parse(param)));
                        Chat.Print("Command", $"{(GetHotbar(int.Parse(param)) ? "Enabled" : "Disabled")} Hotbar{int.Parse(param)}");
                        break;
                }
            },
            GetHelpText = () => "Toggles a hotbar"
        },
    };

    private static void SetHotbar(int hotbarIndex, bool value)
    {
        var targetHotbar = (HotbarName)hotbarIndex - 1;
        
        Service.Configuration.HotbarSettings.Hotbars[targetHotbar].Value = value;
    }

    private static bool GetHotbar(int hotbarIndex)
    {
        var targetHotbar = (HotbarName)hotbarIndex - 1;
        
        return Service.Configuration.HotbarSettings.Hotbars[targetHotbar].Value;
    }
}