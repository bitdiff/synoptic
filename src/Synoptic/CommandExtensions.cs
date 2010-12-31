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

            ConstructorInfo constructorInfo = methodInfo.DeclaringType.BestMatch(parseResult);

            if (constructorInfo == null)
            {
                throw new CommandException("Parameter types need at least one constructor");
            }

            object o = constructorInfo.Invoke(GetObjects(constructorInfo.GetParameters(), parseResult));

            methodInfo.Invoke(o, objects);
        }

        internal static object[] GetObjects(IEnumerable<ParameterInfo> parameterInfos, CommandLineParseResult parseResult)
        {
            var args = new List<object>();
            
            foreach (var parameterInfo in parameterInfos)
            {
                CommandLineParameter commandLineParameter = parseResult.ParsedParameters.FirstOrDefault(p => p.Name.EqualEnough(parameterInfo.Name));

                if (!parameterInfo.ParameterType.IsPrimitive && parameterInfo.ParameterType != typeof (string))
                {
                    ConstructorInfo constructorInfo = parameterInfo.ParameterType.BestMatch(parseResult);
                    object o = constructorInfo.Invoke(GetObjects(constructorInfo.GetParameters(), parseResult));
                    args.Add(o);
                    continue;
                }

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

        internal static ConstructorInfo BestMatch(this Type type, CommandLineParseResult parseResult)
        {
            ConstructorInfo result = null;
            var maxMatch = 0;
            foreach (var constructorInfo in type.GetConstructors())
            {
                if (result == null)
                    result = constructorInfo;

                var match = 0;
                foreach (var parameterInfo in constructorInfo.GetParameters())
                {
                    CommandLineParameter commandLineParameter =
                        parseResult.ParsedParameters.FirstOrDefault(p => p.Name.EqualEnough(parameterInfo.Name));
                    if (commandLineParameter != null)
                        match++;
                }

                if (match > maxMatch)
                {
                    maxMatch = match;
                    result = constructorInfo;
                }
            }

            return result;
        }
    }
}