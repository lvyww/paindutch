using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace TypeB
{
    public enum WordStates
    {
        NO_TYPE,
        RIGHT,
        WRONG,
        CURRENT
    };


internal static class TextInfo
    {
        public static string WrongExclude = "……——“”‘’";
        public static string MatchText = ""; //赛文
//        public static string CachedMatchText = "";//赛文
        public static int Paragraph = 0; //缓存段号
        public static TxtSource CacheTxtType = TxtSource.unchange; //赛文
        public static string TextMD5 = "";
        public static int PageNum = 0;
        public static List<TextBlock> Blocks = new List<TextBlock>(); //显示文本UI

        public static List <string> Words = new List<string>();

        public static List<WordStates> wordStates = new List<WordStates>(); //显示文本UI的状态

        public static List<WordStates> BlocksStates = new List<WordStates>(); //显示文本UI的状态

        public static int PageStartIndex = 0;

        public static bool Exit = false;

        public static Dictionary<int, string> WrongRec = new Dictionary<int, string>();

     //   public static Dictionary<int, string> WrongCounter = new Dictionary<int, string>();
    //    public static Dictionary<int, string> BackCounter = new Dictionary<int, string>();
    //    public static Dictionary<int, string> CorrectionCounter = new Dictionary<int, string>();
        static public string CalMD5(string text)
        {


            byte[] b = System.Text.Encoding.Default.GetBytes(text);

            b = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
            {
                ret += b[i].ToString("x").PadLeft(2, '0');
            }
            return ret;

        }




        public static void Check(string inputTxt)
        {
            StringInfo siInput = new StringInfo(inputTxt);

            if (TextInfo.Words.Count == 0)
            {
                return;
            }

            for (int i = 0; i < TextInfo.Words.Count; i++)
            {
                if (i >= siInput.LengthInTextElements)
                    TextInfo.wordStates[i] = WordStates.NO_TYPE;
                
                //else if (TextInfo.Words[i] == siInput.SubstringByTextElements(i, 1) ||            ( Config.GetBool("看打模式") &&  ( TextInfo.Words[i] == "“" || TextInfo.Words[i] == "”") && (siInput.SubstringByTextElements(i, 1) == "“" || siInput.SubstringByTextElements(i, 1) == "”")))
                else if (TextInfo.Words[i] == siInput.SubstringByTextElements(i, 1) || MainWindow.Current.IsLookingType)
                            TextInfo.wordStates[i] = WordStates.RIGHT;
                else
                    TextInfo.wordStates[i] = WordStates.WRONG;

            }
        }

   
    }
}
