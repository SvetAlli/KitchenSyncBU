using System.Collections.Generic;
using KamiLib.ChatCommands;
using KamiLib.Interfaces;
using KitchenSync.Utilities;

namespace KitchenSync.Commands;

public class CrossHotbarCommands : IPluginCommand
{
    public string CommandArgument => "crosshotbar";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = "enable",
            Aliases = new List<string>{"on"},
            CommandAction = () =>
            {
                Service.Configuration.HotbarSettings.Hotbars[HotbarName.CrossHotbar].Value = true;
                Service.Configuration.HotbarSettings.Hotbars[HotbarName.DoubleCrossL].Value = true;
                Service.Configuration.HotbarSettings.Hotbars[HotbarName.DoubleCrossR].Value = true;
                Chat.Print("Command", "Enabled CrossHotbars");
            },
            GetHelpText = () => "Enables CrossHotbars"
        },
        new SubCommand
        {
            CommandKeyword = "disable",
            Aliases = new List<string>{"off"},
            CommandAction = () =>
            {
                Service.Configuration.HotbarSettings.Hotbars[HotbarName.CrossHotbar].Value = false;
                Service.Configuration.HotbarSettings.Hotbars[HotbarName.DoubleCrossL].Value = false;
                Service.Configuration.HotbarSettings.Hotbars[HotbarName.DoubleCrossR].Value = false;
                Chat.Print("Command", "Disabled CrossHotbars");
            },
            GetHelpText = () => "Disable CrossHotbars"
        },
        new SubCommand
        {
            CommandKeyword = "toggle",
            Aliases = new List<string>{"t"},
            CommandAction = () =>
            {
                var state = !AnyEnabled();
                
                Service.Configuration.HotbarSettings.Hotbars[HotbarName.CrossHotbar].Value = state;
                Service.Configuration.HotbarSettings.Hotbars[HotbarName.DoubleCrossL].Value = state;
                Service.Configuration.HotbarSettings.Hotbars[HotbarName.DoubleCrossR].Value = state;
                Chat.Print("Command", $"{(state ? "Enabled" : "Disabled")} CrossHotbars");
            },
            GetHelpText = () => "Toggles CrossHotbars"
        },
    };

    private static bool AnyEnabled()
    {
        return Service.Configuration.HotbarSettings.Hotbars[HotbarName.CrossHotbar].Value ||
               Service.Configuration.HotbarSettings.Hotbars[HotbarName.DoubleCrossL].Value ||
               Service.Configuration.HotbarSettings.Hotbars[HotbarName.DoubleCrossR].Value;
    }
}