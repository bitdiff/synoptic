using System.Collections.Generic;
using System.Linq;

namespace Synoptic
{
    internal static class CommandNameExtractor
    {
        public static string Extract(IEnumerable<CommandLineParseResult> parsedCommandLineResults)
        {
            if (parsedCommandLineResults == null || parsedCommandLineResults.Count() == 0)
                return null;

            return parsedCommandLineResults.First(p => 
                p.AdditionalParameters != null && p.AdditionalParameters.Length > 0)
                .AdditionalParameters.FirstOrDefault();
        }
    }
}