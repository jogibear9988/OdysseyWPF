using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using PasswordSafe.Classes;
using System.Windows;
using System.ComponentModel;
using PasswordSafe.Data.Biz;

namespace PasswordSafe.DataTemplateSelectors
{
    public class BreadcrumbItemSelector : DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (!DesignerProperties.GetIsInDesignMode(container))
            {
                Window window = Application.Current.MainWindow;

                CustomNode node = item as CustomNode;
                if (node != null) return window.FindResource("bcBreadcrumbItemTemplate") as DataTemplate;

                Category category = item as Category;
                if (category != null) return window.FindResource("bcCategoryItemTemplate") as DataTemplate;

                Folder folder = item as Folder;
                if (folder != null) return window.FindResource("bcFolderItemTemplate") as DataTemplate;
            }


            return base.SelectTemplate(item, container);
        }
    }
}
