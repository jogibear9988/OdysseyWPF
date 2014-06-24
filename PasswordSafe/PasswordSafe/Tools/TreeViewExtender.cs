using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using PasswordSafe.Data.Biz;
using System.Windows.Media;
using System.Diagnostics;

namespace PasswordSafe.Tools
{
    //EXPLAIN:
    /// <summary>
    /// Extends a TreeView to modify the selected item.
    /// </summary>
    public static class TreeViewExtender
    {
        /// <summary>
        /// Deselects a selected item from a TreeView.
        /// </summary>
        /// <param name="treeView">The TreeView.</param>
        public static void Deselect(this TreeView treeView)
        {
            if (treeView.Items.Count > 0 && treeView.SelectedItem != null)
            {
                foreach (var o in treeView.Items)
                {
                    var item = treeView.ItemContainerGenerator.ContainerFromItem(o) as TreeViewItem;
                    item.IsSelected = false;
                    DeselectItems(item);
                }
            }
        }

        private static void DeselectItems(TreeViewItem item)
        {
            foreach (var o in item.Items)
            {
                var sub = item.ItemContainerGenerator.ContainerFromItem(o) as TreeViewItem;
                if (sub != null)
                {
                    sub.IsSelected = false;
                    DeselectItems(sub);
                }
            }
        }

        public static void SelectFirst(this TreeView treeView)
        {
            if (treeView.Items.Count > 0)
            {
                var item = treeView.ItemContainerGenerator.ContainerFromItem(treeView.Items[0]) as TreeViewItem;
                if (item != null) item.IsSelected = true;

            }
        }

        public static void Select(this TreeView treeView, int index)
        {
            object o = treeView.Items[index];
            TreeViewItem item = Select(treeView, o);
        }

        public static void Select(this TreeView treeView, Category category)
        {
            SelectItem(treeView, category);
        }


        private static void SelectItem(ItemsControl itemsControl, Category category)
        {
            ItemContainerGenerator gen = itemsControl.ItemContainerGenerator;
            if (gen.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                EventHandler eh = null;
                eh = new EventHandler(delegate
            {
                if (gen.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                {
                    gen.StatusChanged -= eh;
                    SelectItem(itemsControl, category);
                }
            });
                gen.StatusChanged += eh;
            }
            else
            {
                TreeViewItem root = gen.ContainerFromItem(category) as TreeViewItem;
                if (root != null)
                {
                    root.IsSelected = true;
                    return;
                }

                Category original = category;
                while (category != null)
                {
                    foreach (var item in itemsControl.Items)
                    {
                        TreeViewItem container = gen.ContainerFromItem(item) as TreeViewItem;
                        if (item == original)
                        {
                            if (container != null)
                            {
                                container.IsSelected = true;
                            }
                            return;
                        }
                        else
                        {
                            if (item == category)
                            {

                                if (container != null)
                                {
                                    container.IsExpanded = true;
                                    SelectItem(container, original);
                                }
                            }
                        }
                    }
                    category = category.Parent;
                }
            }
        }

        public static TreeViewItem Select(this TreeView treeView, object value)
        {
            if (treeView.SelectedItem == value) return null;

            TreeViewItem container = treeView.ItemContainerGenerator.ContainerFromItem(value) as TreeViewItem;
            if (container == null)
            {
                foreach (var item in treeView.Items)
                {
                    TreeViewItem c = treeView.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                    if (c != null)
                    {
                        container = Select(c, value);
                        if (container != null) break;
                    }
                }
            }
            if (container != null) container.IsSelected = true;
            return container;
        }

        private static TreeViewItem Select(TreeViewItem tvItem, object value)
        {
            TreeViewItem tv = tvItem.ItemContainerGenerator.ContainerFromItem(value) as TreeViewItem;
            if (tv != null)
            {
                tvItem.IsExpanded = true;
                return tv;
            }
            foreach (var item in tvItem.Items)
            {
                TreeViewItem c = tvItem.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (c != null)
                {
                    tv = Select(c, value);
                    if (tv != null)
                    {
                        tvItem.IsExpanded = true;
                        return tv;
                    }
                }

            }
            return null;
        }

        public static TreeViewItem FirstItem(this TreeView treeView)
        {
            if (treeView.Items.Count > 0)
            {
                object value = treeView.Items[0];
                TreeViewItem item = treeView.ItemContainerGenerator.ContainerFromItem(value) as TreeViewItem;
                return item;
            }
            else return null;
        }
    }
}
