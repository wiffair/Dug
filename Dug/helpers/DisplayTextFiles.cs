// Dug is a DNS lookup tool
// Copyright(C) 2024  Richard Cole
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

using dug.resources;
using System.Resources;

namespace dug.helpers
{
    public class DisplayTextFiles
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
        public void PrintLicenseText()
        {
            var helpInfoResource = _resourceManager.GetObject("LICENSE");
            if (helpInfoResource != null)
            {
                string helpText = (string)helpInfoResource;
                Console.WriteLine(helpText);
            }
        }
    }
}
