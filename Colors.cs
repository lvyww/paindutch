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

    



    }
}
