using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Synoptic
{
    internal static class CommandExtensions
    {
        internal static void Run(this Command command, CommandLineParseResult parseResult)
        {
            MethodInfo methodToInvoke = command.LinkedToMethod;
            object[] objects = GetCommandParameterValues(command.Parameters, parseResult);

            object instance = Activator.CreateInstance(methodToInvoke.DeclaringType);
            methodToInvoke.Invoke(instance, objects);
        }

        internal static object[] GetCommandParameterValues(IEnumerable<ParameterInfoWrapper> parameters, CommandLineParseResult parseResult)
        {
            var args = new List<object>();
            foreach (var parameter in parameters)
            {
                CommandLineParameter commandLineParameter = parseResult.ParsedParameters.FirstOrDefault(p => p.Name.SimilarTo(parameter.Name));
                object value = null;

                // Method has parameter which was not supplied.
                if (commandLineParameter == null || commandLineParameter.Value == null)
                {
                    if (parameter.DefaultValue != null)
                    {
                        value = parameter.DefaultValue;
                    }
                }
                else
                {
                    value = commandLineParameter.Value;
                }

                if (value != null)
                {
                    args.Add(GetConvertedParameter(parameter, value));
                    continue;
                }

                args.Add(null);
            }

            return args.ToArray();
        }

        private static object GetConvertedParameter(ParameterInfoWrapper parameter, object value)
        {
            if (!parameter.IsOptionValueRequired)
                value = value != null;

            return Convert.ChangeType(value, parameter.Type);
        }
    }
}