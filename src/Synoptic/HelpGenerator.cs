using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Mono.Options;
using Synoptic.ConsoleFormat;

namespace Synoptic
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

        public void ShowCommandUsage(IEnumerable<Command> availableCommands, OptionSet optionSet)
        {
            var processName = Process.GetCurrentProcess().ProcessName;
            var usagePreamble = String.Format("Usage: {0} ", processName);
            var usagePattern = GenerateUsagePattern(optionSet);

            new ConsoleFormatter(ConsoleWriter.Default).Write(
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

            new ConsoleFormatter(ConsoleWriter.Default).Write(commandTable);
        }

        public void ShowCommandHelp(Command command, List<CommandAction> availableActions)
        {
            var maximumActionNameLength = availableActions.Count() > 0 ? availableActions.Max(c => c.Name.Length) : 0;

            var actionTable =
                new ConsoleTable().AddRow(
                    new ConsoleCell(String.Format("The available actions for command '{0}' are:", command.Name)).WithPadding(0));

            int maximumPrototypeLength = 0;

            foreach (var action in availableActions)
            {
                if (action.Parameters.Count > 0)
                {
                    var prototypeLength = action.Parameters
                        .Max(c => c.GetOptionPrototypeHelp().Length);

                    if (prototypeLength > maximumPrototypeLength)
                        maximumPrototypeLength = prototypeLength;
                }
            }

            foreach (var action in availableActions)
            {
                actionTable.AddRow(
                    new ConsoleCell(action.Name).WithWidth(maximumActionNameLength),
                    new ConsoleCell(action.Description));

                if (action.Parameters.Count > 0)
                {
                    foreach (var p in action.Parameters)
                    {
                        actionTable.AddRow(new ConsoleCell(p.GetOptionPrototypeHelp()).
                                               WithWidth(maximumPrototypeLength).WithPadding(6),
                                           new ConsoleCell(p.Description));
                    }
                }

                if (availableActions.Last() != action)
                    actionTable.AddEmptyRow();
            }

            new ConsoleFormatter(ConsoleWriter.Default).Write(actionTable);
        }
    }
}