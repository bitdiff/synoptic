using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Mono.Options;
using Synoptic.ConsoleFormat;

namespace Synoptic.HelpUtilities
{
    internal class CommandLineHelpGenerator
    {
        public static CommandLineHelp Generate(IEnumerable<CommandAction> commandActions)
        {
            return new CommandLineHelp(commandActions);
        }

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
                new ConsoleTable(
                    new ConsoleRow(
                        new ConsoleCell(usagePreamble) { Width = usagePreamble.Length, Padding = 0 },
                        new ConsoleCell(usagePattern) { Padding = 0 })));

            var maximumCommandNameLength = availableCommands.Count() > 0 ? availableCommands.Max(c => c.Name.Length) : 0;

            var commandTable = new ConsoleTable("\nThe available commands are:");

            foreach (var command in availableCommands)
            {
                commandTable.AddRow(
                    new ConsoleRow(
                        new ConsoleCell(command.Name.ToHyphened()) { Width = maximumCommandNameLength },
                        new ConsoleCell(command.Description)));
            }

            foreach(var row in commandTable.Rows.Skip(1))
            {
                row.Cells.ElementAt(0).Style.ForegroundColor = ConsoleColor.Black;
                row.Cells.ElementAt(0).Style.BackgroundColor = ConsoleColor.Blue;
            }

            ConsoleFormatter.Write(commandTable);
        }
    }
}