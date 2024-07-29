using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TypeB
{
    static internal class IntStringDict
    {
        static public Dictionary<int, string> BiaoDing = new Dictionary<int, string>();
        static public Dictionary<int, string> Selection = new Dictionary<int, string>();

        static private Dictionary<int, string> DefautlBiaoDing = new Dictionary<int, string>
        {
            {48, "）)"},
            {49, "！!"},
            {50, "@"},
            {51, "#"},
            {52, "￥$"},
            {53, "%"},
            {54, "……^"},
            {55, "&"},
            {56, "*"},
            {57, "（("},
            {186, "；：;:"},
            {187, "+="},
            {188, "，《,<"},
            {189, "——-_"},
            {190, "。》.>"},
            {191, "？/?、"},
            {192, "·`~"},
            {219, "【[{"},
            {220, "、\\|"},
            {221, "】]}"},
            {222, "“”‘’\"'"},
        };
        static private Dictionary<int, string> DefautlSelection = new Dictionary<int, string>
        {
            {48, "）)0"},
            {49, "！!"},
            {50, "@2"},
            {51, "#3"},
            {52, "￥$4"},
            {53, "%5"},
            {54, "……^6"},
            {55, "&7"},
            {56, "*8"},
            {57, "（(9"},
            {186, "；：;:"},
            {187, "+="},
            {188, "，《,<"},
            {189, "—-_"},
            {190, "。》.>"},
            {191, "？/?、"},
            {192, "·`~"},
            {219, "【[{"},
            {220, "、\\|"},
            {221, "】]}"},
            {222, "“”‘’\"'"},
        };


        static public void  Load()
        {
            BiaoDing = Read("标顶键符映射.txt", DefautlBiaoDing);
            Selection = Read("选重键符映射.txt", DefautlSelection);

        }

        static private Dictionary<int, string> Read (string path,  Dictionary<int, string> defautDict)
        {

            Dictionary<int, string> outDict = new Dictionary<int, string>();

            if (File.Exists (path))
            {
                string txt = File.ReadAllText (path).Replace ("\r\n", "\n");
                string[]lines = txt.Split (new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in lines)
                {
                    string[] ls = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries); ;
                    if (ls.Length == 1)
                    {
                        Int32.TryParse(ls[0], out int val);
                        if (val > 0)
                            outDict.Add(val, "");

                    }
                    else if (ls.Length >= 2)
                    {
                        Int32.TryParse(ls[0], out int val);
                        if (val > 0)
                            outDict.Add(val, ls[1]);
                    }
                }

            }
            else //不存在文件
            {
                outDict = defautDict;
                StreamWriter sw = new StreamWriter(path);
                foreach(var v in outDict)
                    sw.WriteLine(v.Key + "\t" + v.Value);

                sw.Close();
            }

            return outDict;
        }

        static public Dictionary<int, string> ReadTmp(string path, Dictionary<string, int> defautDict)
        {

            Dictionary<int, string> outDict = new Dictionary<int, string>();


            foreach(var v in defautDict)
            {
                if (outDict.ContainsKey(v.Value))
                    outDict[v.Value] = outDict[v.Value] + v.Key;
                else 
                    outDict.Add(v.Value, v.Key);
                outDict = outDict.OrderBy(o=>o.Key).ToDictionary(o=>o.Key, o => o.Value);
            }
            {
       
                StreamWriter sw = new StreamWriter(path);
                foreach (var v in outDict)
                    sw.WriteLine("{"+v.Key + ", " + "\""+v.Value + "\"},");
                sw.Close();
            }

            return outDict;
        }

    }
}
