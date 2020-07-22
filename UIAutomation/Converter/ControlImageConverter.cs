using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using UIAutomation.Common;

namespace UIAutomation.Converter
{
    public class ControlImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (SystemConfig.DicControlPng.Keys.Contains(value.ToString()))
            {
                return SystemConfig.DicControlPng[value.ToString()];
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
