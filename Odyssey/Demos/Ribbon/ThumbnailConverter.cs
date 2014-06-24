using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Odyssey.Controls;

namespace Demos
{
    public class ThumbnailConverter:IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            RibbonThumbnail tn = value as RibbonThumbnail;
            if (tn == null) return value;

            string s = System.IO.Path.GetFileName(tn.ImageSource.ToString());
            return s;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
