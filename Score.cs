using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Globalization;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;

namespace TypeB
{
    public enum ZiciType
    {
        zi,
        ci,
        punct
    };
static internal class Score
    {
        public static int Hit = 0;
        public static int LeftCount = 0;
        public static int RightCount = 0;
        public static int SpaceCount = 0;
        public static double HitRate = 0;
        public static int TotalWordCount = 0;
        public static int InputWordCount = 0;
        public static int CurWord = 0;
        public static double Speed = 0;
        public static int Backs = 0;
        public static double KPW = 0;
  //      public static double Accuracy = 0;
        public static int Wrong = 0;
        public static int More = 0;
        public static int Less = 0;
        public static TimeSpan Time;
        private static Random RND = new Random();
        public static int Paragraph = 0;
        public static int Correction = 0;
        public static double LRRatio = 0;

        public static int BimeHit = 0;
        public static int BimeBacks = 0;
        public static int BimeCorrection = 0;



        //打词率计算
        public static Stack<ZiciType> ZiciStack = new Stack<ZiciType>();


        public static List<long> ImeKeyTime = new List<long>();
        public static List<long> CommitTime = new List<long>();
        public static List<int> ImeKeyValue  = new List<int>();
        public static List<string> CommitStr = new List<string>();

        public static List<long> BiaoDingImeKeyTime = new List<long>();
        public static List<long> BiaoDingCommitTime = new List<long>();
        public static List<int> BiaoDingImeKeyValue = new List<int>();
        public static List<string> BiaoDingCommitStr = new List<string>();


        public static string ExcludePuncts = "~!@#$%^&*()_+|}{\":?><`[]\\;',./~！@#￥%……&*（）——+{}|：“”《》？·、【】；‘’，。";
        public static void AddInputStack(string text)
        {
            var si = new StringInfo(text);
            if (si.LengthInTextElements == 0)
                return;

            bool[] IsPunct = new bool[si.LengthInTextElements];
            int NonePunctCount = 0;
            for (int i=0;i< si.LengthInTextElements;i++)
            {
                if (ExcludePuncts.Contains(si.SubstringByTextElements(i, 1)))
                    IsPunct[i] = true;
                else
                    NonePunctCount++;
            }

            bool IsCi = NonePunctCount >= 2;

            foreach (bool ispunct in IsPunct)
            {
                if (ispunct)
                    ZiciStack.Push(ZiciType.punct);
                else if (IsCi)
                    ZiciStack.Push(ZiciType.ci);
                else
                    ZiciStack.Push(ZiciType.zi);
            }
        }
        public static double GetHit ()
        {
            if (BimeHit > 0)
                return BimeHit;
            else
                return Hit;

          //  return Math.Max(Hit, BimeHit);
        }

        public static double GetBacks()
        {

            return BimeBacks > 0 ? BimeBacks : Backs;
          //  return Math.Max(Backs, BimeBacks);
        }

        public static double GetValidSpeed()
        {
            double rt = Speed;

            if (Wrong > 0)
            {
                rt = (double)(InputWordCount - Wrong * 5) / Time.TotalMinutes;

            }
            else
            {
                rt = Speed;
            }

            return rt;
        }
        public static double GetCorrection()
        {
            return Math.Max(Correction, BimeCorrection);
        }

        public static double GetAccuracy()
        {
    

           return   (GetHit() - GetCorrection() - GetBacks() * 2.0) / (TotalWordCount + GetCorrection()) * TotalWordCount / GetHit();
            

     
        }

        public static double GetCiRatio()
        {
            double total = 0;
            double ci = 0;
            foreach (ZiciType type in ZiciStack)
            {
                switch (type)
                {
                    case ZiciType.zi:
                        total++;
                        break;
                    case ZiciType.ci:
                        ci++;
                        total++;
                        break;
                    case ZiciType.punct:
                        break;
                    default:
                        break;

                }
            }



            if (total == 0)
                return 0;
            else
                return ci / total;
        }
        public static void Reset()
        {
            Hit = 0;
            LeftCount = 0;
            RightCount = 0;
            SpaceCount = 0;
            HitRate = 0;
            TotalWordCount = 0;
            InputWordCount = 0;
            CurWord = 0;
            Speed = 0;
            Backs = 0;
            Correction = 0;
            KPW = 0;
        //    Accuracy = 0;
            Time = new TimeSpan(0);
            Wrong = 0;
            More = 0;
            Less = 0;
            BimeHit = 0;
            BimeBacks = 0;
            BimeCorrection = 0;
            LRRatio = 0;
            ZiciStack.Clear();

            ImeKeyTime.Clear();
            CommitTime.Clear();
            ImeKeyValue.Clear();
            CommitStr.Clear();



            BiaoDingImeKeyTime.Clear();
            BiaoDingCommitTime.Clear();
            BiaoDingImeKeyValue.Clear();
            BiaoDingCommitStr.Clear();

        }

        public static string Progress()
        {
            StringBuilder r = new StringBuilder();

            string SpeedReport = Score.Speed.ToString("F2"); ;
            if (Wrong > 0)
            {
                SpeedReport = ((double)(InputWordCount - Wrong * 5) / Time.TotalMinutes).ToString("F2") + "/" + SpeedReport;

            }

            r.AppendFormat("{0}\t{1}\t{2}\t{3}", SpeedReport, Score.HitRate.ToString("F2"), Score.KPW.ToString("F2"), GetAccuracy().ToString("P2"));

            return r.ToString();
        }
        public static string Report()
        {
            List<string> report = new List<string>();

            string SpeedReport = Math.Round( Score.Speed , 2).ToString("F2"); ;

            if (Config.GetBool("看打模式"))
            {
                int wr = Math.Max(More, Less);
                if (wr > 0)
                    SpeedReport = Math.Round(((double)(TotalWordCount - wr * 5) / Time.TotalMinutes), 2).ToString("F2") + "/" + SpeedReport;
            }
            else if (Wrong > 0)
            {
                SpeedReport = Math.Round(((double)(TotalWordCount - Wrong * 5) / Time.TotalMinutes),2).ToString("F2") + "/" + SpeedReport;

            }

            string s = Config.GetString("成绩单屏蔽模块(逗号分隔多个)");
            bool isBime =  BimeHit > 0;
            bool notBime = !isBime;

            //计算伪装倍率


            double ratio1 = (1 + HitRate) / HitRate;
            double tmp_Machang = KPW * ratio1;

            if (tmp_Machang <= 2.7 && TotalWordCount > 50)
                ratio1 = (2.7 + RND.NextDouble() * 0.05) / Score.KPW;
            else if (tmp_Machang <= 2.8 && TotalWordCount > 50)
                ratio1 = (2.8 + RND.NextDouble() * 0.05) / Score.KPW;

            if (notBime && GetCiRatio() < 0.2)
                ratio1 = 1;


            report.Add("第" + Paragraph + "段");
            if (!s.Contains("速度"))
                report.Add("速度" + SpeedReport);
            if (!s.Contains("击键"))
            {
                if (Config.GetBool("伪装一"))
                {
                    report.Add("击键" + (HitRate * ratio1).ToString("F2"));
                }
                else
                    report.Add("击键" + HitRate.ToString("F2"));
            }

            if (StateManager.txtSource == TxtSource.trainer)
            {
                report.Add("/" + WinTrainer.TargetHit.ToString("F2"));
            }

            if (!s.Contains("码长"))
            {
                if (Config.GetBool("伪装一"))
                {
                    report.Add("码长" + (Score.KPW * ratio1 ).ToString("F2"));
                }
                else
                {
                    report.Add("码长" + Score.KPW.ToString("F2"));
                }
            }



            if (!s.Contains("字数")) 
                report.Add("字数"+TotalWordCount.ToString());

            int TypeCount  = RetypeCounter.Get(TextInfo.TextMD5);
            if (!s.Contains("重打"))
            {
                if (TypeCount > 1)
                    report.Add("重打" + (TypeCount -1).ToString());
            }
            if (!s.Contains("总键数"))
            {
                if (Config.GetBool("伪装一"))
                    report.Add("总键数" + (Math.Round(ratio1 * GetHit())).ToString("F0"));
                else
                    report.Add("总键数" +  GetHit().ToString("F0"));

            }


            if (!s.Contains("键法"))
                if (BimeHit == 0)
                {
 
                    if (RightCount == 0)
                        LRRatio = 1;
                    else
                        LRRatio =(double) LeftCount / (double)RightCount;


                    if (Config.GetBool("伪装一"))
                        report.Add("键法" + LRRatio.ToString("p2") + " (左" + Math.Round( ratio1 * LeftCount) + "右" + (Math.Round(ratio1 * GetHit()) - Math.Round(ratio1 * LeftCount) - Math.Round(ratio1 * SpaceCount)) + "空格" + Math.Round(ratio1 * SpaceCount) + ")");
                    else
                        report.Add("键法" + LRRatio.ToString("p2")+ " (左" + LeftCount + "右" + RightCount + "空格" + SpaceCount + ")");

                }

            if (!s.Contains("回改")) 
                report.Add("回改" + Score.GetCorrection().ToString("F0"));
            if (!s.Contains("退格")) 
                report.Add("退格" + GetBacks().ToString("F0"));
            if (!s.Contains("键准")) 
                report.Add("键准" + GetAccuracy().ToString("P2"));
            if (!s.Contains("打词率") && notBime)
            {
                if (Config.GetBool("伪装一"))
                {
                    report.Add("打词率" + (0.01+ 0.04 * RND.NextDouble()).ToString("P2"));
                }
                else
                    report.Add("打词率" + GetCiRatio().ToString("P2"));
            }

            if (!s.Contains("选重") && notBime)
                report.Add("选重" + GetChoose().ToString());

            if (!s.Contains("标顶") && notBime)
                report.Add("标顶" + GetBiaoDing().ToString());
            if (!s.Contains("用时"))
            {
                string t = Score.Time.ToString();
                int semi = t.LastIndexOf(":");
                if (t.Length > semi + 6)
                    t = t.Substring(0,semi + 6);
                report.Add("用时" + t);//(@"hh\:mm\:ss"))
            }



            if (!s.Contains("错字"))
            {
                

                if (Config.GetBool("看打模式"))
                {
                    if (Less > 0 && More > 0)
                        report.Add("少" + Less + "多" + More);
                    else if (More > 0)
                        report.Add("多" + More);
                    else if (Less > 0)
                        report.Add("少" + Less);
                }
                else

                
                {
                    if (Wrong > 0)
                        report.Add("错字" + Wrong);
                }
            }

            if (!s.Contains("盲打正确率") && Config.GetBool("盲打模式"))
            {
                int wr = Math.Max(More, Less);
                double ratio; 

                ratio = Math.Round((double)(TotalWordCount - wr) / (double)TotalWordCount, 4);
                report.Add("盲打正确率" + ratio.ToString("P2"));
            }


            if (!s.Contains("看打正确率") && Config.GetBool("看打模式") && !Config.GetBool("盲打模式"))
            {
                int wr = Math.Max(More, Less);
                double ratio;

                ratio = Math.Round((double)(TotalWordCount - wr) / (double)TotalWordCount, 4);
                report.Add("看打正确率" + ratio.ToString("P2"));
            }

            if (!s.Contains("重打"))
            {
                if (TypeCount <=1)
                    report.Add("【首打认证】");
            }


            if (!s.Contains("盲打模式") && Config.GetBool("盲打模式"))
                report.Add("【盲打模式】");

            if (!s.Contains("看打模式") && Config.GetBool("看打模式") && !Config.GetBool("盲打模式"))
                    report.Add("【看打模式】");

            if (!s.Contains("签名"))
            {
 
                    report.Add(Config.GetString("成绩签名"));
            }


            report.Add(StateManager.Version);
            return string.Join(" ",report);
        }

        static List<Key> KeysLeft = new List<Key>
        {
           Key.Oem3,
            Key.D1,
            Key.D2,
            Key.D3,
            Key.D4,
            Key.D5,
            Key.Tab,
            Key.Q,
            Key.W,
            Key.E,
            Key.R,
            Key.T,
            Key.Capital,
            Key.A,
            Key.S,
            Key.D,
            Key.F,
            Key.G,
            Key.LeftShift,
            Key.Z,
            Key.X,
            Key.C,
            Key.V,
            Key.B,
            Key.LeftCtrl,
            Key.LWin

        };

        static List<Key> KeysRight = new List<Key>
        {
            Key.D6,
            Key.D7,
            Key.D8,
            Key.D9,
            Key.D0,
            Key.OemMinus,
            Key.OemPlus,
            Key.Back,
            Key.Y,
            Key.U,
            Key.I,
            Key.O,
            Key.P,
            Key.OemOpenBrackets,
            Key.Oem6,
            Key.Oem5,
            Key.H,
            Key.J,
            Key.K,
            Key.L,
            Key.Oem1,
            Key.OemQuotes,
            Key.N,
            Key.M,
            Key.OemComma,
            Key.OemPeriod,
            Key.OemQuestion,
            Key.RightShift,
            Key.Return,
            Key.RightCtrl,

        };

        static public int GetChoose()
        {
            int choose = 0;

            long thresh = 20;
            


            for (int i = 0; i< ImeKeyTime.Count; i++)
            {
                for (int j = 0; j< CommitTime.Count; j++)
                {
                    if (CommitTime[j] > ImeKeyTime[i] + thresh)
                        break;

                    if (Math.Abs(CommitTime[j] - ImeKeyTime[i]) <= thresh)
                        if (IntStringDict.Selection.ContainsKey(ImeKeyValue[i]) && !IntStringDict.Selection[ImeKeyValue[i]].Contains(CommitStr[j])) 
                        {
                            choose++;
                            break;
                        }

                }
            }




            return choose;
        }



        static public int GetBiaoDing()
        {
            int bd= 0;

            long thresh = 20;



            for (int i = 0; i < BiaoDingImeKeyTime.Count; i++)
            {
                for (int j = 0; j < BiaoDingCommitTime.Count; j++)
                {
                    if (BiaoDingCommitTime[j] > BiaoDingImeKeyTime[i] + thresh)
                        break;

                    if (Math.Abs(BiaoDingCommitTime[j] - BiaoDingImeKeyTime[i]) <= thresh)
                        if (IntStringDict.BiaoDing.ContainsKey(BiaoDingImeKeyValue[i]) && IntStringDict.BiaoDing[BiaoDingImeKeyValue[i]].Contains(BiaoDingCommitStr[j]))
                        {
                            bd++;
                            break;
                        }

                }
            }




            return bd;
        }

        static public void SetJianFa(Key key)
        {
            if (key  == Key.Space)
                SpaceCount++;
           if (KeysRight.Contains(key))
                RightCount++;
           else if (KeysLeft.Contains(key))
                LeftCount++;

        }
    }
}
