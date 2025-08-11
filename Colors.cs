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
            Brushes.Gray,
            Brushes.ForestGreen,
            Brushes.LightSeaGreen,
            Brushes.RoyalBlue,
            Brushes.SlateBlue,
            Brushes.BlueViolet,
            Brushes.DeepPink,
            Brushes.Crimson,

        };





        public static SolidColorBrush GetSpeedColor(double speed)
        {

            if (speed >= 200)
                return Levels[7];
            else if (speed >= 180)
                return Levels[6];
            else if (speed >= 160)
                return Levels[5];
            else if (speed >= 140)
                return Levels[4];
            else if (speed >= 120)
                return Levels[3];
            else if (speed >= 100)
                return Levels[2];
            else if (speed >= 80)
                return Levels[1];
            else
                return Levels[0];

        }

    



    }
}
