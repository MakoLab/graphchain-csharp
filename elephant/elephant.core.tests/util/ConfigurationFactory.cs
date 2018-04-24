using System.IO;
using Microsoft.Extensions.Configuration;

namespace elephant.core.tests.util
{
    public static class ConfigurationFactory
    {
        public static IConfigurationRoot CreateConfigurationRoot()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            return builder.Build();
        }
    }
}
