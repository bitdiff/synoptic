using System;

namespace Synoptic
{
    internal static class PrototypeGenerator
    {
        private const string DefaultSuffix = "=";

        internal static string ToOptionPrototype(this ParameterInfoWrapper parameter)
        {
            string suffix = parameter.IsValueRequiredWhenOptionIsPresent ? DefaultSuffix : "";

            if (String.IsNullOrEmpty(parameter.Prototype))
                return parameter.Name.ToHyphened() + suffix;

            string proto = parameter.Prototype.Trim();
            
            if (parameter.IsValueRequiredWhenOptionIsPresent)
            {
                if (proto.EndsWith("=") || proto.EndsWith(":"))
                    return proto;

                return proto + DefaultSuffix;
            }

            if (proto.EndsWith("=") || proto.EndsWith(":"))
                return proto.TrimEnd('=', ':');

            return proto;
        }
    }
}