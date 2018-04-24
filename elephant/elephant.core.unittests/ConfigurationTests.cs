using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace elephant.core.unittests
{
    [TestClass]
    public class ConfigurationTests
    {
        private const string _testConfigurationJson = @"{
                                                    ""AllegroGraphConnection"": {
                                                    ""url"": ""testUrl"",
                                                    ""storeID"": ""testID"",
                                                    ""user"": ""testUser"",
                                                    ""pass"": ""testPass""
                                                    }
                                                }";
        private const string _fileName = "test_appsettings.json";

        [TestMethod]
        public void ReadConfigurationFile()
        {
            var basePath = Directory.GetCurrentDirectory();
            File.WriteAllText(Path.Combine(basePath, _fileName), _testConfigurationJson);
            var builder = new ConfigurationBuilder().SetBasePath(basePath).AddJsonFile(_fileName);
            var root = builder.Build();
            Assert.AreEqual("testUrl", root["AllegroGraphConnection:url"]);
            Assert.AreEqual("testID", root["AllegroGraphConnection:storeID"]);
            Assert.AreEqual("testUser", root["AllegroGraphConnection:user"]);
            Assert.AreEqual("testPass", root["AllegroGraphConnection:pass"]);
            File.Delete(Path.Combine(basePath, _fileName));
        }
    }
}
