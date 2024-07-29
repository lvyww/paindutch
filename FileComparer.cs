using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

namespace TypeB
{
    public class FileComparer
    {
        private int Llen { get; set; } = 0;
        private int Rlen { get; set; } = 0;
        private string[] source { get; set; } = null;
        private string[] current { get; set; } = null;
        private int[,] CellValue { get; set; } = null;
        private int[,] CellNumber { get; set; } = null;
        private int[,] CellDlen { get; set; } = null;
        private bool IsChanged { get; set; } = false;
        private List<Point> LineComList { get; set; } = new List<Point>();
        private List<LineCompareInfo> InfoList { get; set; } = new List<LineCompareInfo>();
        private StringBuilder sourceBuffer { get; set; } = new StringBuilder();
        private StringBuilder tagBuffer { get; set; } = new StringBuilder();

        public FileComparer()
        {
        }

        private string Replace_Special_String(string sourceStr)
        {
            return sourceStr.Replace(@"\", @"\\").Replace("{", @"\{").Replace("}", @"\}");
        }

        /// <summary>
        /// 比较文件的主函数
        /// </summary>
        /// <param name="src">比较文本之一</param>
        /// <param name="dst">比较文本之二</param>
        /// <returns>CompareResultString类对象，包含两个文件的Rtf字符串</returns>
        public CompareResultString Execute(string src, string dst)
        {
            source = src.Split('\r');
            current = dst.Split('\r');

            string[] LstrArray = null;
            string[] RstrArray = null;
            if (current.Length > source.Length)
            {
                IsChanged = true;
                Llen = current.Length;
                Rlen = source.Length;
                LstrArray = current;
                RstrArray = source;
            }
            else
            {
                Llen = source.Length;
                Rlen = current.Length;
                LstrArray = source;
                RstrArray = current;
            }

            CellValue = new int[Llen + 1, Rlen + 1];
            CellNumber = new int[Llen + 1, Rlen + 1];
            CellDlen = new int[Llen + 1, Rlen + 1];
            for (int i = Llen - 1; i >= 0; i--)
            {
                for (int j = Rlen - 1; j >= 0; j--)
                {
                    if (LstrArray[i].Trim() == RstrArray[j].Trim())
                    {
                        CellValue[i, j] = 1;
                        CellNumber[i, j] = TripleMax(CellNumber[i + 1, j + 1] + 1, CellNumber[i, j + 1], CellNumber[i + 1, j]);
                        CellDlen[i, j] = CellDlen[i + 1, j + 1] + 1;
                    }
                    else
                    {
                        CellValue[i, j] = Line_String_Compare(LstrArray[i], RstrArray[j]);
                        if (CellValue[i, j] > 0)
                        {
                            CellNumber[i, j] = TripleMax(CellNumber[i + 1, j + 1] + 1, CellNumber[i, j + 1], CellNumber[i + 1, j]);
                            if (CellNumber[i, j] == 0)
                            {
                                continue;
                            }
                            CellDlen[i, j] = CellDlen[i + 1, j + 1] + 1;
                        }
                        else
                        {
                            CellNumber[i, j] = TripleMax(CellNumber[i + 1, j + 1], CellNumber[i, j + 1], CellNumber[i + 1, j]);
                            if (CellNumber[i, j] == 0)
                            {
                                continue;
                            }
                            if (CellNumber[i, j + 1] >= CellNumber[i + 1, j])
                            {
                                CellDlen[i, j] = CellDlen[i, j + 1];
                            }
                            else
                            {
                                CellDlen[i, j] = CellDlen[i + 1, j] + 1;
                            }
                        }

                    }
                }
            }

            // 将比较结构构建为Rtf信息用于Richtext的显示
            Line_Information_Initialize();
            Create_Rtf_String();
            CompareResultString RtfTr = new CompareResultString();
            RtfTr.LeftRtf = sourceBuffer.ToString();
            RtfTr.RightRtf = tagBuffer.ToString();
            return RtfTr;
        }

        private int TripleMax(int a, int b, int c)
        {
            return a > b ? (a > c ? a : c) : (b > c ? b : c);
        }

        private void Create_Rtf_String()
        {
            foreach (LineCompareInfo infotemp in InfoList)
            {
                if (infotemp.IsChanged)
                {
                    if (infotemp.CompareDegree == 1)
                    {
                        sourceBuffer.Append("\\cf0" + source[infotemp.RightCount]);
                        tagBuffer.Append("\\cf0" + current[infotemp.LeftCount]);
                    }
                    else if (infotemp.CompareDegree == -1)
                    {
                        if (infotemp.LeftCount == -1)
                        {
                            sourceBuffer.Append("\\cf1");
                            sourceBuffer.Append(source[infotemp.RightCount]);
                            sourceBuffer.Append("\\cf0");
                        }
                        else
                        {
                            tagBuffer.Append("\\cf1");
                            tagBuffer.Append(current[infotemp.LeftCount]);
                            tagBuffer.Append("\\cf0");
                        }
                    }
                    else
                    {
                        Get_Compare_Infomations(source[infotemp.RightCount], current[infotemp.LeftCount]);
                    }
                }
                else
                {
                    if (infotemp.CompareDegree == 1)
                    {
                        sourceBuffer.Append("\\cf0" + source[infotemp.LeftCount]);
                        tagBuffer.Append("\\cf0" + current[infotemp.RightCount]);
                    }
                    else if (infotemp.CompareDegree == -1)
                    {
                        if (infotemp.LeftCount == -1)
                        {
                            tagBuffer.Append("\\cf1");
                            tagBuffer.Append(current[infotemp.RightCount]);
                            tagBuffer.Append("\\cf0");
                        }
                        else
                        {
                            sourceBuffer.Append("\\cf1");
                            sourceBuffer.Append(source[infotemp.LeftCount]);
                            sourceBuffer.Append("\\cf0");
                        }
                    }
                    else
                    {
                        Get_Compare_Infomations(source[infotemp.LeftCount], current[infotemp.RightCount]);
                    }
                }
                sourceBuffer.Append("\\par\r\n");
                tagBuffer.Append("\\par\r\n");
            }
        }

        private List<Point> String_Compare(string sou, string tag)
        {
            LineComList.Clear();
            IsChanged = false;
            char[] lchar = null;
            char[] rchar = null;
            if (sou.Length > tag.Length)
            {
                lchar = sou.ToCharArray();
                rchar = tag.ToCharArray();
            }
            else
            {
                IsChanged = true;
                lchar = tag.ToCharArray();
                rchar = sou.ToCharArray();
            }
            Rlen = lchar.Length;
            Llen = rchar.Length;
            CellValue = null;
            CellNumber = null;
            CellDlen = null;
            CellValue = new int[Llen + 1, Rlen + 1];
            CellNumber = new int[Llen + 1, Rlen + 1];
            CellDlen = new int[Llen + 1, Rlen + 1];
            for (int i = Llen - 1; i >= 0; i--)
            {
                for (int j = Rlen - 1; j >= 0; j--)
                {
                    if (rchar[i] == lchar[j])
                    {
                        CellValue[i, j] = 1;
                        CellNumber[i, j] = TripleMax(CellNumber[i + 1, j + 1] + 1, CellNumber[i, j + 1], CellNumber[i + 1, j]);
                    }
                    else
                    {
                        CellNumber[i, j] = TripleMax(CellNumber[i + 1, j + 1], CellNumber[i, j + 1], CellNumber[i + 1, j]);
                        CellValue[i, j] = 0;
                    }
                    if (CellNumber[i, j] == 0)
                    {
                        continue;
                    }
                    if (CellValue[i, j] == 1)
                    {
                        CellDlen[i, j] = CellDlen[i + 1, j + 1] + 1;
                    }
                    else
                    {
                        if (CellNumber[i, j + 1] >= CellNumber[i + 1, j])
                        {
                            CellDlen[i, j] = CellDlen[i, j + 1];
                        }
                        else
                        {
                            CellDlen[i, j] = CellDlen[i + 1, j] + 1;
                        }
                    }
                }
            }
            Get_Compare_List(new Point(0, 0));

            return LineComList;
        }

        private void Get_Compare_Infomations(string sou, string tag)
        {
            int lastx = -1;
            int lasty = -1;
            foreach (Point point in String_Compare(sou, tag))
            {
                int llen = point.X - lastx - 1;
                int rlen = point.Y - lasty - 1;
                if (llen > 0)
                {
                    if (IsChanged)
                    {
                        sourceBuffer.Append("\\cf1 ");
                        sourceBuffer.Append(sou.Substring(lastx + 1, llen));
                        sourceBuffer.Append("\\cf0 ");
                    }
                    else
                    {
                        tagBuffer.Append("\\cf1 ");
                        tagBuffer.Append(tag.Substring(lastx + 1, llen));
                        tagBuffer.Append("\\cf0 ");
                    }
                }
                if (rlen > 0)
                {
                    if (IsChanged)
                    {
                        tagBuffer.Append("\\cf1 ");
                        tagBuffer.Append(tag.Substring(lasty + 1, rlen));
                        tagBuffer.Append("\\cf0 ");
                    }
                    else
                    {
                        sourceBuffer.Append("\\cf1 ");
                        sourceBuffer.Append(sou.Substring(lasty + 1, rlen));
                        sourceBuffer.Append("\\cf0 ");
                    }
                }
                if (IsChanged)
                {
                    sourceBuffer.Append(sou[point.X]);
                    tagBuffer.Append(tag[point.Y]);
                }
                else
                {
                    tagBuffer.Append(tag[point.X]);
                    sourceBuffer.Append(sou[point.Y]);
                }
                lastx = point.X;
                lasty = point.Y;
            }
            if (IsChanged)
            {
                if (lasty < tag.Length - 1)
                {
                    tagBuffer.Append("\\cf1 ");
                    tagBuffer.Append(tag.Substring(lasty + 1));
                    tagBuffer.Append("\\cf0 ");
                }
                if (lastx < sou.Length - 1)
                {
                    sourceBuffer.Append("\\cf1 ");
                    sourceBuffer.Append(sou.Substring(lastx + 1));
                    sourceBuffer.Append("\\cf0 ");
                }
            }
            else
            {
                if (lasty < sou.Length - 1)
                {
                    sourceBuffer.Append("\\cf1 ");
                    sourceBuffer.Append(sou.Substring(lasty + 1));
                    sourceBuffer.Append("\\cf0 ");
                }
                if (lastx < tag.Length - 1)
                {
                    tagBuffer.Append("\\cf1 ");
                    tagBuffer.Append(tag.Substring(lastx + 1));
                    tagBuffer.Append("\\cf0 ");
                }
            }
            tagBuffer.Append("\\cf0 ");
            sourceBuffer.Append("\\cf0 ");
        }

        private void Line_Information_Initialize()
        {
            Get_Compare_List(new Point(0, 0));
            int lastlift = -1;
            int lastright = -1;
            int linecount = 0;
            foreach (Point pt in LineComList)
            {
                if (pt.X - lastlift - pt.Y + lastright > 0)
                {
                    for (int i = 0; i < pt.X - lastlift - 1; i++)
                    {
                        if (lastright + 1 + i < pt.Y)
                        {
                            this.InfoList.Add(new LineCompareInfo(linecount++, lastlift + 1 + i, lastright + 1 + i, 0, IsChanged));
                        }
                        else
                        {
                            this.InfoList.Add(new LineCompareInfo(linecount++, lastlift + 1 + i, -1, -1, IsChanged));
                        }
                    }

                }
                else if (pt.X - lastlift - pt.Y + lastright < 0)
                {
                    for (int i = 0; i < pt.Y - lastright - 1; i++)
                    {
                        if (lastlift + 1 + i < pt.X)
                        {
                            this.InfoList.Add(new LineCompareInfo(linecount++, lastlift + 1 + i, lastright + 1 + i, 0, IsChanged));
                        }
                        else
                        {
                            this.InfoList.Add(new LineCompareInfo(linecount++, -1, lastright + 1 + i, -1, IsChanged));
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < pt.Y - lastright - 1; i++)
                    {
                        this.InfoList.Add(new LineCompareInfo(linecount++, lastlift + 1 + i, lastright + 1 + i, 0, IsChanged));
                    }
                }
                this.InfoList.Add(new LineCompareInfo(linecount++, pt.X, pt.Y, CellValue[pt.X, pt.Y], IsChanged));
                lastlift = pt.X;
                lastright = pt.Y;
            }
            CellValue = null;
            CellNumber = null;
            CellDlen = null;
            LineComList.Clear();
        }

        private void Get_Compare_List(Point point)
        {

            if (point.X < Llen && point.Y < Rlen)
            {
                if (CellValue[point.X, point.Y] > 0)
                {
                    LineComList.Add(point);
                    Get_Compare_List(new Point(point.X + 1, point.Y + 1));
                }
                else
                {
                    Point NextPoint = Get_Next_Point(point);
                    if (NextPoint != new Point(0, 0))
                    {
                        LineComList.Add(NextPoint);
                        Get_Compare_List(new Point(NextPoint.X + 1, NextPoint.Y + 1));
                    }
                }
            }
            else
            {
                return;
            }
        }

        private Point Get_Next_Point(Point point)
        {
            if (point == new Point(0, 0))
            {
                return new Point(0, 0);
            }
            Point tempoint = new Point();
            int TripleMaxD = CellDlen[point.X, point.Y];
            int CellNumberv = CellNumber[point.X, point.Y];
            for (int i = point.X; i < Llen; i++)
            {
                for (int j = point.Y; j < Rlen; j++)
                {
                    if (CellValue[i, j] > 0 && CellNumber[i, j] == CellNumberv)
                    {
                        if (CellDlen[i, j] <= TripleMaxD)
                        {
                            tempoint.X = i;
                            tempoint.Y = j;
                        }
                    }
                }
            }
            return tempoint;
        }

        private int Line_String_Compare(string source, string target)
        {
            int m = target.Trim().Length;
            int n = source.Trim().Length;
            int matchCount = 0;

            string longStr, shortStr;

            if (m == 0 || n == 0)
            {
                return 0;
            }
            int k = 0;

            if (m > n)
            {
                k = 2 * n / 3 + 1;
                longStr = target.Trim();
                shortStr = source.Trim();
            }
            else
            {
                k = 2 * m / 3 + 1;
                longStr = source.Trim();
                shortStr = target.Trim();
            }

            int startNo = 0;
            for (int i = 0; i < shortStr.Length; i++)
            {
                int tmpNo = longStr.IndexOf(shortStr[i], startNo);

                if (tmpNo > 0)
                {
                    startNo = tmpNo + 1;
                    matchCount += 1;
                }

                if (matchCount > k)
                {
                    return 2;
                }
            }

            if (matchCount > k)
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }
    }

    public class LineCompareInfo
    {
        public int LineCount { get; set; } = 0;
        public int LeftCount { get; set; } = 0;
        public int RightCount { get; set; } = 0;
        public int CompareDegree { get; set; } = 0;
        public bool IsChanged { get; set; } = true;
        public LineCompareInfo(int line, int left, int right, int info, bool ischang)
        {
            this.LineCount = line;
            this.LeftCount = left;
            this.RightCount = right;
            this.CompareDegree = info;
            this.IsChanged = ischang;
        }
    }

    public class CompareResultString
    {
        public string LeftRtf { get; set; } = "";
        public string RightRtf { get; set; } = "";
    }

    public class CompareInfo
    {
        public int StartIndex { get; set; } = -1;
        public int EndIndex { get; set; } = -1;
        public CompareInfo()
        {
        }
        public CompareInfo(int start, int end)
        {
            StartIndex = start;
            EndIndex = end;
        }
    }
}