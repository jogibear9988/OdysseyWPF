using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Odyssey.Controls.Ribbon.Interfaces;
using PasswordSafe.Properties;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Windows.Data;

namespace Odyssey.Controls
{
    public class RibbonSrc : FrameworkElement
    {
        private static string formatString = "pack://application:,,,/img/{0}_{1}.png";

        /// <summary>
        /// Gets or sets the Format String to convert a Name value to a path the contains the image.
        /// Warning: this value has a global scope!
        /// </summary>
        public string FormatString
        {
            get { return formatString; }
            set { formatString = value; }
        }


        public static string GetName(DependencyObject obj)
        {
            return (string)obj.GetValue(NameProperty);
        }

        /// <summary>
        /// Set the name of an image to attach to an IRibbonButton control.
        /// </summary>
        public static void SetName(DependencyObject obj, string value)
        {
            obj.SetValue(NameProperty, value);
        }

        public static new readonly DependencyProperty NameProperty =
            DependencyProperty.RegisterAttached("Name", typeof(string), typeof(RibbonSrc), new UIPropertyMetadata(null, NamePropertyChanged));

        private static void NamePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            string value = e.NewValue as string;

            IRibbonButton btn = o as IRibbonButton;
            if (btn != null)
            {
                btn.SmallImage = CreateImageSource(value, 16);
                btn.LargeImage = CreateImageSource(value, 32);
                return;
            }
            
            RibbonApplicationMenuItem apItem = o as RibbonApplicationMenuItem;
            if (apItem != null)
            {
                apItem.Image = CreateImageSource(value, 32);
                return;
            }

            RibbonMenuItem item = o as RibbonMenuItem;
            if (item != null)
            {
                item.Image = CreateImageSource(value, 16);
                return;
            }

            RibbonComboBox box = o as RibbonComboBox;
            if (box != null)
            {
                box.Image = CreateImageSource(value, 16);
                return;
            }

            RibbonTextBox tb = o as RibbonTextBox;
            if (tb != null)
            {
                tb.Image = CreateImageSource(value, 16);
                return;
            }

            throw new ArgumentNullException("RibbonSrc.Name can only by attached to controls that implement IRibbonButton.");
        }

        private static ImageSource CreateImageSource(string value, int size)
        {
            string s;
            if (size == 16 && value.Equals("favorites", StringComparison.InvariantCultureIgnoreCase)) s = "pack://application:,,,/img/faves16.png";
            else s = string.Format(formatString, value, size);
            BitmapImage img = new BitmapImage(new Uri(s));
            return img;
        }
    }

}
