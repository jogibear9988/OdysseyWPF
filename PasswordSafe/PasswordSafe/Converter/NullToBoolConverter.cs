using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using PasswordSafe.Data.Biz;

namespace PasswordSafe.Converter
{
    class NullToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return false;
            if ("HasLines".Equals(parameter)) return (value as Field).HasLines;
            if ("HasRange".Equals(parameter)) return (value as Field).HasRange;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
