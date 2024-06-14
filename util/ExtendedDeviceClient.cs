using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.DeviceCommands;
using AdvancedSharpAdbClient.DeviceCommands.Models;
using AdvancedSharpAdbClient.Models;

namespace PokeHelper.util
{
    
    public record ExtendedDeviceClient : DeviceClient
    {
        public ExtendedDeviceClient(IAdbClient adbClient, DeviceData device) 
            : base(adbClient, device)
        {
        }


        /// <summary>
        /// Gets multiple elements by their respective xpaths.
        /// </summary>
        /// <param name="xpaths">The list of xpaths of the elements to find.</param>
        /// <param name="timeout">The timeout for waiting the elements. Only check once if <see langword="default"/> or <see cref="TimeSpan.Zero"/>.</param>
        /// <returns>A dictionary with the xpath as key and the found <see cref="Element"/> as value.</returns>
        public Dictionary<string, Element> FindElements(List<string> xpaths, TimeSpan timeout = default)
        {
            //if (timeout == default) timeout = TimeSpan.FromSeconds(10);
            Stopwatch stopwatch = new();
            stopwatch.Start();

            var foundElements = new Dictionary<string, Element>();

            do
            {
                try
                {
                    var doc = DumpScreen();
                    //Console.WriteLine(doc.InnerXml);
                    if (doc != null)
                    {
                        foreach (var xpath in xpaths)
                        {
                            
                            var xmlNode = doc.SelectSingleNode(xpath);
                            if (xmlNode == null) continue;
                            //Console.WriteLine($"Got node: {xmlNode.OuterXml}");
                            var element = Element.FromXmlNode(AdbClient, Device, xmlNode);
                            if (element != null) foundElements[xpath] = element;

                        }
                        
                        if (foundElements.Values.All(element => element != null)) break;
                    }
                }
                catch (XmlException ex)
                {
                    Console.WriteLine($"XML Exception: {ex.Message}");
                }

                if (timeout == default) { break; }
            } 
            while (stopwatch.Elapsed < timeout);

            return foundElements;
        }
    }
}
