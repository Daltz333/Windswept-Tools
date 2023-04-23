using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSE.Models;

namespace WSE.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType().IsEnum)
            {
                return ((Enum)value).ToString();
            }

            return "Null";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string val)
            {
                switch (val)
                {
                    case (nameof(HardmodeState.None)):
                        return HardmodeState.None;
                    case (nameof(HardmodeState.Turtle)):
                        return HardmodeState.Turtle;
                    case (nameof(HardmodeState.Duck)):
                        return HardmodeState.Turtle;
                }
            }

            return HardmodeState.None;
        }
    }
}
