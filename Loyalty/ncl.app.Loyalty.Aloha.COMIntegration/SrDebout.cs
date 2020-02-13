using System;
using System.IO;

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
        static public DeboutLevel m_Level;
        //static Mutex m_mutex = new Mutex();

        static SrDebout()
        {
            m_Level = DeboutLevel.LogWarning;
        }

        static public void CloseDebout()
        {
            m_Debout.Close();
        }

        static public long DeboutSize()
        {
            if (File.Exists(m_File))
            {
                FileInfo fileInfo = new FileInfo(m_File);
                return fileInfo.Length;
            }
            return 0;
        }

        static public void DeleteDebout()
        {
            if (File.Exists(m_File))
            {
                File.Delete(m_File);
            }
        }

        static public void WriteDebout(String s, DeboutLevel level)
        {
            try
            {
                if (level <= m_Level)
                {
                    String outMsg = String.Format("{0:MM/dd/yy HH:mm:ss.fff} {1}: ", DateTime.Now, level.ToString()) + s;
                    InitializeStreamWriter();
                    m_Debout.WriteLine(outMsg);
                    m_Debout.Flush();
                    m_Debout.Close();
                }
            }
            catch
            { }
        }

        static public void InitializeStreamWriter()
        {
            try
            {
                string path = @"c:\BootDrv\Aloha\tmp\";
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