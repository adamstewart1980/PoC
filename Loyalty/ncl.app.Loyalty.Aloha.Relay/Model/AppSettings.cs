using System;
using System.Text;
using ncl.app.Loyalty.Aloha.Relay.Helpers;

namespace ncl.app.Loyalty.Aloha.Relay.Model
{
    public class AppSettings
    {
        public string EndpointBaseAddress { get; set; }
        public string EndpointMethod { get; set; }
        public string ApiKey { get; set; }
        /// <summary>
        /// Dont use this directly as it may need formatting. Go via FormattedCardLogPath
        /// </summary>
        public string CardLogPath { get; set; } 
        public string RetryListPath { get; set; }
        public string SrDeboutLogFilePath { get; set; }

        public string FormattedCardLogPath 
        {
            get
            {
                if (this.CardLogPath.IndexOf(@"{0}") >= 0)
                {
                    return string.Format(this.CardLogPath, DateTime.Now.StartOfWeek(DayOfWeek.Monday).ToString("yyyy-MM-dd"));
                }

                return this.CardLogPath;
            }
        }

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
