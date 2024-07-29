using LibB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;


namespace TypeB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 




    public partial class WinTrainer : Window
    {
        public const string Folder = "练单器/";
        public static WinTrainer Current
        {
            get
            {
                foreach (var s in App.Current.Windows)
                {
                    if (s is WinTrainer)
                    {
                        return (WinTrainer)s;

                    }

                }

                return null;
            }

        }

     //   Dictionary<string, int> log = new Dictionary<string, int>();

        Dictionary<string, string> cfg = new Dictionary<string, string>
        {

            {"换段击键", "6" },
             {"每轮降击","0.05" },
            {"每组字数", "10" },




             {"上次打开的文件", "" },
             {"上次的段数", "0" },


        };

        bool CfgInit;
        bool SliderInit;

   //     List<string> InputWords = new List<string>();
        bool Jumped = false;

       
        string mode = "fixed";
        public static double TargetHit = 0;



        List<List<string>> DisplayRoot = new List<List<string>>();


        int TotalGroup;


      

        int MaxGroupSize;
        int RetypeCount = 0;
        double MaxHitRate = 0;
        double AverageGroupSize;


        string TxtFile;















        double ftsize = 24;
        private void ShowWords()
        {
            // var sList = DisplayInfo; 


            fld.FontSize = ftsize;
            fld.Text = string.Join("", DisplayRoot[Convert.ToInt32(cfg["上次的段数"])]);
            fld.FontFamily = MainWindow.Current.GetCurrentFontFamily();
            fld.Background = MainWindow.Current.BdDisplay.Background;
            fld.Foreground = Colors.DisplayForeground;


    



        }

   


        private void InitSlider()
        {
            SliderInit = false;
            sld.Minimum = 1;
            sld.Maximum = TotalGroup;
            sld.Value = Convert.ToInt32(cfg["上次的段数"]) + 1;



            SliderInit = true;
        }

        private void ReadTxt() //从文件重新读取码表
        {
            TxtFile = FileSelector.SelectedItem.ToString();
            string filename = Folder + TxtFile + ".txt";
            if (CfgInit)
            {
                if (File.Exists(filename))
                {
                    cfg["上次打开的文件"] = TxtFile + ".txt";
                    WriteCfg();
                }
            }

            string mbtxt = File.ReadAllText(filename).Trim().Replace("\r", "");//.Replace(" ", "\t");
            do
            {
                mbtxt = mbtxt.Replace("\n\n", "\n");
            } while (mbtxt.Contains("\n\n"));

            do
            {
                mbtxt = mbtxt.Replace("  ", " ");
            } while (mbtxt.Contains("  "));

            string[] lines = mbtxt.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            //         List<word> TrainTable = new();



            DisplayRoot.Clear();

          

            int MaxLineLen = (from line in lines select line.Length).Max();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            if (!chars.Contains( lines[0].Substring(0,1)) && MaxLineLen> 4) //变长
            {
                mode = "varible";

                int group = 0;
                foreach (string line in lines)
                {
                    DisplayRoot.Add(new List<string>());

                    StringInfo si = new StringInfo(line);

                    for (int i = 0; i < si.LengthInTextElements; i++)
                    {
                        string name = si.SubstringByTextElements(i, 1);
                        DisplayRoot[group].Add(name);

                    }

                    group++;
                }

                TotalGroup = group;


                MaxGroupSize = 0;
                AverageGroupSize = 0;
                foreach (var g in DisplayRoot)
                {
                    AverageGroupSize += g.Count;
                    if (MaxGroupSize < g.Count)
                        MaxGroupSize = g.Count;
                }
                AverageGroupSize /= TotalGroup;

            }
            else
            {
                List<String> RootList = new List<String>();
                mode = "fixed";




                foreach (string line in lines)
                {
                    if (line.Length >= 1)
                        RootList.Add(line);

                }


                TotalGroup = (RootList.Count + Convert.ToInt32(cfg["每组字数"]) - 1) / Convert.ToInt32(cfg["每组字数"]);



                MaxGroupSize = Convert.ToInt32(cfg["每组字数"]);

                int k = 0;

                for (int i = 0; i < TotalGroup; i++)
                {
                    DisplayRoot.Add(new List<string>());

                    int jmax;
                    if (i < TotalGroup - 1)
                    {
                        jmax = Convert.ToInt32(cfg["每组字数"]);
                    }
                    else
                    {
                        jmax = RootList.Count - Convert.ToInt32(cfg["每组字数"]) * (TotalGroup - 1);
                    }
                    for (int j = 0; j < jmax; j++)
                    {
                        DisplayRoot[i].Add(RootList[k]);

                        k++;
                    }
                }
            }



            JumpGroup();

            RetypeCount = 0;
            MaxHitRate = 0;
            InitSlider();

            InitGroup();


        }

        private void ReadTxt_old() //从文件重新读取码表
        {
            TxtFile = FileSelector.SelectedItem.ToString();
            string filename = Folder  + TxtFile + ".txt";
            if (CfgInit)
            {
                if (File.Exists(filename))
                {
                    cfg["上次打开的文件"] = TxtFile + ".txt";
                    WriteCfg();
                }
            }

            string mbtxt = File.ReadAllText(filename).Trim().Replace("\r", "").Replace(" ", "\t");
            do
            {
                mbtxt = mbtxt.Replace("\n\n", "\n");
            } while (mbtxt.Contains("\n\n"));

            do
            {
                mbtxt = mbtxt.Replace("\t\t", "\t");
            } while (mbtxt.Contains("\t\t"));

            string[] lines = mbtxt.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            //         List<word> TrainTable = new();



            DisplayRoot.Clear();


            if (!lines[0].Contains("\t") && lines[0].Length >= 4)
            {
                mode = "varible";

                int group = 0;
                foreach (string line in lines)
                {
                    DisplayRoot.Add(new List<string>());

                    StringInfo si = new StringInfo(line);

                    for (int i = 0; i < si.LengthInTextElements; i++)
                    {
                        string name = si.SubstringByTextElements(i, 1);
                        DisplayRoot[group].Add(name);

                    }

                    group++;
                }

                TotalGroup = group;


                MaxGroupSize = 0;
                AverageGroupSize = 0;
                foreach (var g in DisplayRoot)
                {
                    AverageGroupSize += g.Count;
                    if (MaxGroupSize < g.Count)
                        MaxGroupSize = g.Count;
                }
                AverageGroupSize /= TotalGroup;

            }
            else
            {
                List<String> RootList = new List<String>();
                mode = "fixed";


                

                foreach (string line in lines)
                {
                    if (line.Length >= 1)
                        RootList.Add(line);

                }


                TotalGroup = (RootList.Count + Convert.ToInt32(cfg["每组字数"]) - 1) / Convert.ToInt32(cfg["每组字数"]);



                MaxGroupSize = Convert.ToInt32(cfg["每组字数"]);

                int k = 0;

                for (int i = 0; i < TotalGroup; i++)
                {
                    DisplayRoot.Add(new List<string>());

                    int jmax;
                    if (i < TotalGroup - 1)
                    {
                        jmax = Convert.ToInt32(cfg["每组字数"]);
                    }
                    else
                    {
                        jmax = RootList.Count - Convert.ToInt32(cfg["每组字数"]) * (TotalGroup - 1);
                    }
                    for (int j = 0; j < jmax; j++)
                    {
                        DisplayRoot[i].Add(RootList[k]);

                        k++;
                    }
                }
            }



            JumpGroup();

            RetypeCount = 0;
            MaxHitRate = 0;
            InitSlider();

            InitGroup();


        }

        public static IEnumerable<T> Randomize<T>(IEnumerable<T> source)
        {
            Random rnd = new Random();
            return source.OrderBy((item) => rnd.Next());
        }
        private void InGroupRand() // 组内重排
        {
            DisplayRoot[Convert.ToInt32(cfg["上次的段数"])] = Randomize(DisplayRoot[Convert.ToInt32(cfg["上次的段数"])]).ToList() ;  

        }









        private int CalWordCount()
        {
            int sum = 0;
            foreach (var item in DisplayRoot[Convert.ToInt32(cfg["上次的段数"])])
            {
                sum += new StringInfo(item).LengthInTextElements;
            }

            return sum;
        }



        public void GetNextRound(double accuracy, double hitrate, int wrong, string result)
        {
            if (accuracy >= 0.9999 && hitrate >= TargetHit && wrong == 0)
            {
              

                string t =  "击键 " + hitrate.ToString("F2") + "/" + TargetHit.ToString("0.00");
                AutoNextGroup();
                string matchText = GetMatchText();
                MainWindow.Current.LoadText(matchText, RetypeType.first, TxtSource.trainer, false, true);

                MainWindow.Current.UpdateTopStatusText(t);

                QQHelper.SendQQMessageD(MainWindow.Current.QQGroupName, result, matchText, 150, MainWindow.Current);


            }
            else
            {
                string t = "击键 " + hitrate.ToString("F2") + "/" + TargetHit.ToString("0.00");
                RetypeGroup(true,true);
                MainWindow.Current.LoadText(GetMatchText(), RetypeType.retype, TxtSource.trainer, false, true);
                MainWindow.Current.UpdateTopStatusText(t);
                if (hitrate >= MaxHitRate && accuracy >= 0.9999 && wrong == 0)
                {
                    QQHelper.SendQQMessage(MainWindow.Current.QQGroupName, result, 150, MainWindow.Current);
                    MaxHitRate = hitrate;

                }
         //       if (RetypeCount + 1 >= 5 && (RetypeCount + 1) % 5 == 0) //重打5次发一下成绩

            }
        }

        public void F3()
        {
            
      //      RetypeGroup(false, false);
            MainWindow.Current.LoadText(GetMatchText(), RetypeType.retype, TxtSource.trainer, false, true);
            MainWindow.Current.UpdateTopStatusText("重打");
        }

        public void CtrlL()
        {

            RetypeGroup(true, false);
            MainWindow.Current.LoadText(GetMatchText(), RetypeType.retype, TxtSource.trainer, false, true);
            MainWindow.Current.UpdateTopStatusText("乱序");
        }

        private void DisplayHit()
        {

            TBHitrate.Text = "换段击键 " + TargetHit.ToString("0.00");

        }

        private void DisplayHit(double hitrate)
        {

            TBHitrate.Text = "击键 "+ hitrate.ToString("F2") + "/" + TargetHit.ToString("0.00");

        }

        private void UpdateFileList()
        {
            DirectoryInfo folder = new DirectoryInfo(Folder);



            foreach (FileInfo file in folder.GetFiles("*.txt"))
                FileSelector.Items.Add(file.Name.Substring(0, file.Name.Length - 4));
            FileSelector.SelectedIndex = 0;
        }

        private void RetypeGroup(bool rand, bool count) //重打本组
        {
            if (count)
                RetypeCount++;

            if (rand)
                InGroupRand();

            ShowWords();
            

            WriteCfg();

            TargetHit = Convert.ToDouble(cfg["换段击键"]) - Convert.ToDouble(cfg["每轮降击"]) * (RetypeCount);
            if (mode == "varible")
                TargetHit = Math.Round((float)(TargetHit * Math.Pow(AverageGroupSize / (double)DisplayRoot[Convert.ToInt32(cfg["上次的段数"])].Count, 0.35)), 2);

            if (TargetHit < 0.01)
                TargetHit = 0.01;

            DisplayHit();

            stattxt.Text = "第 " + (Convert.ToInt32(cfg["上次的段数"]) + 1) + "/" + TotalGroup + " 段";
        }
        private void InitGroup() //初始化组
        {
            RetypeCount = 0;
            MaxHitRate = 0;


            InGroupRand();
            ShowWords();
            LoadText();
  
            WriteCfg();

            TargetHit = Convert.ToDouble(cfg["换段击键"]) - Convert.ToDouble(cfg["每轮降击"]) * (RetypeCount);
            if (mode == "varible")
                TargetHit = Math.Round((float)(TargetHit * Math.Pow(AverageGroupSize / (double)DisplayRoot[Convert.ToInt32(cfg["上次的段数"])].Count, 0.35)), 2);

            if (TargetHit < 0.01)
                TargetHit = 0.01;
   


                DisplayHit();
            
            stattxt.Text = "第 " + (Convert.ToInt32(cfg["上次的段数"]) + 1) + "/" + TotalGroup + " 段";
       

        }



        public void AutoNextGroup()
        {

            cfg["上次的段数"] = (Convert.ToInt32(cfg["上次的段数"]) + 1).ToString();
            if (Convert.ToInt32(cfg["上次的段数"]) == TotalGroup)
                cfg["上次的段数"] = "0";
            sld.Value = Convert.ToInt32(cfg["上次的段数"]) + 1;


            InitGroup();


     

        }
        
        string GetMatchText ()
        {
            StringBuilder sb = new StringBuilder();
            string name = FileSelector.SelectedItem.ToString() + " " + "目标" + Convert.ToDouble(cfg["换段击键"]).ToString("F2");

            if (Convert.ToDouble(cfg["每轮降击"]) > 0.000001)
                name += "-" + Convert.ToDouble(cfg["每轮降击"]).ToString("F2");
            sb.Append(name);
            sb.AppendLine();
            string txt = string.Join("", DisplayRoot[Convert.ToInt32(cfg["上次的段数"])]); 
            sb.Append(txt);
            sb.AppendLine();
            sb.Append("-----第");
            sb.Append(Convert.ToInt32(cfg["上次的段数"]) + 1);
            sb.Append("段");


            sb.Append("-");

            sb.Append(" 共");
            sb.Append(TotalGroup);
            sb.Append("段 ");

            /*
            sb.Append(" 进度 ");
            sb.Append((Index - 1) * SectionSize);
            sb.Append("/");
            sb.Append(display);
            sb.Append("字 ");
*/
            sb.Append(" 本段");
            sb.Append(new StringInfo(txt).LengthInTextElements);
            sb.Append("字 ");

            sb.Append("Pain练单器");
            return sb.ToString();
        }

        private void JumpGroup()
        {

            if (Jumped)
            {
                cfg["上次的段数"] = "0";
                return;
            }

            else
                Jumped = true;



            if (Convert.ToInt32(cfg["上次的段数"]) > 0 && Convert.ToInt32(cfg["上次的段数"]) < TotalGroup)
            {

                sld.Value = Convert.ToInt32(cfg["上次的段数"]) + 1;
    
                InitGroup();

                return;
            }
            else
            {
                cfg["上次的段数"] = "0";
                return;
            }

        }

        public WinTrainer()
        {

            InitializeComponent();

            UpdateFileList();
            InitCfg();


            ReadTxt();
            ShowWords();
            LoadText();



        }

        private void FileSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CfgInit)
                ReadTxt();

        }


     


        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CfgInit && SliderInit)
            {
                cfg["上次的段数"] = (Convert.ToInt32(sld.Value) - 1).ToString();
    
                InitGroup();
            }
        }





        private void RandAllClick(object sender, RoutedEventArgs e)
        {
            RandAllGroup();
        }


        private void RandAllGroup()
        {

            if (mode == "fixed")
            {
                List<string> RootList = new List<string>();

                foreach (var ss in DisplayRoot)
                {
                    foreach (string s in ss)
                    {
                        RootList.Add(s);
                    }
                }

                int count = RootList.Count;


                int[] arr = new int[count];

                for (int j = 0; j < count; j++)
                {
                    arr[j] = j;
                }

                int[] arr2 = new int[count];


                Random rand = new Random();

                for (int j = 0; j < count; j++)
                {
                    int rd_rng = count - j;
                    int r = rand.Next(rd_rng);
                    arr2[j] = arr[r];
                    arr[r] = arr[rd_rng - 1];

                }





                string[] tmpstr = new string[count];

                for (int j = 0; j < count; j++)
                {
                    tmpstr[j] = RootList[arr2[j]];
                }

                for (int j = 0; j < count; j++)
                {
                    RootList[j] = tmpstr[j];
                }



                int k = 0;
                DisplayRoot.Clear();

                for (int i = 0; i < TotalGroup; i++)
                {
                    DisplayRoot.Add(new List<string>());

                    int jmax;
                    if (i < TotalGroup - 1)
                    {
                        jmax = Convert.ToInt32(cfg["每组字数"]);
                    }
                    else
                    {
                        jmax = count - Convert.ToInt32(cfg["每组字数"]) * (TotalGroup - 1);
                    }
                    for (int j = 0; j < jmax; j++)
                    {
                        DisplayRoot[i].Add(RootList[k]);

                        k++;
                    }
                }
                InitGroup();
                InitSlider();


            }
            else if (mode == "varible")
            {


                int count = DisplayRoot.Count;


                int[] arr = new int[count];

                for (int j = 0; j < count; j++)
                {
                    arr[j] = j;
                }

                int[] arr2 = new int[count];


                Random rand = new Random();

                for (int j = 0; j < count; j++)
                {
                    int rd_rng = count - j;
                    int r = rand.Next(rd_rng);
                    arr2[j] = arr[r];
                    arr[r] = arr[rd_rng - 1];

                }



                List<List<string>> tmpstr = new List<List<string>>();
                for (int i = 0; i < count; i++)
                {
                    tmpstr.Add(new List<string>());
                }


                for (int j = 0; j < count; j++)
                {
                    tmpstr[j] = DisplayRoot[arr2[j]];
                }

                for (int j = 0; j < count; j++)
                {
                    DisplayRoot[j] = tmpstr[j];
                }



                InitGroup();

                InitSlider();



            }


        }

        private void norm_Click(object sender, RoutedEventArgs e)
        {
            ReadTxt();
        }







        private void speed_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CfgInit)
            {
                cfg["换段击键"] = speed.Text;
                if (DisplayRoot != null)
                {
                    InitGroup();

                }
                WriteCfg();
            }
        }








        private void InitCfg()
        {
            char[] s2 = { '\t', '\r', '\n' };
            if (File.Exists("TrainerConfig.txt"))
            {
                StreamReader sr = new StreamReader("TrainerConfig.txt");
                string[] lines = sr.ReadToEnd().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    string[] ls = line.Split(s2, StringSplitOptions.RemoveEmptyEntries);

                    if (ls.Length < 2)
                        continue;

                    cfg[ls[0]] = ls[1];
                }
                sr.Close();

            }
            else
            {
                WriteCfg();
            }


            for (int i = 0; i < FileSelector.Items.Count; i++)
            {
                if (cfg["上次打开的文件"] == FileSelector.Items[i].ToString() + ".txt")
                {
                    FileSelector.SelectedIndex = i;
                }
            }




         

            speed.Text = cfg["换段击键"];
            TextNum.Text = cfg["每组字数"];
            TextHitDecrease.Text = cfg["每轮降击"];

       






            this.Top = MainWindow.Current.Top;
            this.Left = MainWindow.Current.Left - this.Width;

            CfgInit = true;

        }





        private void WriteCfg()
        {

            cfg["删除此文件即可重置设置"] = "获取更新加Q群：21134461";

            try
            {
                StreamWriter sr = new StreamWriter("TrainerConfig.txt");
                foreach (var item in cfg)
                {
                    sr.WriteLine(item.Key + "\t" + item.Value);
                }
                sr.Flush();
                sr.Close();
            }
            catch (Exception)
            {

                
            }

        }


 




        private void TextNum_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (CfgInit)
            {
                if (int.TryParse(TextNum.Text, out int tmp2))
                {

                    cfg["每组字数"] = tmp2.ToString();
                    if (DisplayRoot != null)
                    {

                        ReadTxt();
                        ShowWords();
                        LoadText();
                    }
                    WriteCfg();
                }
                else
                {

                    TextNum.Text = cfg["每组字数"];
                }


            }

        }

        private void TextHitDecrease_TextChanged(object sender, TextChangedEventArgs e)
        {
            double tmp2;
            if (CfgInit)
            {
                if (double.TryParse(TextHitDecrease.Text, out tmp2))
                {

                    cfg["每轮降击"] = tmp2.ToString();
                    if (DisplayRoot != null)
                    {
                        InitGroup();


                    }
                    WriteCfg();
                }
                else
                {

                    TextHitDecrease.Text = cfg["每轮降击"];
                }


            }
        }


        private int GetCharCount(List<StringInfo> siList)
        {
            var lens = from si in siList select si.LengthInTextElements;

            return lens.Sum();


        }

        private int GetCharCount(string s)
        {
            return new StringInfo(s).LengthInTextElements;
        }



        private void LoadText()
        {
            
            MainWindow.Current.LoadText(GetMatchText(), RetypeType.first, TxtSource.trainer, false,true);
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Current.LoadText(GetMatchText(), RetypeType.first, TxtSource.trainer, false, true);
            QQHelper.SendQQMessage(MainWindow.Current.QQGroupName, GetMatchText(), 150, MainWindow.Current);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
 
            if (TextInfo.Exit)
                e.Cancel = false;
            else
            {
                e.Cancel = true;//取消这次关闭事件
                Hide();//隐藏窗口，以便下次调用show
            }


            
        }
    }
}

