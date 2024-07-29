using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;


namespace TestArticle
{
    public class Article
    { 
        public string Title { get; set; } 
        public StringInfo Text { get; set; }


        public Article(string title, string text)//, int progress = 0)
        {
            Title = title;
            Text = new StringInfo(text.Replace("\n","").Replace("\r", "").Replace("\t", ""));

        }
    }  
    internal static class  ArticleManager
    {
        const string FolderPath = "文章";



        public static Dictionary <string,Article> Articles = new Dictionary<string,Article> ();


   
        public static void ReadFiles ()
        {
            Articles.Clear();
            DirectoryInfo dir = new DirectoryInfo(FolderPath);


            foreach (FileInfo file in dir.GetFiles("*.txt")) 
            {
                string name = file.Name;

                Encoding enc = TxtFileEncoder.GetEncoding(file.FullName);
                string txt = File.ReadAllText(file.FullName,enc);

 
                Articles.Add(name, new Article(name, txt));
       

            }


        }

        public static int SectionSize
        {
            get
            {
                return ArticleConfig.GetInt("每段字数");
            }
            set
            {
                ArticleConfig.Set("每段字数", value);
                ArticleConfig.WriteConfig(500);
            }
        }


        public  static string GetCurrentSection()
        {

            string rt;
            if (!Articles.ContainsKey(Title))
                return "";

            if (Progress >= TotalSize)
                return "没了";

            if (Progress + SectionSize < TotalSize)
                rt =  Articles[Title].Text.SubstringByTextElements((Index - 1) * SectionSize, SectionSize);
            else
                rt = Articles[Title].Text.SubstringByTextElements((Index - 1) * SectionSize);



            if (EnableFilter)
            {
                //            txt = Filter.ProcFilter(txt);

                rt = Filter.ProcFilter(rt);
            }

            if (RemoveSpace)
                rt = rt.Replace(" ", "").Replace("　", "");

            return rt;
        }

        public static void NextSection()
        {
            if (!Articles.ContainsKey(Title))
                return;

            if (Progress >= TotalSize)
                return;

            Progress = Math.Min(Progress + SectionSize, TotalSize);
            

        }

        public static void PrevSection()
        {
            if (!Articles.ContainsKey(Title))
                return;

            if (Progress ==  0)
                return;

            Progress = Math.Max(Progress - SectionSize, 0);


        }

        public static string GetFormattedCurrentSection()
        {
            if (!Articles.ContainsKey(Title))
                return "";

            StringBuilder sb = new StringBuilder();
            sb.Append(Title);
            sb.AppendLine();
            string txt = GetCurrentSection();
            sb.Append(txt);
            sb.AppendLine();
            sb.Append("-----第");
            sb.Append(Index);
            sb.Append("段");


            sb.Append("-");

            sb.Append(" 共");
            sb.Append(MaxIndex);
            sb.Append("段 ");

            sb.Append(" 进度 ");
            sb.Append((Index - 1) * SectionSize);
            sb.Append("/");
            sb.Append(TotalSize);
            sb.Append("字 ");

            sb.Append(" 本段");
            sb.Append(new StringInfo(txt).LengthInTextElements);
            sb.Append("字 ");

            sb.Append("Pain散播器");
            return sb.ToString();
            
        }

        public static string GetFormattedNextSection()
        {

            string rt;
            if (!Articles.ContainsKey(Title))
                return "";
            else
            {
                rt = GetFormattedCurrentSection();
                NextSection();
            }
            return rt; 
        }

        public static int Index
        {
            get
            {
                if (!Articles.ContainsKey(Title))
                    return 1;
                else
                {
                    int counter = 0;


                    while (true)
                    {
                        if (SectionSize * counter -1 >= Progress)
                            break;

                        counter++;
                    }
              

                    return counter;
                }

            }


        }

        public static int TotalSize
        {
            get
            {
                return Articles[Title].Text.LengthInTextElements;
            }
        }

        public static int MaxIndex
        {
            get
            {
                if (!Articles.ContainsKey(Title))
                    return 1;
                else
                {
                    int counter = 0;

                    while (true)
                    {
                        if (SectionSize * counter - 1 >= TotalSize -1 )
                            break;

                        counter++;
                    }

                    return counter;
                }

            }



        }

        public static string Title
        {
            get
            {
                return ArticleConfig.GetString("当前文章");
            }
        }

        public static int Progress
        {
            get
            {
                if (!Articles.ContainsKey(Title))
                    return 0;
                else
                    return ArticleConfig.GetInt("进度_" + Title);
            }
            set
            {
                if (Articles.ContainsKey(Title))
                {
                    int v = Math.Min( Math.Max(0, value), TotalSize - 1);

                    ArticleConfig.Set("进度_" + Title, v);
                    ArticleConfig.WriteConfig(500);
    
                }

            }
        }

        public static bool EnableFilter
        {
            get
            {

                    return ArticleConfig.GetBool("字集过滤");
            }
            set
            {

                    ArticleConfig.Set("字集过滤" , value);
                    ArticleConfig.WriteConfig(500);


            }
        }

        public static bool RemoveSpace
        {
            get
            {

                return ArticleConfig.GetBool("去除空格");
            }
            set
            {

                ArticleConfig.Set("去除空格", value);
                ArticleConfig.WriteConfig(500);


            }
        }


        public static int  Search(string text, int startIndex)
        {
            int rt = -1;
            
            rt = Articles[Title].Text.String.IndexOf(text,startIndex);
            if (rt > 0)
                rt = new StringInfo( Articles[Title].Text.String.Substring(0, rt)).LengthInTextElements;

            return rt;
        }

        static ArticleManager()
        {
            ArticleConfig.SetDefault
            (
                "每段字数", "200",
                "字集过滤", "是",
                "去除空格", "是"
                
            );

            ArticleConfig.ReadConfig();


        }
    }
}
