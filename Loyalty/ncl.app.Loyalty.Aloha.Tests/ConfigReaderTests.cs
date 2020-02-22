using Newtonsoft.Json;
using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace ncl.app.Loyalty.Aloha.Tests
{
    [TestFixture]
    public class ConfigReaderTests
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void EnsureReadingFileWorks()
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configFilePath = Path.Combine(location, "appsettings.json");

            var config = File.ReadAllText(Path.Combine(location, "appsettings.json"));
            var configuration = JsonConvert.DeserializeObject<ConfigHolder>(config);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(configuration.AppSettings.ApiKey));
            Assert.IsTrue(!string.IsNullOrWhiteSpace(configuration.AppSettings.CardLogPath));
            Assert.IsTrue(!string.IsNullOrWhiteSpace(configuration.AppSettings.EndpointBaseAddress));
            Assert.IsTrue(!string.IsNullOrWhiteSpace(configuration.AppSettings.EndpointMethod));
            Assert.IsTrue(!string.IsNullOrWhiteSpace(configuration.AppSettings.RetryListPath));
            Assert.IsTrue(!string.IsNullOrWhiteSpace(configuration.AppSettings.SrDeboutLogFilePath));
        }
    }
}
