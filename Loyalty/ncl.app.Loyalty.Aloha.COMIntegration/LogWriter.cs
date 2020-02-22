using ncl.app.Loyalty.Aloha.Relay.Interfaces;

namespace ncl.app.Loyalty.Aloha.COMIntegration
{
    public class LogWriter : ILogWriter
    {
        public void WriteLog(string message)
        {
            SrDebout.WriteDebout(message, SrDebout.DeboutLevel.LogAlways);
        }
    }
}