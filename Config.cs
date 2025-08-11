using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Automation.Peers;
using System.Configuration;
using System.Threading;
using System.Windows.Threading;
using System.Reflection;

namespace TypeB
{
    static internal  class Config
    {
        static public Dictionary<string, string> dicts = new Dictionary<string, string>();
        static public string Path = "config.txt";

        static Config()
        {
            for (int i = 0; i + 1 < ConfigList.Length; i += 2)
            {
                dicts[ConfigList[i]] = ConfigList[i + 1];
            }

            ReadConfig();
        }

        static private string[] ConfigList = {
            "窗口高度", "720",
            "窗口宽度", "1000",
            "字体大小", "30",
            "窗体背景色", "505050",
            "窗体字体色", "D3D3D3",
            "跟打区背景色", "C5B28F",
            "跟打区字体色", "000000",
            "打对色", "95b0e3",
            "打错色", "FF6347",
            "显示进度条", "是",
            //"长流用户名", "",
            //"长流密码", "",
            "极速用户名", "",
            "极速密码", "",
            "禁止F3重打", "否",
            //"增强键准提示", "否",
            //"增强速度提示", "否",
            "速度跟随提示", "否",
            "盲打模式", "否",
            "看打模式", "否",
            "字体", "#霞鹜文楷 GB 屏幕阅读版",
            "行距", "0.35",
            "允许滚动", "是",
            //"回放功能", "否",
            "自动发送成绩", "是",
            "鼠标中键载文", "否",
            "错字重打", "否",
            "错字重复次数", "3",
            "QQ窗口切换模式(1-2)", "1",
            //"字集过滤与替换", "否",
            "载文模式(1-4)", "1",
            "成绩面板展开", "是",
            "成绩签名", "Pain打器",
            "成绩单屏蔽模块(逗号分隔多个)", "无",
            "开启程序调试Log", "否",
            "获取更新", "QQ群775237860"
        };



        static public void SetDefault(params string[] args) 
        { 
            for (int i = 0; i + 1 < args.Length; i+=2)
            {
                dicts[args[i]] = args[i+1];
            }

        }


        static private Timer WriteTimer = null;


        static private void WriteNow(object obj)
        {

            if (Path == "")
                return;

            try
            {
                StreamWriter sw = new StreamWriter(Path);

                foreach (var c in dicts)
                {
                    sw.WriteLineAsync(c.Key + "\t" + c.Value);
                }


                sw.Close();
                if (WriteTimer != null)
                {
                    WriteTimer.Dispose();
                    WriteTimer = null;
                }
            }
            catch (Exception)
            {

                
            }
            finally
            {

            }




        }
    
        static public void WriteConfig (int Delay = 0)
        {

            if (Path == "")
                return;

            try
            {
                if (Delay == 0)
                {
                    if (WriteTimer != null)
                    {
                        WriteTimer.Dispose();
                        WriteTimer = null;
                    }

                    StreamWriter sw = new StreamWriter(Path);

                    foreach (var c in dicts)
                    {
                        sw.WriteLineAsync(c.Key + "\t" + c.Value);
                    }


                    sw.Close();
                }
                else if (Delay > 0)
                {
                    if (WriteTimer == null)
                    {
                        WriteTimer = new Timer(WriteNow, null, Delay, Timeout.Infinite);
                    }
                    else
                    {
                        WriteTimer.Dispose();
                        WriteTimer = new Timer(WriteNow, null, Delay, Timeout.Infinite);
                        //    WriteTimer.Change(Delay, Timeout.Infinite);

                    }
                }
            }
            catch (Exception)
            {

                
            }
            finally { }
           

        }

        static public void ReadConfig ()
        {
            //     char[] sp = { '\r', ' ', '\t' };

            if (!File.Exists(Path))
            { 
                WriteConfig();
                return;
            }

            char[] sp1 = { '\n' };

            string[] lines = File.ReadAllText(Path).Split(sp1, StringSplitOptions.RemoveEmptyEntries);


            foreach (string line in lines)
            {
                if (line.Substring(0, 1) == "#")
                    continue;
                string line_p = line.Replace("\r", "").Replace("\n", "");

                string[] sp = { "\t", " ", "," };

                

                foreach (string s in sp)
                {
                    if (line_p.Contains(s))
                    {
                        int pos = line_p.IndexOf(s);
                        if (pos >= 1 && pos <= line_p.Length - 2)
                        {
                            string key = line_p.Substring(0, pos);
                            string value = line_p.Substring(pos + 1);
                            if (dicts.ContainsKey(key))
                            {
                                dicts[key] = value;
                            }

                            break;
                        }
                    }
                }



            }


            WriteConfig();



        }

        static public bool GetBool (string key)
        {
            if (dicts.ContainsKey(key) && dicts[key] == "是")
                return true;
            else
                return false;
        }
        static public string GetString(string key)
        {
            if (dicts.ContainsKey(key))
                return dicts[key];
            else
                return "";
        }

        static public int GetInt(string key)
        {
            if (dicts.ContainsKey(key) && Int32.TryParse(dicts[key], out  int num))
                return num;
            else
                return 0;
        }


        static public double GetDouble(string key)
        {
            if (dicts.ContainsKey(key) && Double.TryParse(dicts[key], out double num))
                return num;
            else
                return 0;
        }

        static public void Set (string key, bool value)
        {
            if (value)
                dicts[key] = "是";
            else
                dicts[key] = "否";

            WriteConfig(3000);
        }
        static public void Set(string key, int value)
        {
            dicts[key] = value.ToString() ;
            WriteConfig(3000);
        }

        static public void Set(string key, string value)
        {
            dicts[key] = value;
            WriteConfig(3000);
        }

        static public void Set(string key, double value, int fraction = -1)
        {
            string f = "F" + fraction.ToString();
            if (fraction > 0)
                dicts[key] = value.ToString(f);
            else
                dicts[key] = value.ToString();

            WriteConfig(3000);

        }
    }



}
