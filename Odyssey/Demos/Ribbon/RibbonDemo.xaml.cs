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
using Odyssey.Controls.Classes;
using Odyssey.Controls;

namespace Demos.Ribbon
{
    /// <summary>
    /// Interaction logic for RibbonDemo.xaml
    /// </summary>
    public partial class RibbonDemo : RibbonWindow
    {
        public RibbonDemo()
        {
            InitializeComponent();
        }

        private void Win7Click(object sender, RoutedEventArgs e)
        {
            SkinManager.SkinId = SkinId.Windows7;
        }

        private void VistaClick(object sender, RoutedEventArgs e)
        {
            SkinManager.SkinId = SkinId.Vista;
        }

        private void OfficeBlueClick(object sender, RoutedEventArgs e)
        {
            SkinManager.SkinId = SkinId.OfficeBlue;
        }

        private void OfficeSilverClick(object sender, RoutedEventArgs e)
        {
            SkinManager.SkinId = SkinId.OfficeSilver;
        }

        private void OfficeBlackClick(object sender, RoutedEventArgs e)
        {
            SkinManager.SkinId = SkinId.OfficeBlack;
        }

        private void Context1Click(object sender, RoutedEventArgs e)
        {
            ribbonBar.ContextualTabSet = ribbonBar.ContextualTabSets[0];
        }

        private void Context2Click(object sender, RoutedEventArgs e)
        {
            ribbonBar.ContextualTabSet = ribbonBar.ContextualTabSets[1];
        }

        private void ContextOffClick(object sender, RoutedEventArgs e)
        {
            ribbonBar.ContextualTabSet = null;
        }


        private void ShowBelowClick(object sender, RoutedEventArgs e)
        {
            ribbonBar.ToolbarPlacement = QAPlacement.Bottom;
        }

        private void ShowAboveClick(object sender, RoutedEventArgs e)
        {
            ribbonBar.ToolbarPlacement = QAPlacement.Top;
        }

        private void CloseDemoClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RibbonGroup_LaunchDialog(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Launcher");
        }

    }
}
