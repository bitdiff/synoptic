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
            MethodInfo methodInfo = command.LinkedToMethod;
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();

            object[] objects = GetObjects(parameterInfos, parseResult);

            object o = Activator.CreateInstance(methodInfo.DeclaringType);

            methodInfo.Invoke(o, objects);
        }

        internal static object[] GetObjects(IEnumerable<ParameterInfo> parameterInfos, CommandLineParseResult parseResult)
        {
            var args = new List<object>();
            
            foreach (var parameterInfo in parameterInfos)
            {
                CommandLineParameter commandLineParameter = parseResult.ParsedParameters.FirstOrDefault(p => p.Name.EqualEnough(parameterInfo.Name));

                if (commandLineParameter != null)
                {
                    var value = commandLineParameter.Value;
                    if (parameterInfo.ParameterType == typeof(bool))
                        value = commandLineParameter.Name.Equals(commandLineParameter.Value,
                                                                                      StringComparison.OrdinalIgnoreCase).ToString();

                    object changeType = Convert.ChangeType(value, parameterInfo.ParameterType);
                    args.Add(changeType);
                    continue;
                }
                
                args.Add(null);
            }

            return args.ToArray();
        }
    }
}