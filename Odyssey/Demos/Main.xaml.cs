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
using Demos.Ribbon;

namespace Demos
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
        }

        private void OutlookClick(object sender, RoutedEventArgs e)
        {
            Outlook outlook = new Outlook();
            outlook.ShowDialog();
        }

        private void ExplorerBarClick(object sender, RoutedEventArgs e)
        {
            ExplorerBar explorerBar = new ExplorerBar();
            explorerBar.ShowDialog();
        }

        private void BreadcrumbBarClick(object sender, RoutedEventArgs e)
        {
            BreadcrumbDS bds = new BreadcrumbDS();
            bds.ShowDialog();
        }

        private void RibbonBarClick(object sender, RoutedEventArgs e)
        {
            RibbonDemo ribbonDemo = new RibbonDemo();
            ribbonDemo.Show();
        }
    }
}
