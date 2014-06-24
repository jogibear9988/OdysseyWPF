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
using PasswordSafe.Classes;
using Odyssey.Controls;
using PasswordSafe.Data.Biz;

namespace PasswordSafe.UserControls
{
    /// <summary>
    /// Interaction logic for TemplateGrid.xaml
    /// </summary>
    public partial class TemplateGrid : UserControl
    {
        public TemplateGrid()
        {
            InitializeComponent();
        }

        private void edit_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

            //SelectableCategoryField field = (e.OriginalSource as Control).DataContext as SelectableCategoryField;
            //field.IsSelected = true;

        }

        private void OnCategoryBreadcrumbItemSelected(object sender, RoutedPropertyChangedEventArgs<BreadcrumbItem> e)
        {
            if (e.NewValue != null)
            {
                NodeBase node = e.NewValue.Data as NodeBase;
                SelectedNode = node;
            }
        }


        public Category SelectedCategory
        {
            get { return (Category)GetValue(SelectedCategoryProperty); }
            set { SetValue(SelectedCategoryProperty, value); }
        }

        public static readonly DependencyProperty SelectedCategoryProperty =
            DependencyProperty.Register("SelectedCategory", typeof(Category), typeof(TemplateGrid),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                SelectedCategoryPropertyChanged));


        private static void SelectedCategoryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TemplateGrid)d).OnSelectedCategoryChanged(d, (Category)e.OldValue, (Category)e.NewValue);

        }

        protected virtual void OnSelectedCategoryChanged(object sender, Category oldCategory, Category newCategory)
        {
        }

        /// <summary>
        /// Gets or sets the selected node of the breadcrumb.
        /// </summary>
        public NodeBase SelectedNode
        {
            get { return (NodeBase)GetValue(SelectedNodeProperty); }
            set { SetValue(SelectedNodeProperty, value); }
        }

        public static readonly DependencyProperty SelectedNodeProperty =
            DependencyProperty.Register("SelectedNode", typeof(NodeBase), typeof(TemplateGrid), new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedNodePropertyChanged));

        private static void OnSelectedNodePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TemplateGrid)d).OnSelectedNodeChanged((NodeBase)e.OldValue, (NodeBase)e.NewValue);
        }

        protected virtual void OnSelectedNodeChanged(NodeBase oldNode, NodeBase newNode)
        {
        }

        public NodeBase BreadcrumbRoot
        {
            get { return (NodeBase)GetValue(BreadcrumbRootProperty); }
            set { SetValue(BreadcrumbRootProperty, value); }
        }

        public static readonly DependencyProperty BreadcrumbRootProperty =
            DependencyProperty.Register("BreadcrumbRoot", typeof(NodeBase), typeof(TemplateGrid), new UIPropertyMetadata(null, OnBreadcrumbRootChanged));

        private static void OnBreadcrumbRootChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TemplateGrid)d).OnBreadcrumbRootChanged(d, (NodeBase)e.OldValue, (NodeBase)e.NewValue);
        }

        protected virtual void OnBreadcrumbRootChanged(object sender, NodeBase oldNode, NodeBase newNode)
        {
            BreadcrumbHierarchy = UIContext.GetBreadcrumbDropDownData(newNode);           
        }



        public IEnumerable<string> BreadcrumbHierarchy
        {
            get { return (IEnumerable<string>)GetValue(BreadcrumbHierarchyProperty); }
            set { SetValue(BreadcrumbHierarchyProperty, value); }
        }

        public static readonly DependencyProperty BreadcrumbHierarchyProperty =
            DependencyProperty.Register("BreadcrumbHierarchy", typeof(IEnumerable<string>), typeof(TemplateGrid), new UIPropertyMetadata(null));


    }
}
