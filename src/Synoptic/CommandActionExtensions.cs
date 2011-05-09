using System;
using System.Collections.Generic;
using System.Linq;

namespace Synoptic
{
    internal static class CommandActionExtensions
    {
        internal static void Run(this CommandAction commandAction, IDependencyResolver resolver, CommandLineParseResult parseResult)
        {
            var instance = resolver.Resolve(commandAction.LinkedToMethod.DeclaringType);
            object[] parameterValues = GetCommandParameterValues(commandAction.Parameters, parseResult);
            commandAction.LinkedToMethod.Invoke(instance, parameterValues);
        }

        private static object[] GetCommandParameterValues(IEnumerable<ParameterInfoWrapper> parameters, CommandLineParseResult parseResult)
        {
            var args = new List<object>();
            foreach (var parameter in parameters)
            {
                CommandLineParameter commandLineParameter = 
                    parseResult.ParsedParameters.FirstOrDefault(p => p.Name.SimilarTo(parameter.Name));
                
                object value = null;

                // Method has parameter which was not supplied.
                if (commandLineParameter == null || commandLineParameter.Value == null)
                {
                    if (parameter.DefaultValue != null)
                    {
                        value = parameter.DefaultValue;
                    }
                    else if(parameter.IsRequired)
                    {
                        throw new CommandActionException(String.Format("The parameter '{0}' is required.", parameter.Name));
                    }
                }
                else
                {
                    value = commandLineParameter.Value;
                }

                if (value != null)
                {
                    args.Add(GetConvertedParameterValue(parameter, value));
                    continue;
                }

                args.Add(null);
            }

            return args.ToArray();
        }

        private static object GetConvertedParameterValue(ParameterInfoWrapper parameter, object value)
        {
            if (!parameter.IsValueRequiredWhenOptionIsPresent)
                value = value != null;

            return Convert.ChangeType(value, parameter.Type);
        }
    }
}