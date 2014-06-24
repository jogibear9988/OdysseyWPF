using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using PasswordSafe.Controls;
using System.Windows;

namespace PasswordSafe.Converter
{
    public class DisplayTypeToVisibleConverter:IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DisplayType desiredType = (DisplayType)Enum.Parse(typeof(DisplayType), parameter.ToString());
            DisplayType type = (DisplayType)value;

            return type == desiredType ? Visibility.Visible : Visibility.Hidden;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
