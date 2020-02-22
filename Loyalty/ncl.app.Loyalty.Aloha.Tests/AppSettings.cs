using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncl.app.Loyalty.Aloha.Tests
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
