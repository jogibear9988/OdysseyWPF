using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using PasswordSafe.Classes;
using System.Windows;
using PasswordSafe.Data.Biz;

namespace PasswordSafe.Converter
{
    /// <summary>
    /// A Value Converter that expects a FrameworkElement as Value that has a DataContext of Password.
    /// It returns an ImageSource that displays the state of Password.IsFavorite.
    /// </summary>
    public class FavoriteImageConverter:IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Password password = value as Password;
            //bool isFavorite = password != null && password.IsFavorite;

            bool isFavorite = (value!=null) && (bool)value;

            int size = (parameter == null) ? 32 : int.Parse(parameter as string);

            string name =  isFavorite ? "Favorites_" : "NoFaves";
            return new BitmapImage(new Uri(string.Format("pack://application:,,,/img/{0}{1}.png", name, size)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
