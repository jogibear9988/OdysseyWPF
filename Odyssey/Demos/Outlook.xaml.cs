using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Odyssey.Controls;
using Odyssey.Controls.Classes;

namespace Demos
{
    /// <summary>
    /// Interaction logic for Outlook.xaml
    /// </summary>
    public partial class Outlook : Window
    {
        public Outlook()
        {
            InitializeComponent();
        }


        private void OutlookBarIncClick(object sender, RoutedEventArgs e)
        {
            bar.MaxNumberOfButtons++;
        }

        private void OutlookBarDecClick(object sender, RoutedEventArgs e)
        {
            bar.MaxNumberOfButtons--;
        }

        private void bar_SelectedSectionChanged(object sender, RoutedPropertyChangedEventArgs<Odyssey.Controls.OutlookSection> e)
        {
            if (IsInitialized)
            {
                label.Text = e.NewValue.Header.ToString();
            }
        }


        private void BlueSkinClick(object sender, RoutedEventArgs e)
        {
            SkinManager.SkinId = SkinId.OfficeBlue;
           
        }

        private void SilverSkinClick(object sender, RoutedEventArgs e)
        {
            SkinManager.SkinId = SkinId.OfficeSilver;
        }

        private void BlackSkinClick(object sender, RoutedEventArgs e)
        {
            SkinManager.SkinId = SkinId.OfficeBlack;
        }


    }
}
