using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace TypeB
{
    public class StringToFontFamily : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FontFamily fontfamily = (FontFamily)value;
            LanguageSpecificStringDictionary lsd = fontfamily.FamilyNames;
            if (lsd.ContainsKey(XmlLanguage.GetLanguage("zh-cn")))
            {
                string fontname = null;
                if (lsd.TryGetValue(XmlLanguage.GetLanguage("zh-cn"), out fontname))
                {
                    return fontname;
                }
            }
            else
            {
                string fontname = null;
                if (lsd.TryGetValue(XmlLanguage.GetLanguage("en-us"), out fontname))
                {
                    return fontname;
                }
            }
            return "Arial";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string fontname = (string)value;
            FontFamily fontfamily = new FontFamily(fontname);
            return fontfamily;
        }



    }
}
