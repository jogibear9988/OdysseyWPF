using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using PasswordSafe.Classes;
using PasswordSafe.Data.Biz;

namespace PasswordSafe.Converter
{
    public class IsModifiedFromUIPasswordConverter:IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Password password = value as Password;
            if (password != null)
            {
                return password.IsModified;
            }
            else return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
