using ncl.app.Loyalty.Aloha.Relay.Model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace ncl.app.Loyalty.Aloha.COMIntegration
{
    static class SrDebout
    {
        public enum DeboutLevel { LogAlways = 0, LogSevere, LogWarning, LogInfo };

        //LogAlways     messages that we always show
        //LogSevere     bad exceptions
        //LogWarning    expected exceptions
        //LogInfo       flow trace, parameter values

        static string m_File;
        static StreamWriter m_Debout;
        public static DeboutLevel m_Level;
        //static Mutex m_mutex = new Mutex();

        static SrDebout()
        {
            m_Level = DeboutLevel.LogWarning;
        }

        public static void CloseDebout()
        {
            m_Debout.Close();
        }

        public static long DeboutSize()
        {
            if (File.Exists(m_File))
            {
                FileInfo fileInfo = new FileInfo(m_File);
                return fileInfo.Length;
            }
            return 0;
        }

        public static void DeleteDebout()
        {
            if (File.Exists(m_File))
            {
                File.Delete(m_File);
            }
        }

        public static void WriteDebout(String s, DeboutLevel level)
        {
            try
            {
                if (level <= m_Level)
                {
                    String outMsg = String.Format("{0:dd/MM/yy HH:mm:ss.fff} {1}: ", DateTime.Now, level.ToString()) + s;
                    InitializeStreamWriter();
                    m_Debout.WriteLine(outMsg);
                    m_Debout.Flush();
                    m_Debout.Close();
                }
            }
            catch
            { }
        }

        public static void InitializeStreamWriter()
        {
            try
            {
                var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string configFilePath = Path.Combine(location, "appsettings.json");

                var config = File.ReadAllText(Path.Combine(location, "appsettings.json"));
                var configuration  = JsonConvert.DeserializeObject<Configuration>(config);
                string path = configuration.AppSettings.SrDeboutLogFilePath;
                
                //path = @"c:\bootdrv\aloha\tmp\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                m_File = path + @"debout.Intercept-Log.log";
                m_Debout = new StreamWriter(m_File, true);
            }
            catch
            { }
        }
    }
}