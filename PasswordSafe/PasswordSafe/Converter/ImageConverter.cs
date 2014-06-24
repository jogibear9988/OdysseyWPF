using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using PasswordSafe.Data.Biz;

namespace PasswordSafe.Converter
{
    /// <summary>
    /// Gets the Images for the breadcrumbs from a Node.
    /// </summary>
    public class ImageConverter:IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string image = "pack://application:,,,/img/category_16.png";
            CustomNode node = value as CustomNode;

            if (node != null) image = "pack://application:,,,/img/home_16.png";
            else
            {
                Category category = value as Category;
                Folder folder = value as Folder;

                if (category != null ) image = "pack://application:,,,/img/category_16.png";
                else if (folder != null) image = "pack://application:,,,/img/faves16.png";
            }
            return new BitmapImage(new Uri(image));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
