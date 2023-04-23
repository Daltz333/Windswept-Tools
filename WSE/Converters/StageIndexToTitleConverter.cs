using Avalonia.Data.Converters;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSE.Models;

namespace WSE.Converters
{
    public class StageIndexToTitleConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int val)
            {
                // Ensure we can index
                if (GameSaveUtil.StageNameLookupTable.Length - 1 >= val)
                {
                    return GameSaveUtil.StageNameLookupTable[val];
                } else
                {
                    return val.ToString();
                }
            } else
            {
                return "null";
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string val)
            {
                var index = GameSaveUtil.StageNameLookupTable.IndexOf(val);
                if (index != -1) {
                    return index;
                } else
                {
                    // Reverse lookup failed, convert back to int if possible
                    if (int.TryParse(val, out var result))
                    {
                        return result;
                    } else
                    {
                        return 0;
                    }
                }
            } else
            {
                return 0;
            }
        }
    }
}
