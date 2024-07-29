using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TypeB.MainWindow;







namespace TypeB
{

    static internal class RetypeCounter
    {
        static public string Path = "RetypeCounter20230804.txt";
   //     static public string SumKey = "合计";
        static public int HourThresh = 6;
        static public bool Loaded = false;
        static private Dictionary<string, Dictionary<string, int>> Dict = new Dictionary<string, Dictionary<string, int>>();
        static public int[] Buffer = new int[1000];




        /*
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
        */
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
                Dict.Clear();
                Dict[date] = new Dictionary<string, int>();
            }

            if (!Dict[date].ContainsKey(key))
                Dict[date].Add(key, value);
            else
                Dict[date][key] = Dict[date][key] + value;



            Write();

        }


        static public int Get(string key)
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
                return 0;
            else if (!Dict[date].ContainsKey(key))
                return 0;
            else
                return  Dict[date][key];



        }

        static private void Read()
        {
            try
            {
                Loaded = true;
                string folder = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);

                string fullpath = folder + "\\" + Path;
                if (!File.Exists(fullpath))
                    return;

                string txt = File.ReadAllText(fullpath).Replace("\r", "");
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
            catch (Exception)
            {

                Dict.Clear();
                Loaded = true;
            }

        }
        /*
        static public void Write1()
        {

            string folder = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);

            string fullpath = folder + "\\" + Path;

            StreamWriter sw = new StreamWriter(fullpath);






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

*/
        static public void Write()
        {

            try
            {
                string folder = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);

                string fullpath = folder + "\\" + Path;

                StreamWriter sw = new StreamWriter(fullpath);




                int hour = DateTime.Now.Hour;

                string date = "";
                if (hour < HourThresh)
                    date = DateTime.Now.AddDays(-1).ToString("d");
                else
                    date = DateTime.Now.ToString("d");



                foreach (var DayRecord in Dict)
                {
                    if (DayRecord.Key != date)
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
    
        }
    }


}
