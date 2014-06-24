using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using PasswordSafe.Data.Biz;

namespace PasswordSafe.Converter
{
    public class CopyFieldConverter:IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            IEnumerable<Field> fields = value as IEnumerable<Field>;
            if (fields == null) return Binding.DoNothing;

            return fields.Where(f => f.Type != FieldType.Separator);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
