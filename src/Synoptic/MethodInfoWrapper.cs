using System.Linq;
using System.Reflection;

namespace Synoptic
{
    public class MethodInfoWrapper
    {
        public MethodInfoWrapper(MethodInfo method)
        {
            LinkedToMethod = method;

            Name = method.Name;

            var attributes = method.GetCustomAttributes(typeof(CommandAttribute), true);
            if (attributes.Length > 0)
            {
                var commandParameter = (CommandAttribute)attributes.First();

                Description = Description.GetNewIfValid(commandParameter.Description);
                Name = Name.GetNewIfValid(commandParameter.Name);
            }
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public MethodInfo LinkedToMethod { get; set; }
    }
}