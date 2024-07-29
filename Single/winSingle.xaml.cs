using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace TrainTiger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    class word
    {
        public string name;
        public string code;
        public long freq;
        public word(string a, string b, long c)
        {
            name = a;
            code = b;
            freq = c;
        }
    }




    public partial class MainWindow : Window
    {

        Dictionary<string, int> log = new Dictionary<string, int>();

        Dictionary<string, string> cfg = new Dictionary<string, string>
        {
            {"练习编码","全码" },
            {"编码提示","关闭" },
            {"自动上屏码长", "4" },
            {"换段击键", "3" },
             {"每轮降击","0" },
            {"每组字数", "10" },
            {"字根字体", "等线" },
             {"编码字体", "consolas" },

            {"字体大小", "1.3" },
             {"设置面板", "展开" },
             {"窗口宽度", "680" },
             {"窗口高度", "400" },
             {"上次打开的文件", "" },
             {"上次的段数", "0" },
             {"重码上屏键", ";'123456789" },
             {"删除此文件即可重置设置", "作者：ID;PeaceB" }
        };

        bool CfgInit;
        bool SliderInit;

        List<string> InputWords = new List<string>();
        bool Jumped = false;

        bool ShowAnswer = false;
        string mode = "fixed";
        double TargetHit = 0;



        List<List<string>> DisplayRoot = new List<List<string>>();

        Dictionary<string, string> MB = new Dictionary<string, string>(); //显示码表
        Dictionary<string, string> RMB = new Dictionary<string, string>(); //校对码表
        int TotalGroup;


        int cur = 0;

        int MaxGroupSize;
        int round = 0;
        double AverageGroupSize;

        double CodeFw = 16.5;
        string TxtFile;

        string[] sep = { " ", "\n", "\r" };

        int stateEnd = 0;
        bool RoundFail = true;
        double grouphit = 0;
        int BackSpace = 0;


        Stopwatch sw = new Stopwatch();
        //        Dictionary<string, string> mb = new Dictionary<string, string>();
        //        Dictionary<string, string> tmb = new Dictionary<string, string>();

        //        Run[] iword = new Run[9999];

        //       Paragraph para = new Paragraph();
        //      Paragraph para2 = new Paragraph();
        Color myArgbColor = new Color();
        SolidColorBrush myBrush;

        List<TextBlock> Tbr = new List<TextBlock>();
        List<TextBlock> Tbc = new List<TextBlock>();
        List<TextBlock> Tbu = new List<TextBlock>();

        static List<word> words = new List<word>();
        static Dictionary<string, List<string>> MatchTable = new Dictionary<string, List<string>>();


        static void ReadBaseMb(string path)
        {
            string mbtxt = File.ReadAllText(path).Trim().Replace("\r", "").Replace(" ", "\t");
            do
            {
                mbtxt = mbtxt.Replace("\n\n", "\n");
            } while (mbtxt.Contains("\n\n"));

            do
            {
                mbtxt = mbtxt.Replace("\t\t", "\t");
            } while (mbtxt.Contains("\t\t"));

            string[] lines = mbtxt.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);


            foreach (string line in lines)
            {
                if (line.Contains("\t"))
                {
                    string[] ls = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    string name = ls[0];
                    string code = ls[1];
                    long freq = 0;
                    if (ls.Length >= 3)
                    {
                        long.TryParse(ls[2], out freq);
                    }

                    words.Add(new word(name, code, freq));
                }
            }


            words = words.OrderByDescending(a => a.freq).ToList();

            Dictionary<string, int> mp = new Dictionary<string, int>();

            foreach (var w in words)
            {
                if (!MatchTable.ContainsKey(w.name))
                    MatchTable.Add(w.name, new List<string>());

                if (!mp.ContainsKey(w.code))
                {
                    mp[w.code] = 1;

                    MatchTable[w.name].Add(w.code);
                }
                else
                {
                    mp[w.code] += 1;
                    if (mp[w.code] == 2)
                    {
                        MatchTable[w.name].Add(w.code + ";");
                    }
                    else if (mp[w.code] == 3)
                    {
                        MatchTable[w.name].Add(w.code + "'");
                    }

                    MatchTable[w.name].Add(w.code + mp[w.code].ToString());

                }
            }




        }

        private void InitTextblocks()
        {
            Tbr.Clear();
            Tbc.Clear();
            Tbu.Clear();
            displays.Children.Clear();
            displays.ColumnDefinitions.Clear();

            ColumnDefinition cd0 = new ColumnDefinition();
            cd0.Width = new GridLength(1, GridUnitType.Star);
            displays.ColumnDefinitions.Add(new ColumnDefinition());


            for (int i = 0; i < MaxGroupSize; i++)
            {
                TextBlock r = new TextBlock();

                TextBlock c = new TextBlock();
                TextBlock u = new TextBlock();
                //                r.FontFamily = new FontFamily("./resources/#TumanPUA");
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(1, GridUnitType.Star);
                displays.ColumnDefinitions.Add(new ColumnDefinition());
                r.SetValue(Grid.RowProperty, 0);
                c.SetValue(Grid.RowProperty, 1);
                u.SetValue(Grid.RowProperty, 2);
                r.SetValue(Grid.ColumnProperty, i + 1);
                c.SetValue(Grid.ColumnProperty, i + 1);
                u.SetValue(Grid.ColumnProperty, i + 1);
                displays.Children.Add(r);
                displays.Children.Add(c);
                displays.Children.Add(u);
                Tbr.Add(r);
                Tbc.Add(c);
                Tbu.Add(u);

            }
            ColumnDefinition cd1 = new ColumnDefinition();
            cd1.Width = new GridLength(1, GridUnitType.Star);
            displays.ColumnDefinitions.Add(new ColumnDefinition());


        }

        private void InitSlider()
        {
            SliderInit = false;
            sld.Minimum = 1;
            sld.Maximum = TotalGroup;
            sld.Value = Convert.ToInt32(cfg["上次的段数"]) + 1;

            fsize.Minimum = 0.2;
            fsize.Maximum = 3;
            fsize.TickFrequency = 0.1;

            SliderInit = true;
        }


        private void ReadTxt() //从文件重新读取码表
        {
            TxtFile = FileSelector.SelectedItem.ToString();
            string filename = @"mb\" + TxtFile + ".txt";
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


            MB.Clear();
            RMB.Clear();
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
                        if (MatchTable.ContainsKey(name))
                        {
                            DisplayRoot[group].Add(name);



                            if (!MB.ContainsKey(name))
                                MB.Add(name, MatchTable[name][0]);

                            foreach (var w in MatchTable[name])
                            {
                                if (!RMB.ContainsKey(w))
                                    RMB.Add(w, name);

                            }
                        }

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

                if (lines[0].Contains("\t"))
                {
                    foreach (string line in lines)
                    {

                        if (line.Length > 0)
                        {
                            string[] l = line.Split(new char[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);

                            if (!MB.ContainsKey(l[0]))
                            {
                                MB.Add(l[0], l[1]);
                                RootList.Add(l[0]);

                            }




                            if (!RMB.ContainsKey(l[1]))
                                RMB.Add(l[1], l[0]);



                        }
                    }
                }
                else
                {

                    foreach (string line in lines)
                    {
                        if (MatchTable.ContainsKey(line))
                        {
                            if (!MB.ContainsKey(line))
                                MB.Add(line, MatchTable[line][0]);

                         
                            RootList.Add(line);

                            foreach (string w in MatchTable[line])
                            {

                                if (!RMB.ContainsKey(w))
                                    RMB.Add(w, line);
                                
                            }
                        }
                    }



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

            round = 0;
            InitSlider();

            InitGroup();


        }
        private void InGroupRand() // 组内重排
        {
            int count = DisplayRoot[Convert.ToInt32(cfg["上次的段数"])].Count;


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
                tmpstr[j] = DisplayRoot[Convert.ToInt32(cfg["上次的段数"])][arr2[j]];
            }

            for (int j = 0; j < count; j++)
            {
                DisplayRoot[Convert.ToInt32(cfg["上次的段数"])][j] = tmpstr[j];
            }




        }

        private void ShowHint(bool yes, int id)
        {
            if (yes == false)
            {
                Tbc[id].Foreground = Brushes.Red;
            }
            else
            {
                Tbc[id].Foreground = Brushes.DarkGray;
            }


        }

        private void RetypeThisGroup()
        {
            RoundFail = false;
            sw.Reset();
            stateEnd = 0;
            BackSpace = 0;

            cur = 0;

            InputWords.Clear();
            foreach (var tb in Tbu)
            {
                tb.Text = "";
                InputWords.Add("");
            }
            InputBox.Clear();



            foreach (TextBlock tb in Tbc)
            {
                if (ShowAnswer)
                {
                    tb.Foreground = Brushes.Black;
                }
                else
                {
                    tb.Foreground = displays.Background;
                }
            }


            foreach (var tb in Tbr)
            {
                tb.Foreground = Brushes.Black;
                tb.FontWeight = FontWeights.Normal;
            }

            foreach (var tb in Tbu)
            {
                tb.Foreground = Brushes.DarkGray;

            }


            Tbr[0].Foreground = myBrush;
            Tbr[0].FontWeight = FontWeights.Medium;
            InputBox.Focus();

        }



        private void FormatDisplay()
        {


            string currentPath = System.AppDomain.CurrentDomain.BaseDirectory;
            Uri uri = new Uri(currentPath);
            FontFamily fm = new FontFamily(uri, cfg["字根字体"] + "," + "./fonts/#五笔字根字体");
            foreach (var tb in Tbr)
            {

                tb.Padding = new Thickness(0, 0, 0, 0);
                tb.FontSize = 30 * Convert.ToDouble(cfg["字体大小"]);
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Bottom;

                tb.TextWrapping = TextWrapping.NoWrap;





                tb.FontFamily = fm;
   
            }
            foreach (var tb in Tbc)
            {

                tb.Padding = new Thickness(0, 0, 0, 0);
                double size_eff = 1.0;
                if (tb.Text.Length >= 4)
                {
                    size_eff = 1.0 - Convert.ToDouble(tb.Text.Length) * 0.05;
                    tb.FontWeight = FontWeights.DemiBold;
                }
                else
                {
                    tb.FontWeight = FontWeights.Normal;
                }

                tb.FontSize = CodeFw * Convert.ToDouble(cfg["字体大小"]) * size_eff;



                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.FontFamily = new FontFamily(cfg["编码字体"]);
                //               tb.Background = Brushes.Gray ;
                tb.TextWrapping = TextWrapping.NoWrap;
            }

            foreach (var tb in Tbu)
            {
                tb.Padding = new Thickness(0, 0, 0, 0);
                double size_eff = 1.0;
                if (tb.Text.Length >= 4)
                {
                    size_eff = 1.0 - Convert.ToDouble(tb.Text.Length) * 0.05;
                    tb.FontWeight = FontWeights.DemiBold;
                }
                else
                {
                    tb.FontWeight = FontWeights.Normal;
                }
                tb.FontSize = CodeFw * Convert.ToDouble(cfg["字体大小"]) * size_eff;



                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.FontFamily = new FontFamily(cfg["编码字体"]);

                tb.TextWrapping = TextWrapping.NoWrap;

            }


        }
        private void RefreshDisplay(int ErrCode)
        {




            if (double.TryParse(speed.Text, out double tmp1))
            {
                cfg["换段击键"] = tmp1.ToString();
            }
            else
            {
                speed.Text = cfg["换段击键"];
            }


            if (int.TryParse(TbMaxCode.Text, out int tmp2))
            {
                //               MaxCode = tmp2;
                cfg["自动上屏码长"] = tmp2.ToString();
            }
            else
            {

                TbMaxCode.Text = cfg["自动上屏码长"];
            }


            //下一个高亮
            if (cur + 1 < DisplayRoot[Convert.ToInt32(cfg["上次的段数"])].Count)
            {
                Tbr[cur + 1].Foreground = myBrush;
                Tbr[cur + 1].FontWeight = FontWeights.Medium;
            }




            Tbr[cur].FontWeight = FontWeights.Normal;

            if (ErrCode == 1)
            {
                Tbr[cur].Foreground = Brushes.Red;
                Tbc[cur].Foreground = Brushes.Red;
            }
            else if (ErrCode == 2)
            {
                Tbr[cur].Foreground = Brushes.MediumPurple;
                if (Tbc[cur].Text != Tbu[cur].Text || ShowAnswer)
                    Tbc[cur].Foreground = Brushes.MediumPurple;
            }
            else if (ErrCode == 0)
            {
                Tbr[cur].Foreground = Brushes.DarkGray;

                if (Tbc[cur].Text != Tbu[cur].Text || ShowAnswer)
                    Tbc[cur].Foreground = Brushes.DarkGray;
            }
        }




        private void Retype(object sender, ExecutedRoutedEventArgs e)
        {
            RetypeThisGroup();
        }

        private void EndInput(bool RoundFail)
        {

            UpdateLog();
            WriteLog();
            sw.Stop();





            if (RoundFail)
            {
                stateEnd = 2;

            }
            else if (grouphit >= TargetHit && (ShowAnswer == false || cfg["编码提示"] == "打开"))
            {
                if (cfg["编码提示"] == "隔轮")
                    ShowAnswer = true;
                NextGroup();  //下一组
            }
            else
            {
                if (cfg["编码提示"] == "隔轮")
                    ShowAnswer = !ShowAnswer;
                round++;
                InitGroup();//重打

            }


        }

        private double CalHit()
        {
            double sum = 0;
            foreach (TextBlock tb in Tbu)
            {
                if (tb.Text != "")
                {
                    if (cfg["练习编码"] == "全码")
                    {
                        sum += tb.Text.Length;
                        if (tb.Text.Length < Convert.ToInt32(cfg["自动上屏码长"]) && !cfg["重码上屏键"].Contains(tb.Text.Substring(tb.Text.Length - 1, 1)))
                            sum++;
                    }
                    else
                    {
                        sum++;
                    }


                }
            }
            return sum / sw.Elapsed.TotalSeconds;

        }
        private void DisplayHit()
        {

            if (sw.Elapsed.TotalSeconds > 0.0001)
                hitrate.Text = "击键 " + CalHit().ToString("0.00") + " / " + TargetHit.ToString("0.00");
            else
                hitrate.Text = "击键 " + grouphit.ToString("0.00") + " / " + TargetHit.ToString("0.00");

        }

        private void UpdateFileList()
        {
            DirectoryInfo folder = new DirectoryInfo(@"mb");



            foreach (FileInfo file in folder.GetFiles("*.txt"))
                FileSelector.Items.Add(file.Name.Substring(0, file.Name.Length - 4));
            FileSelector.SelectedIndex = 0;
        }

        private void InitGroup() //初始化组
        {
            InitTextblocks();
            InGroupRand();
            FormatDisplay();
            WriteCfg();

            TargetHit = Convert.ToDouble(cfg["换段击键"]) - Convert.ToDouble(cfg["每轮降击"]) * (round);
            if (mode == "varible")
                TargetHit = Math.Round((float)(TargetHit * Math.Pow(AverageGroupSize / (double)DisplayRoot[Convert.ToInt32(cfg["上次的段数"])].Count, 0.35)), 2);

            if (TargetHit < 0.01)
                TargetHit = 0.01;
            sw.Reset();
            DisplayHit();
            foreach (var tb in Tbr)
            {
                tb.Text = "";

            }
            foreach (var tb in Tbc)
            {
                tb.Text = "";
            }
            foreach (var tb in Tbu)
            {
                tb.Text = "";
            }

            for (int i = 0; i < DisplayRoot[Convert.ToInt32(cfg["上次的段数"])].Count; i++)
            {
                Tbr[i].Text = DisplayRoot[Convert.ToInt32(cfg["上次的段数"])][i];

                if (cfg["练习编码"] == "全码")
                {
                    Tbc[i].Text = MB[DisplayRoot[Convert.ToInt32(cfg["上次的段数"])][i]];
                    double size_eff = 1.0;
                    if (Tbc[i].Text.Length >= 4)
                    {
                        size_eff = 1.0 - Convert.ToDouble(Tbc[i].Text.Length) * 0.05;
                        Tbc[i].FontWeight = FontWeights.DemiBold;
                    }
                    else
                    {
                        Tbc[i].FontWeight = FontWeights.Normal;
                    }

                    Tbc[i].FontSize = CodeFw * Convert.ToDouble(cfg["字体大小"]) * size_eff;

                }
                else
                {
                    Tbc[i].Text = MB[DisplayRoot[Convert.ToInt32(cfg["上次的段数"])][i]].Substring(0, 1);


                }

                if (ShowAnswer == true)
                {

                    Tbc[i].Foreground = Brushes.Black;
                }
                else
                {
                    Tbc[i].Foreground = displays.Background;

                }
            }






            stattxt.Text = "第 " + (Convert.ToInt32(cfg["上次的段数"]) + 1) + "/" + TotalGroup + " 段";
            RetypeThisGroup();

        }

        private void NextGroup()
        {

            cfg["上次的段数"] = (Convert.ToInt32(cfg["上次的段数"]) + 1).ToString();
            if (Convert.ToInt32(cfg["上次的段数"]) == TotalGroup)
                cfg["上次的段数"] = "0";
            sld.Value = Convert.ToInt32(cfg["上次的段数"]) + 1;
            round = 0;
            InitGroup();
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
                round = 0;
                InitGroup();

                return;
            }
            else
            {
                cfg["上次的段数"] = "0";
                return;
            }

        }

        public MainWindow()
        {

            InitializeComponent();

            //         InitTextblocks();
            ReadBaseMb("基础码表.txt");
            UpdateFileList();
            InitCfg();

            ReadLog();

            myArgbColor = Color.FromArgb(255, 0, 90, 150);
            myBrush = new SolidColorBrush(myArgbColor);








            ReadTxt();



        }

        private void FileSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CfgInit)
                ReadTxt();

        }


        private void Rst_Click(object sender, RoutedEventArgs e)
        {
            RetypeThisGroup();
        }



        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CfgInit && SliderInit)
            {
                cfg["上次的段数"] = (Convert.ToInt32(sld.Value) - 1).ToString();
                round = 0;
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



        private void FontSizeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CfgInit)
            {
                cfg["字体大小"] = fsize.Value.ToString();
                FormatDisplay();
                WriteCfg();
            }

        }


        private void change_hint(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CfgInit)
            {
                if (e.NewValue == 2)
                {
                    cfg["编码提示"] = "关闭";
                    ShowAnswer = false;
                }
                else if (e.NewValue == 0)
                {
                    ShowAnswer = true;
                    cfg["编码提示"] = "打开";
                }
                else if (e.NewValue == 1)
                {
                    ShowAnswer = true;
                    cfg["编码提示"] = "隔轮";
                }

                if (DisplayRoot != null)
                {
                    InitGroup();

                }
                WriteCfg();
            }

        }

        private void TbMaxCode_TextChange(object sender, TextChangedEventArgs e)
        {
            if (CfgInit)
            {
                cfg["自动上屏码长"] = TbMaxCode.Text;
                if (DisplayRoot != null)
                {
                    InitGroup();

                }
                WriteCfg();
            }
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

        private void set_sld_value(Slider s, double val)
        {
            s.Value = val;
        }

        private void change_code(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CfgInit)
            {
                if (DisplayRoot != null)
                {
                    InitGroup();

                }
                WriteCfg();
            }
        }

        private void hint_off(object sender, MouseButtonEventArgs e)
        {
            Hint.Value = 2;
            cfg["编码提示"] = "关闭";
        }

        private void hint_toggle(object sender, MouseButtonEventArgs e)
        {
            Hint.Value = 1;
            cfg["编码提示"] = "隔轮";
        }

        private void hint_on(object sender, MouseButtonEventArgs e)
        {
            Hint.Value = 0;
            cfg["编码提示"] = "打开";
        }




        private void expanded(object sender, RoutedEventArgs e)
        {


            if (CfgInit)
            {
                cot.Height = new GridLength(cot.ActualHeight);
                Application.Current.MainWindow.Height += 139;
                cfg["设置面板"] = "展开";
                cfg["窗口宽度"] = Application.Current.MainWindow.Width.ToString();
                cfg["窗口高度"] = Application.Current.MainWindow.Height.ToString();
                WriteCfg();

                cot.Height = new GridLength(1.0, GridUnitType.Star);
            }

        }

        private void expd_Collapsed(object sender, RoutedEventArgs e)
        {

            if (CfgInit)
            {
                cot.Height = new GridLength(cot.ActualHeight);
                Application.Current.MainWindow.Height -= 139;
                cfg["设置面板"] = "折叠";
                cfg["窗口宽度"] = Application.Current.MainWindow.Width.ToString();
                cfg["窗口高度"] = Application.Current.MainWindow.Height.ToString();
                WriteCfg();
                cot.Height = new GridLength(1.0, GridUnitType.Star);
            }

        }

        private void SldCode_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CfgInit)
            {
                if (e.NewValue == 0)
                {
                    cfg["练习编码"] = "全码";
                }
                else if (e.NewValue == 1)
                {
                    cfg["练习编码"] = "首码";
                }

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
            if (File.Exists("config.txt"))
            {
                StreamReader sr = new StreamReader("config.txt");
                string[] lines = sr.ReadToEnd().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    string[] ls = line.Split(s2, StringSplitOptions.RemoveEmptyEntries);
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




            TbMaxCode.Text = cfg["自动上屏码长"];

            speed.Text = cfg["换段击键"];
            TextNum.Text = cfg["每组字数"];
            TextHitDecrease.Text = cfg["每轮降击"];

            fsize.Value = Convert.ToDouble(cfg["字体大小"]);

            if (cfg["练习编码"] == "全码")
            {
                SldCode.Value = 0;
            }
            else if (cfg["练习编码"] == "首码")
            {
                SldCode.Value = 1;
            }

            if (cfg["编码提示"] == "关闭")
            {
                Hint.Value = 2;
            }
            else if (cfg["编码提示"] == "隔轮")
            {
                Hint.Value = 1;
                ShowAnswer = true;
            }
            else if (cfg["编码提示"] == "打开")
            {
                Hint.Value = 0;
                ShowAnswer = true;
            }

            if (cfg["设置面板"] == "展开")
            {
                expd.IsExpanded = true;
            }
            else if (cfg["设置面板"] == "折叠")
            {
                expd.IsExpanded = false;
            }

            Application.Current.MainWindow.Width = Convert.ToInt32(Convert.ToDouble(cfg["窗口宽度"]));
            Application.Current.MainWindow.Height = Convert.ToInt32(Convert.ToDouble(cfg["窗口高度"]));


            CfgInit = true;

        }




        private void WriteCfg()
        {
            StreamWriter sr = new StreamWriter("config.txt");
            cfg["删除此文件即可重置设置"] = "获取更新加Q群：21134461";
            foreach (var item in cfg)
            {
                sr.WriteLine(item.Key + "\t" + item.Value);
            }
            sr.Flush();
            sr.Close();
        }


        private void ReadLog()
        {
            char[] s2 = { '\t', '\r', '\n' };
            if (File.Exists("打字记录.txt"))
            {
                StreamReader sr = new StreamReader("打字记录.txt");
                string[] lines = sr.ReadToEnd().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    if (line == "合计：")
                        break;
                    string[] ls = line.Split(s2, StringSplitOptions.RemoveEmptyEntries);

                    if (ls.Length == 3)
                        log[ls[0] + "\t" + ls[1]] = Convert.ToInt32(ls[2]);
                }
                sr.Close();

            }


        }

        private void UpdateLog()
        {
            string item = TxtFile + "\t" + DateTime.Now.ToString("d");

            log.TryGetValue(item, out int count);

            count += DisplayRoot[Convert.ToInt32(cfg["上次的段数"])].Count;

            log[item] = count;

        }


        private void WriteLog()
        {
            StreamWriter sr = new StreamWriter("打字记录.txt");
            int total = 0;
            Dictionary<string, int> sum = new Dictionary<string, int>();
            foreach (var item in log)
            {
                sr.WriteLine(item.Key + "\t" + item.Value);
                string filename = item.Key.Split(new char[] { '\t' })[0];
                sum.TryGetValue(filename, out int count);
                count += item.Value;
                sum[filename] = count;
                total += item.Value;
            }
            sr.WriteLine("");
            sr.WriteLine("合计：");
            sr.WriteLine("总字数\t" + total);


            foreach (var item in sum)
            {
                sr.WriteLine(item.Key + "\t" + item.Value);
            }
            sr.Flush();
            sr.Close();
        }

        private void SldHit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void win_size_change(object sender, SizeChangedEventArgs e)
        {

            if (CfgInit)
            {
                cfg["窗口宽度"] = Convert.ToInt32(Application.Current.MainWindow.Width).ToString();
                cfg["窗口高度"] = Convert.ToInt32(Application.Current.MainWindow.Height).ToString();

                WriteCfg();
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
                        InitTextblocks();
                        ReadTxt();

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

        private void InputBox_TextInput(object sender, TextCompositionEventArgs e)
        {


            if (!sw.IsRunning && stateEnd != 2)
            {
                sw.Start();
            }
            Tbu[cur].Text += e.Text;

            string s = Tbu[cur].Text;

            if (cfg["练习编码"] == "首码" && s.Length > 0)
            {
                NextWord("");

            }

            else if (cfg["练习编码"] == "全码" && cfg["重码上屏键"].Contains(e.Text))
            {
                NextWord("");


            }

            else if (cfg["练习编码"] == "全码" && s.Length == Convert.ToInt32(cfg["自动上屏码长"]) && MB[DisplayRoot[Convert.ToInt32(cfg["上次的段数"])][cur]].Length <= Convert.ToInt32(cfg["自动上屏码长"])) //超长自动上屏
            {

                NextWord("");


            }

            else if (cfg["练习编码"] == "全码" && s.Length >= Convert.ToInt32(cfg["自动上屏码长"]) + 1 && MB[DisplayRoot[Convert.ToInt32(cfg["上次的段数"])][cur]].Length > Convert.ToInt32(cfg["自动上屏码长"])) //超长自动上屏
            {

                Tbu[cur].Text = Tbu[cur].Text.Substring(0, Tbu[cur].Text.Length - 1);
                NextWord(s.Substring(s.Length - 1, 1));


            }

            //        && cur + 1 < DisplayRoot[Convert.ToInt32(cfg["上次的段数"])].Count
        }




        private void NextWord(string CacheKey)
        {

            InputWords[cur] = InputBox.Text.Trim();

            if (CacheKey.Length > 0 && cur + 1 < DisplayRoot[Convert.ToInt32(cfg["上次的段数"])].Count)
            {
                Tbu[cur + 1].Text = CacheKey;

            }



            if (stateEnd == 2) //结束有错
            {
                if (cfg["编码提示"] == "隔轮" && ShowAnswer == false)
                    ShowAnswer = true;
                round++;
                InitGroup();
                return;
            }

            if (sw.IsRunning == false)
            {
                sw.Start();

            }


            if (cur < DisplayRoot[Convert.ToInt32(cfg["上次的段数"])].Count)
            {
                if (cfg["练习编码"] == "全码")
                {
                    string s = Tbu[cur].Text;
                    if (!RMB.ContainsKey(s) || (RMB[s] != DisplayRoot[Convert.ToInt32(cfg["上次的段数"])][cur] && s != MB[DisplayRoot[Convert.ToInt32(cfg["上次的段数"])][cur]]))
                    {
                        RefreshDisplay(1);
                        RoundFail = true;
                    }
                    else if (BackSpace > 0)
                    {
                        RefreshDisplay(2);
                        RoundFail = true;
                        BackSpace = 0;

                    }
                    else
                    {
                        RefreshDisplay(0);
                    }
                }
                else
                {
                    string s = Tbu[cur].Text;
                    if (s != MB[DisplayRoot[Convert.ToInt32(cfg["上次的段数"])][cur]].Substring(0, 1))
                    {
                        RefreshDisplay(1);
                        RoundFail = true;
                    }
                    else if (BackSpace > 0)
                    {
                        RefreshDisplay(2);
                        RoundFail = true;
                        BackSpace = 0;
                    }
                    else
                    {
                        RefreshDisplay(0);
                    }
                }
            }


            if (cur + 1 < DisplayRoot[Convert.ToInt32(cfg["上次的段数"])].Count)
            {
                DisplayHit();
                cur++;

            }
            else
            {

                grouphit = CalHit();
                //                DisplayHit();
                EndInput(RoundFail);
            }








            return;
        }

        private void InputBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                NextWord("");
            }

            if (e.Key == Key.Back)
            {
                if (Tbu[cur].Text.Length > 0)
                {
                    Tbu[cur].Text = Tbu[cur].Text.Substring(0, Tbu[cur].Text.Length - 1);
                    BackSpace = 1;
                }

            }

        }
    }
}
