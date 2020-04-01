using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Digitizeit.Quartz.HostedService.Helpers
{
    public static class EmbeddedResourceHelper
    {
        public static string GetTextResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(name));

            string resourceText;

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream ?? throw new InvalidOperationException()))
            {
                resourceText = reader.ReadToEnd();
            }

            return resourceText;
        }
    }
}