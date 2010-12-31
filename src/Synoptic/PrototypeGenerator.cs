using System;

namespace Synoptic
{
    internal static class PrototypeGenerator
    {
        internal static string ToOptionPrototype(ParameterInfoWrapper parameter)
        {
            string suffix = parameter.IsOptionValueRequired ? ":" : "";

            if (!String.IsNullOrEmpty(parameter.Prototype))
                // In case the prototype already includes the mandatory syntax.
                return parameter.Prototype.TrimEnd(':') + suffix;

            return parameter.Name.ToHyphened() + suffix;
        }
    }
}