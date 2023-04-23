using Avalonia;
using Avalonia.Data.Converters;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSE.Converters
{
    public class IntToBooleanConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int val)
            {
                return val == 0 ? false : true;
            } else if (value is UnsetValueType)
            {
                return value;
            }
            {
                Log.Warning($"{nameof(IntToBooleanConverter)} failed to convert {value}!");
                return false;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool val)
            {
                return val ? 2 : 0;
            } else
            {
                Log.Warning($"{nameof(IntToBooleanConverter)} failed to convertback {value}!");
                return false;
            }
        }
    }
}
