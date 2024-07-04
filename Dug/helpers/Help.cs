using DNS_Checker.resources;
using System.Resources;
using System.Text;
using System.Reflection;

namespace DNS_Checker.helpers
{
    public class Help
    {
        // Setup the Resouce access for the Help Info
        ResourceManager _resourceManager = new ResourceManager(typeof(Resources));

        public void PrintHelpText()
        {
            var helpInfoResource = _resourceManager.GetObject("HelpInfo");
            if (helpInfoResource != null)
            {
                string helpText = (string)helpInfoResource;
                Console.WriteLine(helpText);
            }
        }
    }
}