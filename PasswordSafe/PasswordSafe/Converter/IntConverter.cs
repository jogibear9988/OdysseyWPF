using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;

namespace PasswordSafe.Converter
{
    public class IntConverter:IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int ival = (int)value;
            return ival.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int ival;
            if (!int.TryParse(value as string, out ival)) ival = default(int);
            return ival;
        }

        #endregion
    }
    public class Rule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            //xx
            throw new NotImplementedException();
        }
    }
}
