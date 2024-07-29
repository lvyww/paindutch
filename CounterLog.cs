using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibB
{
    static internal class CounterLog
    {
        static public string Path = "统计.txt";
        static public string SumKey = "合计";
        static public int HourThresh = 6;
        static public bool Loaded = false;
        static private Dictionary<string, Dictionary<string, int>> Dict = new Dictionary<string, Dictionary<string, int>>();
        static public int[] Buffer = new int[1000];


        public static int GetCurrent(string key)
        {
            if (!Loaded)
                Read();

            int hour = DateTime.Now.Hour;

            string date = "";
            if (hour < HourThresh)
                date = DateTime.Now.AddDays(-1).ToString("d");
            else
                date = DateTime.Now.ToString("d");


            if (!Dict.ContainsKey(date))
            {
                Dict[date] = new Dictionary<string, int>();
            }

            if (!Dict[date].ContainsKey(key))
                Dict[date].Add(key, 0);


            Write();
            return Dict[date][key];
        }

        public static int GetSum(string key)
        {
            if (!Dict.ContainsKey(SumKey))
            {
                Dict[SumKey] = new Dictionary<string, int>();
            }

            if (!Dict[SumKey].ContainsKey(key))
                Dict[SumKey].Add(key, 0);


            return Dict[SumKey][key];
        }
        static public void Add(string key, int value)
        {
            if (!Loaded)
                Read();

            int hour = DateTime.Now.Hour;

            string date = "";
            if (hour < HourThresh)
                date = DateTime.Now.AddDays(-1).ToString("d");
            else
                date = DateTime.Now.ToString("d");


            if (!Dict.ContainsKey(date))
            {
                Dict[date] = new Dictionary<string, int>();
            }

            if (!Dict[date].ContainsKey(key))
                Dict[date].Add(key, value);
            else
                Dict[date][key] = Dict[date][key] + value;

            if (!Dict.ContainsKey(SumKey))
            {
                Dict[SumKey] = new Dictionary<string, int>();
            }

            if (!Dict[SumKey].ContainsKey(key))
                Dict[SumKey].Add(key, value);
            else
                Dict[SumKey][key] = Dict[SumKey][key] + value;

            Write();

        }

        static private void Read()
        {
            Loaded = true;

            if (!File.Exists(Path))
                return;

            string txt = File.ReadAllText(Path).Replace("\r", "");
            string[] lines = txt.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);


            string date = "";
            foreach (string line in lines)
            {
                string[] ls = line.Split(new char[] { '\t', ' ', ',', '，' }, StringSplitOptions.RemoveEmptyEntries);

                if (ls.Length == 1)
                {
                    Dict[ls[0]] = new Dictionary<string, int>();
                    date = ls[0];
                }
                else if (ls.Length >= 2)
                {
                    Int32.TryParse(ls[1], out int value);
                    if (value > 0)
                        Dict[date][ls[0]] = value;
                }
            }

        }

        static public void Write()
        {

            if (!Loaded)
                Read();

            try
            {
                StreamWriter sw = new StreamWriter(Path);


                Dictionary<string, int> sum = new Dictionary<string, int>();


                if (Dict.ContainsKey(SumKey))
                {
                    sw.WriteLine(SumKey);
                    foreach (var Record in Dict[SumKey])
                        sw.WriteLine(Record.Key + "\t" + Record.Value);
                }
                sw.WriteLine();

                foreach (var DayRecord in Dict)
                {
                    if (DayRecord.Key == SumKey)
                        continue;

                    sw.WriteLine(DayRecord.Key);
                    foreach (var Record in DayRecord.Value)
                        sw.WriteLine(Record.Key + "\t" + Record.Value);


                }
                sw.WriteLine();

                sw.Close();
            }
            catch (Exception)
            {

                
            }
            finally
            {

            }

        }
    }

}
