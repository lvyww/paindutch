using System;
using System.IO;

namespace TypeB
{

    static internal class DebugLog
    {
        static public string Path = "log.txt";
        static public bool Enable = false;

        static public void AppendLine(string text)
        {
            if (Enable)
            {
                string t = "[" + DateTime.Now.ToString("g") + "]: " + text + "\n";
                File.AppendAllText(Path, t);
            }


        }

        static public void AppendSection(string name, string text)
        {
            if (Enable)
            {
                string split = "\n**************************\n";
                string t = "[" + DateTime.Now.ToString("g") + "]: " + name + split + text + split + "\n";
                File.AppendAllText(Path, t);
            }

        }

        static public void a(string text)
        {
            if (Enable)
            {
                AppendLine(text);
            }
        }
    }

}
