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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Odyssey.Controls;
using System.Windows.Media.Animation;

namespace Demos
{
    //BUGFIX: Corrected a glitch when the SubItems of a Breadrumb where reopened. This caused the SelectedItem to be set to null and therefore alls trails after the breadcrumb where removed.

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ExplorerBar : Window
    {
        public ExplorerBar()
        {
            InitializeComponent();
        }

        #region ExplorerBar
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Background = Brushes.LightSteelBlue;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            text3.Visibility = text3.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            text1.Visibility = text1.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }


        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            expander1.IsMinimized ^= true;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            expander2.IsMinimized ^= true;
        }

        private void Animate1Click(object sender, RoutedEventArgs e)
        {
            expander2.IsExpanded ^= true;
            expander1.IsMinimized ^= true;

        }
        private void Animate2Click(object sender, RoutedEventArgs e)
        {
            //text2.Visibility = text2.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            expander1.IsExpanded ^= true;
            expander2.IsExpanded ^= true;
        }
        #endregion

        #region BreadcrumbBar
        /// <summary>
        /// A BreadcrumbItem needs to populate it's Items. This can be due to the fact that a new BreadcrumbItem is selected, and thus
        /// it's Items must be populated to determine whether this BreadcrumbItem show a dropdown button,
        /// or when the Path property of the BreadcrumbBar is changed and therefore the Breadcrumbs must be populated from the new path.
        /// </summary>
        private void BreadcrumbBar_PopulateItems(object sender, Odyssey.Controls.BreadcrumbItemEventArgs e)
        {
            BreadcrumbItem item = e.Item;
            if (item.Items.Count == 0)
            {
                PopulateFolders(item);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Gets a collection of all drives on the computer.
        /// </summary>
        private static IEnumerable<string> GetDrives(string separatorString)
        {
            int separatorLength = separatorString.Length;
            var folders = from drive in System.IO.Directory.GetLogicalDrives() select drive.EndsWith(separatorString) ? drive.Remove(drive.Length - separatorLength) : drive;
            return folders.AsEnumerable();
        }

        /// <summary>
        /// Populate the Items of the specified BreadcrumbItem with the sub folders if necassary.
        /// </summary>
        /// <param name="item"></param>
        private static void PopulateFolders(BreadcrumbItem item)
        {
            List<FolderItem> folderItems = GetFolderItemsFromBreadcrumb(item);
            item.ItemsSource = folderItems;
        }

        /// <summary>
        /// Gets a list of FolderItems that are the subfolders of the specified BreadcrumbItem.
        /// </summary>
        private static List<FolderItem> GetFolderItemsFromBreadcrumb(BreadcrumbItem item)
        {
            BreadcrumbBar bar = item.BreadcrumbBar;
            string path = bar.PathFromBreadcrumbItem(item);
            string trace = item.TraceValue;
            string[] subFolders;
            if (trace.Equals("Computer"))
            {
                subFolders = GetDrives(bar.SeparatorString).ToArray();
            }
            else
            {
                try
                {
                    subFolders = (from dir in System.IO.Directory.GetDirectories(path + "\\") select System.IO.Path.GetFileName(dir)).ToArray();
                }
                catch
                {
                    //maybe we don't have access!
                    subFolders = new string[] { };
                }
            }
            List<FolderItem> folderItems = (from folder in subFolders orderby folder select new FolderItem { Folder = folder }).ToList();
            return folderItems;
        }

        /// <summary>
        /// Convert the path from visual to logical or vice versa:
        /// </summary>
        private void BreadcrumbBar_PathConversion(object sender, PathConversionEventArgs e)
        {
            if (e.Mode == PathConversionEventArgs.ConversionMode.DisplayToEdit)
            {
                if (e.DisplayPath.StartsWith(@"Computer\", StringComparison.OrdinalIgnoreCase))
                {
                    e.EditPath = e.DisplayPath.Remove(0, 9);
                }
                else if (e.DisplayPath.StartsWith(@"Network\", StringComparison.OrdinalIgnoreCase))
                {
                    string editPath = e.DisplayPath.Remove(0, 8);
                    editPath = @"\\" + editPath.Replace('\\', '/');
                    e.EditPath = editPath;
                }
            }
            else
            {
                if (e.EditPath.StartsWith("c:", StringComparison.OrdinalIgnoreCase))
                {
                    e.DisplayPath = @"Desktop\Computer\" + e.EditPath;
                }
                else if (e.EditPath.StartsWith(@"\\"))
                {
                    e.DisplayPath = @"Desktop\Network\" + e.EditPath.Remove(0, 2).Replace('/', '\\');
                }
            }
        }

        /// <summary>
        /// Show a progress bar animation for demonstation purpose.
        /// </summary>
        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation(100, new Duration(new TimeSpan(0, 0, 2)));
            da.FillBehavior = FillBehavior.Stop;
            bar.BeginAnimation(BreadcrumbBar.ProgressValueProperty, da);
        }

        /// <summary>
        /// The dropdown menu of a BreadcrumbItem was pressed, so delete the current folders, and repopulate the folders
        /// to ensure actual data.
        /// </summary>
        private void bar_BreadcrumbItemDropDownOpened(object sender, BreadcrumbItemEventArgs e)
        {
            BreadcrumbItem item = e.Item;

            // only repopulate, if the BreadcrumbItem is dynamically generated which means, item.Data is a  pointer to itself:
            if (!(item.Data is BreadcrumbItem))
            {
                UpdateFolderItems(item);
            }
        }

        /// <summary>
        /// Update the list of Subfolders from a BreadcrumbItem.
        /// </summary>
        private void UpdateFolderItems(BreadcrumbItem item)
        {
            List<FolderItem> actualFolders = GetFolderItemsFromBreadcrumb(item);
            List<FolderItem> currentFolders = item.ItemsSource as List<FolderItem>;
            currentFolders.Clear();
            currentFolders.AddRange(actualFolders);

        }

        public void ShowStaticBreadcrumbBar(object sender, RoutedEventArgs e)
        {
            BreadcrumbDS ds = new BreadcrumbDS();
            ds.ShowDialog();
        }
        #endregion
    }
}
