using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;


namespace TypeB
{
    static internal class Colors
    {

        public static SolidColorBrush DisplayForeground;
        public static SolidColorBrush CorrectBackground;
        public static SolidColorBrush IncorrectBackground;

        public static SolidColorBrush FromString (string str)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + str));

        }

        public static SolidColorBrush[] Levels =
        {
            Brushes.Crimson,
            Brushes.HotPink,
            Brushes.BlueViolet,
            Brushes.RoyalBlue,
            Brushes.ForestGreen,
            Brushes.Gray
        };

        public static SolidColorBrush GetAccColor(double? accuracy)
        {
            if (accuracy == null)
            {
                return null;
            }

            int index = (int)(Score.GetAccuracy() * 100) - 94;

            if (index < 0) index = 0;
            if (index > 5) index = 5;

            index = 5 - index;

            return Levels[index];
        }

        public static SolidColorBrush GetSpeedColor(double speed)
        {
            if (double.IsNaN(speed) || speed <= 0)
            {
                return Brushes.Gray;
            }

            // 根据速度值返回不同颜色
            // 0-50: 灰色 (慢)
            // 50-100: 紫色
            // 100-150: 绿色
            // 150-200: 黄色
            // 200+: 红色 (快)
            if (speed < 50)
                return Brushes.Black;
            else if (speed < 100)
                return Brushes.Purple;
            else if (speed < 150)
                return Brushes.Green;
            else if (speed < 200)
                return Brushes.Yellow;
            else
                return Brushes.Crimson;
        }

    



    }
}
