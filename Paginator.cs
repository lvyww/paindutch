using System;
using System.Collections.Generic;

namespace TypeB
{

    class Page
    {
        public int HeadStart = -1;
        public int HeadEnd = -1;
        public int BodyStart = -1;
        public int BodyEnd = -1;
        public int FootStart = -1;
        public int FootEnd = -1;


    }
    static class Paginator
    {
        public static int WordsNum = 0;

        public static List<Page> Pages = new List<Page>();
        private static int PageSize = 0;




        public static int nx, ny;

        public static void ArrangePage(double x, double y, double fontsize, int wordnum)
        {
            Pages.Clear();

            if (wordnum <= 0)
                return;


            nx = Convert.ToInt32(Math.Floor((x - 20) / (fontsize + 0) -0.5 ));
            ny = Convert.ToInt32(Math.Floor((y - 10 ) / (fontsize * (Config.GetDouble("行距") + 1))) ) ;

            //至少三行一列
            if (nx < 3)
                nx = 3;
            if (ny < 3)
                ny = 3;


            PageSize = nx * ny;
            if (Config.GetBool("允许滚动"))
                PageSize = 1000;
            //首页


            int headsize = 2;
            int footsize = nx /2 ;
            int bodysize = PageSize - headsize - footsize;


            if (wordnum <= PageSize) //单页
            {
                Page p0 = new Page();
                p0.BodyStart = 0;
                p0.BodyEnd = wordnum - 1;

                Pages.Add(p0);
            }
            else
            {
                //第一页
                Page p0 = new Page();
                p0.BodyStart = 0;
                p0.BodyEnd = bodysize + headsize - 1;
                p0.FootStart = p0.BodyEnd + 1;
                p0.FootEnd = Math.Min(p0.FootStart + footsize - 1, wordnum - 1);


                Pages.Add(p0);

                for (int i = 1; i < wordnum; i++)
                {


                    Page p = new Page();
                    p.HeadStart = Pages[i - 1].BodyEnd - headsize + 1;
                    p.HeadEnd = Pages[i - 1].BodyEnd;



                    if (p.HeadStart + PageSize - 1 >= wordnum - 1) //最后一页
                    {
                        p.BodyStart = p.HeadEnd + 1;
                        p.BodyEnd = wordnum - 1;
                        Pages.Add(p);
                        break;
                    }
                    else
                    {
                        p.BodyStart = p.HeadEnd + 1;
                        p.BodyEnd = p.BodyStart + bodysize - 1;
                        p.FootStart = p.BodyEnd + 1;
                        p.FootEnd = Math.Min(p.FootStart + footsize - 1, wordnum - 1);
                        Pages.Add(p);
                    }


                }

            }



        }

        public static Tuple<int, int> GetRenderRange(int index)
        {
            int start = -1;
            int end = -1;




            int pagenum = GetPageNum(index);
            if (pagenum >= 0)
            {
                Page p = Pages[pagenum];

                if (p.HeadStart >= 0)
                    start = p.HeadStart;
                else
                    start = p.BodyStart;

                if (p.FootEnd >= 0)
                    end = p.FootEnd;
                else
                    end = p.BodyEnd;

            }

            return new Tuple<int, int>(start, end);


        }
        public static int GetPageNum(int index)
        {


            int pos = -1;

            for (int i = 0; i < Pages.Count; i++)
            {

                if (index >= Pages[i].BodyStart && index <= Pages[i].BodyEnd)
                {
                    pos = i;
                }
            }


            return pos;
        }
    }
}
