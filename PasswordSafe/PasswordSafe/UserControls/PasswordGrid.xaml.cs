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
using System.Diagnostics;
using PasswordSafe.Data;
using PasswordSafe.Classes;
using System.Collections.ObjectModel;
using PasswordSafe.Data.Biz;
using Odyssey.Controls;
using System.Threading;
using PasswordSafe.Controls;

namespace PasswordSafe.UserControls
{
    /// <summary>
    /// Interaction logic for PasswordGrid.xaml
    /// </summary>
    public partial class PasswordGrid : UserControl
    {
        public PasswordGrid()
        {
            InitializeComponent();
           CommandBindings.Add(new CommandBinding(FocusSearchBoxCommand, FocusSearchBox));
        }

        static PasswordGrid()
        {
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(FocusSearchBoxCommand, FocusSearchBoxStatic, CanFocusSearchBox));
        }



        /// <summary>
        /// Gets whether data in the datacontext has changed since last call of ResetChanges.
        /// </summary>
        public bool DataChanged
        {
            get { return (bool)GetValue(DataChangedProperty); }
            private set { SetValue(DataChangedProperty, value); }
        }

        public static readonly DependencyProperty DataChangedProperty =
            DependencyProperty.Register("DataChanged", typeof(bool), typeof(PasswordGrid), new UIPropertyMetadata(false));


        public Password SelectedPassword
        {
            get { return (Password)GetValue(SelectedPasswordProperty); }
            set { SetValue(SelectedPasswordProperty, value); }
        }

        public static readonly DependencyProperty SelectedPasswordProperty =
            DependencyProperty.Register("SelectedPassword", typeof(Password), typeof(PasswordGrid), new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedPasswordPropertyChanged));


        private static void OnSelectedPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PasswordGrid)d).OnSelectedPasswordChanged(d, (Password)e.OldValue, (Password)e.NewValue);
        }

        protected virtual void OnSelectedPasswordChanged(object sender, Password oldPassword, Password newPassword)
        {

        }


        /// <summary>
        /// Gets the password of the selected category including only those that match with the text typed in the Search box.
        /// </summary>
        public IEnumerable<Password> Passwords
        {
            get { return (IEnumerable<Password>)GetValue(PasswordsProperty); }
            set { SetValue(PasswordsProperty, value); }
        }

        public static readonly DependencyProperty PasswordsProperty =
            DependencyProperty.Register("Passwords", typeof(IEnumerable<Password>), typeof(PasswordGrid), new UIPropertyMetadata(null));



        /// <summary>
        /// Gets or sets the selected node of the breadcrumb.
        /// </summary>
        public NodeBase SelectedNode
        {
            get { return (NodeBase)GetValue(SelectedNodeProperty); }
            set { SetValue(SelectedNodeProperty, value); }
        }

        public static readonly DependencyProperty SelectedNodeProperty =
            DependencyProperty.Register("SelectedNode", typeof(NodeBase), typeof(PasswordGrid), new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedNodePropertyChanged));

        private static void OnSelectedNodePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PasswordGrid)d).OnSelectedNodeChanged((NodeBase)e.OldValue, (NodeBase)e.NewValue);
        }

        protected virtual void OnSelectedNodeChanged(NodeBase oldNode, NodeBase newNode)
        {
            PasswordsListView.SelectedIndex = 0;
            UpdatePasswords(newNode);
            if (newNode != null)
            {
                newNode.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(newNode_PropertyChanged);
            }
            if (oldNode != null)
            {
                oldNode.PropertyChanged -= newNode_PropertyChanged;
            }

        }

        private void UpdatePasswords(NodeBase newNode)
        {
            Category c = newNode as Category;
            if (c != null)
            {
                SetPasswords(c.NestedPasswords);
            }
            else
            {
                Folder f = newNode as Folder;
                SetPasswords(f != null ? f.NestedPasswords : null);
            }
        }

        void newNode_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "NestedPasswords")
            {
                UpdatePasswords(SelectedNode);
            }
        }

        private Thread filterThread;

        private void SetPasswords(IEnumerable<Password> passwords)
        {
            if (filterThread != null)
            {
                filterThread.Abort();
                filterThread.Join();
            }

            if (!string.IsNullOrEmpty(SearchText))
            {
                filterThread = new Thread(delegate(object text)
                    {
                        passwords = FilteredPasswords(passwords, text as string);
                        this.Dispatcher.Invoke((ThreadStart)delegate()
                        {
                            Passwords = passwords != null ? new ObservableCollection<Password>(passwords) : null;
                            if (Passwords != null) SelectedPassword = Passwords.FirstOrDefault();
                        });
                    });
                filterThread.Start(SearchText);
            }
            else
            {
                Passwords = passwords != null ? new ObservableCollection<Password>(passwords) : null;
                if (Passwords != null) SelectedPassword = Passwords.FirstOrDefault();
            }

        }

        private IEnumerable<Password> FilteredPasswords(IEnumerable<Password> passwords, string text)
        {
            if (passwords != null)
            {
                return string.IsNullOrEmpty(text) ? passwords : passwords.Where(p => MatchFilter(p, text));
            }
            else
            {
                return null;
            }
        }

        private bool MatchFilter(Password password, string text)
        {
            if (password.Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0) return true;
            foreach (Field field in password.Fields)
            {
                string value = field.StringValue;
                if (value!=null && value.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0) return true;
            }
            return false;
        }


        internal void FocusPassword()
        {
            this.PasswordName.Focus();
            this.PasswordName.SelectAll();
        }

        private void OnCategoryBreadcrumbItemSelected(object sender, RoutedPropertyChangedEventArgs<BreadcrumbItem> e)
        {
            if (e.NewValue != null)
            {
                NodeBase node = e.NewValue.Data as NodeBase;
                SelectedNode = node;
            }
        }

        public NodeBase BreadcrumbRoot
        {
            get { return (NodeBase)GetValue(BreadcrumbRootProperty); }
            set { SetValue(BreadcrumbRootProperty, value); }
        }

        public static readonly DependencyProperty BreadcrumbRootProperty =
            DependencyProperty.Register("BreadcrumbRoot", typeof(NodeBase), typeof(PasswordGrid), new UIPropertyMetadata(null, OnBreadcrumbRootChanged));

        private static void OnBreadcrumbRootChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PasswordGrid)d).OnBreadcrumbRootChanged((NodeBase)e.OldValue, (NodeBase)e.NewValue);
        }

        protected virtual void OnBreadcrumbRootChanged(NodeBase oldNode, NodeBase newNode)
        {
            BreadcrumbHierarchy = UIContext.GetBreadcrumbDropDownData(newNode);
            CategoryHierarchy = UIContext.GetBreadcrumbDropDownData(BizContext.Instance.Categories.FirstOrDefault());
        }


        public Category RootCategory
        {
            get { return (Category)GetValue(RootCategoryProperty); }
            set { SetValue(RootCategoryProperty, value); }
        }

        public static readonly DependencyProperty RootCategoryProperty =
            DependencyProperty.Register("RootCategory", typeof(Category), typeof(PasswordGrid), new UIPropertyMetadata(null));



        public IEnumerable<string> BreadcrumbHierarchy
        {
            get { return (IEnumerable<string>)GetValue(BreadcrumbHierarchyProperty); }
            set { SetValue(BreadcrumbHierarchyProperty, value); }
        }

        public static readonly DependencyProperty BreadcrumbHierarchyProperty =
            DependencyProperty.Register("BreadcrumbHierarchy", typeof(IEnumerable<string>), typeof(PasswordGrid), new UIPropertyMetadata(null));




        public IEnumerable<string> CategoryHierarchy
        {
            get { return (IEnumerable<string>)GetValue(CategoryHierarchyProperty); }
            set { SetValue(CategoryHierarchyProperty, value); }
        }

        public static readonly DependencyProperty CategoryHierarchyProperty =
            DependencyProperty.Register("CategoryHierarchy", typeof(IEnumerable<string>), typeof(PasswordGrid), new UIPropertyMetadata(null));


        /// <summary>
        /// Gets or sets the text for the search textbox.
        /// </summary>
        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(PasswordGrid), new UIPropertyMetadata("", OnSearchTextPropertyChanged));

        private static void OnSearchTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PasswordGrid)d).OnSearchTextChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void OnSearchTextChanged(string oldValue, string newValue)
        {
            SelectedNode = new UIContext().RootCategory;
            bool hasText = !string.IsNullOrEmpty(newValue);
            delBtn.Visibility = hasText ? Visibility.Visible : Visibility.Collapsed;
            fndBtn.Visibility = !hasText ? Visibility.Visible : Visibility.Collapsed;
            UpdatePasswords(SelectedNode);
        }

        private void OnSearchBoxDelBtnClick(object sender, RoutedEventArgs e)
        {
            SearchText = "";
        }


        public static readonly RoutedUICommand FocusSearchBoxCommand = new RoutedUICommand("Focus", "FocusSearchBoxCommand", typeof(PasswordGrid));

        public void FocusSearchBox(object sender, ExecutedRoutedEventArgs e)
        {
            searchBox.Focus();
            searchBox.SelectAll();
        }



        public static void FocusSearchBoxStatic(object sender, ExecutedRoutedEventArgs e)
        {
            ((Main)sender).PasswordGrid.FocusSearchBox(sender, e);
        }

        public static void CanFocusSearchBox(object sender, CanExecuteRoutedEventArgs e)
        {
            Main main = (Main)sender;
            e.CanExecute = (main.DisplayType == DisplayType.Passwords && main.DisplayMode == DisplayMode.Passwords);
        }

    }
}
