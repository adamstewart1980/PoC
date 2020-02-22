using System.Text;

namespace ncl.app.Loyalty.Aloha.Relay.Model
{
    public class AppSettings
    {
        public string EndpointBaseAddress { get; set; }
        public string EndpointMethod { get; set; }
        public string ApiKey { get; set; }
        public string CardLogPath { get; set; }
        public string RetryListPath { get; set; }
        public string SrDeboutLogFilePath { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append($"{nameof(EndpointBaseAddress)}:{EndpointBaseAddress}, ");
            builder.Append($"{nameof(EndpointMethod)}:{EndpointMethod}, ");
            builder.Append($"{nameof(ApiKey)}:{ApiKey}, ");
            builder.Append($"{nameof(CardLogPath)}:{CardLogPath}, ");
            builder.Append($"{nameof(RetryListPath)}:{RetryListPath}, ");
            builder.Append($"{nameof(SrDeboutLogFilePath)}:{SrDeboutLogFilePath}, ");

            return builder.ToString();
        }
    }
}
