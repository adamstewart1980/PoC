using System.Text;

namespace ncl.app.Loyalty.Aloha.Relay.Model
{
    public class Configuration
    {
        public AppSettings AppSettings { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append($"{nameof(AppSettings)}: {AppSettings.ToString()}");

            return builder.ToString();
        }
    }
}
