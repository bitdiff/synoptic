using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Mono.Options;
using Synoptic.ConsoleFormat;
using Synoptic.Exceptions;

namespace Synoptic.HelpUtilities
{
    internal class HelpGenerator
    {
        private string GenerateUsagePattern(IEnumerable<Option> optionSet)
        {
            var output = new StringBuilder();
            if (optionSet != null)
            {
                foreach (Option o in optionSet)
                {
                    if (o.OptionValueType == OptionValueType.None)
                    {
                        output.AppendFormat("[--{0}]", o.Prototype);
                    }
                    if (o.OptionValueType == OptionValueType.Required)
                    {
                        output.AppendFormat("[--{0}VALUE]", o.Prototype);
                    }

                    output.Append(" ");
                }
            }

            return output.Append("COMMAND ACTION [ARGS]").ToString();
        }

        public void ShowCommandHelp(IEnumerable<Command> availableCommands, OptionSet optionSet)
        {
            var processName = Process.GetCurrentProcess().ProcessName;
            var usagePreamble = String.Format("Usage: {0} ", processName);
            var usagePattern = GenerateUsagePattern(optionSet);

            ConsoleFormatter.Write(
                new ConsoleTable().AddRow(
                        new ConsoleCell(usagePreamble).WithWidth(usagePreamble.Length).WithPadding(0),
                        new ConsoleCell(usagePattern).WithPadding(0)));

            var maximumCommandNameLength = availableCommands.Count() > 0 ? availableCommands.Max(c => c.Name.Length) : 0;

            var commandTable = new ConsoleTable().
                AddEmptyRow().AddRow(
                    new ConsoleCell("The available commands are:").WithPadding(0));

            foreach (var command in availableCommands)
            {
                commandTable.AddRow(
                        new ConsoleCell(command.Name).WithWidth(maximumCommandNameLength),
                        new ConsoleCell(command.Description));
            }

            ConsoleFormatter.Write(commandTable);
        }

        public void ShowCommandHelp(Command command, IEnumerable<CommandAction> availableActions)
        {
            var maximumActionNameLength = availableActions.Count() > 0 ? availableActions.Max(c => c.Name.Length) : 0;

            var actionTable =
                new ConsoleTable().AddRow(
                    new ConsoleCell(String.Format("The available actions for command '{0}' are:", command.Name)).WithPadding(0));

            foreach (var action in availableActions)
            {
                actionTable.AddRow(
                        new ConsoleCell(action.Name).WithWidth(maximumActionNameLength),
                        new ConsoleCell(action.Description));
            }

            ConsoleFormatter.Write(actionTable);
        }

        public void ShowExceptionHelp(CommandNotFoundException exception)
        {
            ConsoleFormatter.Write(new ConsoleTable()
                                       .AddRow(
                                           new ConsoleCell("'{0}' is not a valid command.",
                                                           exception.CommandName).WithPadding(0)));

            var formattedCommandList = string.Join(" or ",
                                                   (exception.PossibleCommands.Count > 0
                                                        ? exception.PossibleCommands
                                                        : exception.AvailableCommands).Select(
                                                            c => String.Format("'{0}'", c.Name)).ToArray());

            ConsoleFormatter.Write(new ConsoleTable()
                                       .AddEmptyRow()
                                       .AddRow(
                                           new ConsoleCell("Did you mean:").WithPadding(0))
                                       .AddRow(
                                           new ConsoleCell(formattedCommandList)));
        }

        public void ShowExceptionHelp(ActionNotFoundException exception)
        {
            if (exception.ActionName == null)
            {
                ConsoleFormatter.Write(new ConsoleTable()
                                           .AddRow(
                                               new ConsoleCell("You must specify an action name for command '{0}'.",
                                                               exception.Command.Name).WithPadding(0)));
            }
            else
            {
                ConsoleFormatter.Write(new ConsoleTable()
                                           .AddRow(
                                               new ConsoleCell("'{0}' is not a valid action for command '{1}'.",
                                                               exception.ActionName, exception.Command.Name).WithPadding(0)));
            }

            var formattedActionList = string.Join(" or ",
                                                  (exception.PossibleActions.Count > 0
                                                       ? exception.PossibleActions
                                                       : exception.AvailableActions).Select(
                                                           c => String.Format("'{0}'", c.Name)).ToArray());

            ConsoleFormatter.Write(new ConsoleTable()
                                       .AddEmptyRow()
                                       .AddRow(
                                           new ConsoleCell("Did you mean:").WithPadding(0))
                                       .AddRow(
                                           new ConsoleCell(formattedActionList)));
        }
    }
}