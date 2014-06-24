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
using System.Diagnostics;
using Odyssey.Controls.Classes;
using PasswordSafe.Tools;
using System.Threading;
using PasswordSafe.Data;
using PasswordSafe.Properties;
using System.Windows.Media.Effects;
using Odyssey.Ribbon.EventArgs;
using PasswordSafe.Classes;
using PasswordSafe.Data.Biz;
using PasswordSafe.Converter;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Reflection;
using System.Data.SqlServerCe;
using System.Globalization;

namespace PasswordSafe.Controls
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Main : RibbonWindow
    {
        public Main()
        {
            //BizContext.ConnectionString = Settings.Default.PasswordsConnectionString;
            InitializeComponent();
            LoadSettings();
            ribbon.ApplicationMenu.Opened += new EventHandler(OnMenuOpened);
            IsModified = true;
            IsModified = false;

        }

        /// <summary>
        /// Get all categories when the menu is opened to select one for a new password:
        /// </summary>
        void OnMenuOpened(object sender, EventArgs e)
        {
        }

        static Main()
        {
            // Register a static MouseDownEvent  for a TreeViewItem to enable select with a mouse-right-click:
            EventManager.RegisterClassHandler(typeof(TreeViewItem),
                Mouse.MouseDownEvent, new MouseButtonEventHandler(OnTreeViewItemMouseDown), false);
        }

        private static void OnTreeViewItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as TreeViewItem;
            if (e.RightButton == MouseButtonState.Pressed)
            {
                item.IsSelected = true;
                e.Handled = true;
            }
        }


        private void LoadData(string connectionString)
        {
            BizContext.OpenConnection(connectionString);
            RefreshData();
            InitialShow();
            connectionString = "";
            SelectedNode = BizContext.Instance.RootCategory;
        }

        private void UnloadData()
        {
            BizContext.CloseConnection();
            RefreshData();
        }

        private void RefreshData()
        {
            UIContext context = new UIContext();
            PasswordGrid.BreadcrumbRoot = context.GetBreadcrumbRoot();

            TemplateGrid.BreadcrumbRoot =
            PasswordGrid.RootCategory = context.GetCategoryRoot();

            foreach (string res in new string[] {
                "fieldTypesData"
                ,"categoryData"
                ,"folderData"
            })
            {
                ObjectDataProvider provider = Resources[res] as ObjectDataProvider;
                if (provider != null)
                {
                    provider.InitialLoad();
                    provider.Refresh();
                }
            }

        }

        private void LoadSettings()
        {
            Settings settings = Settings.Default;
            outlook.MaxNumberOfButtons = settings.MaxSections;
            outlook.IsMaximized = settings.IsSidebarExpanded;
            ribbon.CanMinimize = settings.CanRibbonMinimize;
            this.PasswordGrid.PasswordColumn.Width = new GridLength(settings.PasswordWith);
            ribbon.IsExpanded = settings.IsRibbonExpanded && ribbon.CanMinimize;
            if (settings.Left != -1) Left = settings.Left;
            if (settings.Top != -1) Top = settings.Top;
            if (settings.Width > 48) Width = settings.Width;
            if (settings.Height > 32) Height = settings.Height;
            //    IsGlassEnabled = settings.IsGlassEnabled;
            Skin = settings.Skin;
        }

        private void SaveSettings()
        {
            Settings settings = Settings.Default;
            settings.IsSidebarExpanded = outlook.IsMaximized;
            settings.IsRibbonExpanded = ribbon.IsExpanded;
            settings.CanRibbonMinimize = ribbon.CanMinimize;
            Rect bounds = RestoreBounds;
            settings.Left = bounds.Left;
            settings.Top = bounds.Top;
            settings.Width = bounds.Width;
            settings.Height = bounds.Height;
            settings.WindowState = WindowState;
            settings.Skin = Skin;
            settings.MaxSections = outlook.MaxNumberOfButtons;
            settings.IsGlassEnabled = IsGlassEnabled;
            settings.PasswordWith = this.PasswordGrid.PasswordColumn.ActualWidth;
            settings.Save();
        }



        protected override void OnContentRendered(EventArgs e)
        {
            WindowState = Settings.Default.WindowState;
            IsGlassEnabled = Settings.Default.IsGlassEnabled;

            base.OnContentRendered(e);
            lockScreen.FocusPassword();
        }

        private void InitialShow()
        {
            TreeViewItem item = this.categoriesTree.FirstItem();
            if (item != null)
            {
                item.IsSelected = true;
                item.IsExpanded = true;
            }

            item = this.foldersTreeView.FirstItem();
            if (item != null)
            {
                item.IsExpanded = true;
            }
            SelectedCategory = BizContext.Instance.RootCategory;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            BizContext.Instance.Save();
            SaveSettings();
            this.ribbon.Focus();
            BizContext.Instance.Commit();
            base.OnClosing(e);
        }


#if false
        private void InitializeDataAsync()
        {
            var ctx = SynchronizationContext.Current;
            ThreadPool.QueueUserWorkItem(state =>
            {
                // this runs in a separate thread:
                var bcRoot = Domain.Nodes;
                var categories = new CategoryBase[] { Domain.RootCategory };
                var rootFolders = new NodeBase[] { Domain.RootFolder };
                var rootCategory = Domain.RootCategory;

                ctx.Post(ctxState =>
                {
                    // this runs in the application thread:
                    categoriesTree.SelectFirst();
                }, null);
            });
        }
#endif

        protected void OnSelectedCategoryChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Category c = e.NewValue as Category;
            SelectedCategory = c;
        }

        protected void OnSelectedFolderChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Folder folder = e.NewValue as Folder;
            SelectedFolder = folder;
        }



        /// <summary>
        /// Occurs when the Category TreeView got the focus.
        /// </summary>
        private void OnCategoriesGotFocus(object sender, RoutedEventArgs e)
        {
            if (this.categoriesTree.SelectedItem == null) categoriesTree.Select(new UIContext().RootCategory);
            ChangedTabsVisibility();
            if (ribbon.SelectedTabItem == folderTab) ribbon.SelectedTabItem = categoryTab;
        }

        /// <summary>
        /// Occurs when the Favorites TreeView got the focus.
        /// </summary>
        private void OnFavoritesGotFocus(object sender, RoutedEventArgs e)
        {
            if (this.foldersTreeView.SelectedItem == null) foldersTreeView.Select(new UIContext().RootFolder);
            ChangedTabsVisibility();
            if (ribbon.SelectedTabItem == categoryTab) ribbon.SelectedTabItem = folderTab;
        }

        private void ChangedTabsVisibility()
        {
            if (ribbon.ContextualTabSet != SidebarTabs)
            {
                ribbon.ContextualTabSet = null;
                ribbon.ContextualTabSet = SidebarTabs;
            }
        }


        /// <summary>
        /// Occurs when the Password Area (Passwords List, Password Details, Search, Breadcrumb,etc.) got the focus.
        /// </summary>
        private void OnPasswordsGotFocus(object sender, RoutedEventArgs e)
        {
            ribbon.ContextualTabSet = null;
        }


        /// <summary>
        /// Gets whether the currently selected password has been modified.
        /// </summary>
        public bool IsTemplateModified
        {
            get { return (bool)GetValue(IsTemplateModifiedProperty); }
            private set { SetValue(IsTemplateModifiedPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey IsTemplateModifiedPropertyKey =
            DependencyProperty.RegisterReadOnly("IsTemplateModified", typeof(bool), typeof(Main), new UIPropertyMetadata(false, TemplateModifiedPropertyChanged));

        private static readonly DependencyProperty IsTemplateModifiedProperty = IsTemplateModifiedPropertyKey.DependencyProperty;

        private static void TemplateModifiedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Main)d).TemplateModified((bool)e.NewValue);
        }

        protected virtual void TemplateModified(bool newValue)
        {
            CheckIsModified(newValue);
        }

        private void CheckIsModified(bool value)
        {
            if (value)
            {
                IsModified = true;
            }
            else
            {
                BizContext context = BizContext.Instance;
                Category root = context.Categories.FirstOrDefault();
                if (root != null)
                {
                    if (root.NestedPasswords.Any(p => p.IsModified))
                    {
                        IsModified = true;
                        return;
                    }
                    if (IsNestedTemplateModified(root))
                    {
                        IsModified = true;
                        return;
                    }
                }
                IsModified = false;
            }
        }

        private bool IsNestedTemplateModified(Category category)
        {
            if (category.IsModified) return true;
            return category.Categories.Any(c => IsNestedTemplateModified(c));
        }

        /// <summary>
        /// Gets or sets whether either a template or password is modified.
        /// </summary>
        public bool IsModified
        {
            get { return (bool)GetValue(IsModifiedProperty); }
            set { SetValue(IsModifiedProperty, value); }
        }

        public static readonly DependencyProperty IsModifiedProperty =
            DependencyProperty.Register("IsModified", typeof(bool), typeof(Main), new UIPropertyMetadata(false, ModifiedPropertyChanged));

        private static void ModifiedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Main)d).ModifiedChanged((bool)e.NewValue);
        }

        protected virtual void ModifiedChanged(bool newValue)
        {
            UpdateMenuItemEnabledStates(newValue);
        }

        //TODO:
        /// <summary>
        /// There is currently a bug that Command Bindings to MenuItems in the RibbonApplicatioMenu do not update the IsEnabled state, so this is done handcrafted:
        /// </summary>
        private void UpdateMenuItemEnabledStates(bool isEnabled)
        {
            SaveAllMenuItem.IsEnabled = isEnabled;
            UndoAllMenuItem.IsEnabled = isEnabled;
        }



        /// <summary>
        /// Gets whether the currently selected password has been modified.
        /// </summary>
        public bool IsPasswordModified
        {
            get { return (bool)GetValue(IsPasswordModifiedProperty); }
            private set { SetValue(IsPasswordModifiedPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey IsPasswordModifiedPropertyKey =
            DependencyProperty.RegisterReadOnly("IsPasswordModified", typeof(bool), typeof(Main), new UIPropertyMetadata(false, PasswordModifiedPropertyChanged));

        private static void PasswordModifiedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Main)d).PasswordModified((bool)e.NewValue);
        }

        protected virtual void PasswordModified(bool newValue)
        {
            CheckIsModified(newValue);
        }

        private static readonly DependencyProperty IsPasswordModifiedProperty = IsPasswordModifiedPropertyKey.DependencyProperty;



        private void OnSelectedTabIndexChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {

            RibbonTabItem selectedTab = ribbon.SelectedTabItem;
            DisplayMode = selectedTab == categoryTab ? DisplayMode.Templates : DisplayMode.Passwords;
        }


        private void SelectCategoryTreeViewItemClicked(object sender, RoutedEventArgs e)
        {
            //ClickableTreeViewItem item = e.OriginalSource as ClickableTreeViewItem;
            //WPFNode node = item.DataContext as WPFNode;

            //MessageBox.Show("Clicked: " + node.Path);
        }


        public DisplayMode DisplayMode
        {
            get { return (DisplayMode)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof(DisplayMode), typeof(Main),
            new FrameworkPropertyMetadata(
                DisplayMode.Passwords,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                DisplayModePropertyChanged
                ));


        private static void DisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Main)d).OnDisplayModeChanged(d, (DisplayMode)e.OldValue, (DisplayMode)e.NewValue);
        }

        protected virtual void OnDisplayModeChanged(object sende, DisplayMode oldMode, DisplayMode newMode)
        {
        }



        public int SectionIndex
        {
            get { return (int)GetValue(SectionIndexProperty); }
            set { SetValue(SectionIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SectionIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SectionIndexProperty =
            DependencyProperty.Register("SectionIndex", typeof(int), typeof(Main), new FrameworkPropertyMetadata(
                0,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnSectionIndexPropertyChanged
                ));


        private static void OnSectionIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Main)d).OnSectionIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual void OnSectionIndexChanged(int oldIndex, int newIndex)
        {
            ribbon.ContextualTabSet = null;
            foreach (RibbonTabItem item in ribbon.Tabs)
            {
                item.Visibility = (item == TemplateTab) ^ (newIndex == 0) ? Visibility.Visible : Visibility.Collapsed;
            }
            ribbon.EnsureTabIsVisible();

            PasswordGrid.Visibility = newIndex == 0 ? Visibility.Visible : Visibility.Hidden;
            TemplateCategoryTab.Visibility =
            TemplateGrid.Visibility = (newIndex == 1 ? Visibility.Visible : Visibility.Hidden);

        }

        public SkinId Skin
        {
            get { return (SkinId)GetValue(SkinProperty); }
            set { SetValue(SkinProperty, value); }
        }

        public static readonly DependencyProperty SkinProperty =
            DependencyProperty.Register("Skin", typeof(SkinId), typeof(Main), new FrameworkPropertyMetadata(SkinId.OfficeBlue, SkinPropertyChanged));


        private static void SkinPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SkinId id = (SkinId)e.NewValue;
            SkinManager.SkinId = id;
            Main main = (Main)o;
            int index = SkinToId(id);
            if (main.StyleGallery.SelectedIndex != index)
            {
                main.StyleGallery.SelectedIndex = index;
            }

        }

        private static int SkinToId(SkinId id)
        {
            switch (id)
            {
                case SkinId.Vista: return 0;
                case SkinId.Windows7: return 1;
                case SkinId.OfficeBlue: return 2;
                case SkinId.OfficeSilver: return 3;
                case SkinId.OfficeBlack: return 4;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void RibbonGallery_HotThumbnailChanged(object sender, RoutedEventArgs e)
        {
            RibbonGallery g = sender as RibbonGallery;
            RibbonThumbnail thumb = g.HotThumbnail;
            if (thumb != null)
            {
                SkinManager.SkinId = IdToSkin(g.Items.IndexOf(g.HotItem));
            }
            else SkinManager.SkinId = Skin;
        }

        private SkinId IdToSkin(int id)
        {

            switch (id)
            {
                case 0: return SkinId.Vista;
                case 1: return SkinId.Windows7;
                case 2: return SkinId.OfficeBlue;
                case 3: return SkinId.OfficeSilver;
                case 4: return SkinId.OfficeBlack;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void RibbonGallery_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RibbonGallery g = sender as RibbonGallery;
            SkinId skin = IdToSkin(g.SelectedIndex);
            if (Skin != skin) Skin = skin;
        }



        /// <summary>
        /// Gets or sets whether the database is locked. This is a dp.
        /// </summary>
        public bool IsLocked
        {
            get { return (bool)GetValue(IsLockedProperty); }
            set { SetValue(IsLockedProperty, value); }
        }

        public static readonly DependencyProperty IsLockedProperty =
            DependencyProperty.Register("IsLocked", typeof(bool), typeof(Main), new UIPropertyMetadata(true, OnLockedPropertyChanged));

        private static void OnLockedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Main)d).OnLockedChanged(d, (bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnLockedChanged(object sender, bool oldvalue, bool newValue)
        {
            if (newValue)
            {
                UnloadData();
                lockScreen.Dispatcher.BeginInvoke((Func<int>)delegate()
                {
                    lockScreen.FocusPassword();
                    return 0;
                });
            }
        }



        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.SystemKey == (Key.M) && Keyboard.Modifiers == ModifierKeys.Alt)
            {
                ribbon.ApplicationMenu.IsOpen ^= true;
            }

            base.OnPreviewKeyDown(e);
        }


        public Folder SelectedFolder
        {
            get { return (Folder)GetValue(SelectedFolderProperty); }
            set { SetValue(SelectedFolderProperty, value); }
        }

        public static readonly DependencyProperty SelectedFolderProperty =
            DependencyProperty.Register("SelectedFolder", typeof(Folder), typeof(Main),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnSelectedFolderPropertyChanged
                ));

        private static void OnSelectedFolderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Main)d).OnSelectedFolderChanged((Folder)e.OldValue, (Folder)e.NewValue);

        }

        protected virtual void OnSelectedFolderChanged(Folder oldFolder, Folder newFolder)
        {
            if (newFolder != null) this.categoriesTree.Deselect();
            this.PasswordGrid.DataContext = newFolder;
            foldersTreeView.Select(newFolder);
            SelectedNode = newFolder;

        }

        public TemplateField SelectedTemplateField
        {
            get { return (TemplateField)GetValue(SelectedTemplateFieldProperty); }
            set { SetValue(SelectedTemplateFieldProperty, value); }
        }

        public static readonly DependencyProperty SelectedTemplateFieldProperty =
            DependencyProperty.Register("SelectedTemplateField", typeof(TemplateField), typeof(Main), new FrameworkPropertyMetadata(null,
                SelectedTemplateFieldPropertyChanged));


        private static void SelectedTemplateFieldPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TemplateField oldField = e.OldValue as TemplateField;
            if (oldField != null) oldField.IsSelected = false;
            Main main = (Main)d;
            TemplateField newField = (e.NewValue as TemplateField);
            if (newField != null) newField.IsSelected = true;
            main.OnSelectedTemplateFieldChanged(main, oldField, newField);
        }

        protected virtual void OnSelectedTemplateFieldChanged(object sender, TemplateField oldField, TemplateField newField)
        {
        }

        /// <summary>
        /// Gets or sets the selected password field.
        /// This is a dependency property.
        /// </summary>
        public Field SelectedPasswordField
        {
            get { return (Field)GetValue(SelectedPasswordFieldProperty); }
            set { SetValue(SelectedPasswordFieldProperty, value); }
        }

        public static readonly DependencyProperty SelectedPasswordFieldProperty =
            DependencyProperty.Register("SelectedPasswordField", typeof(Field), typeof(Main), new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                SelectedPasswordFieldPropertyChanged, CoerceSelectedPasswordField));

        private static void SelectedPasswordFieldPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Field oldField = e.OldValue as Field;
            if (oldField != null) oldField.IsSelected = false;
            Main main = (Main)d;
            Field newField = (e.NewValue as Field);
            if (newField != null)
            {
                newField.IsSelected = true;
            }
            main.OnSelectedPasswordFieldChanged(main, oldField, newField);
        }

        private void OnSelectedPasswordFieldChanged(object sender, Field oldField, Field newField)
        {
        }

        private static object CoerceSelectedPasswordField(DependencyObject d, object baseValue)
        {
            Main main = (Main)d;
            return main.SelectedPassword == null ? null : baseValue;
        }



        public Category SelectedCategory
        {
            get { return (Category)GetValue(SelectedCategoryProperty); }
            set { SetValue(SelectedCategoryProperty, value); }
        }

        public static readonly DependencyProperty SelectedCategoryProperty =
            DependencyProperty.Register("SelectedCategory", typeof(Category), typeof(Main),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                SelectedCategoryPropertyChanged));


        private static void SelectedCategoryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Main)d).OnSelectedCategoryChanged(d, (Category)e.OldValue, (Category)e.NewValue);

        }

        protected virtual void OnSelectedCategoryChanged(object sender, Category oldCategory, Category newCategory)
        {
            if (newCategory != null)
            {
                newCategory.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SelectedCategoryPropertyChanged);
            }
            if (oldCategory != null)
            {
                oldCategory.PropertyChanged -= SelectedCategoryPropertyChanged;
            }
            if (newCategory != null) this.foldersTreeView.Deselect();
            this.PasswordGrid.DataContext = newCategory;

            templateCategoryTree.Select(newCategory);
            categoriesTree.Select(newCategory);
            SelectedNode = newCategory;
            SelectedTemplateField = null;

            IsTemplateModified = newCategory != null ? newCategory.IsModified : false;
        }

        void SelectedCategoryPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // some bindings have IsAsync=True, so we need to do this invoked:
            Dispatcher.BeginInvoke((Func<int>)delegate()
            {
                Category category = (Category)sender;
                IsTemplateModified = category != null ? category.IsModified : false;
                return 0;
            });
        }

        public void NewPassword(Category category)
        {
            Password password = category.CreatePassword();
            if (password != null)
            {
                password.Name = "New Password";
                SelectedCategory = password.Category;
                this.SelectedPassword = password;
                this.PasswordGrid.FocusPassword();

            }
        }



        /// <summary>
        /// Gets the currently selected password.
        /// </summary>
        public Password SelectedPassword
        {
            get { return (Password)GetValue(SelectedPasswordProperty); }
            set { SetValue(SelectedPasswordProperty, value); }
        }

        public static readonly DependencyProperty SelectedPasswordProperty =
            DependencyProperty.Register("SelectedPassword", typeof(Password), typeof(Main), new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnSelectedPasswordPropertyChanged));


        private static void OnSelectedPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                Password oldPassword = (Password)e.OldValue;
                Password newPassword = (Password)e.NewValue;
                Main main = (Main)d;

                if (oldPassword != null) oldPassword.PropertyChanged -= main.OnPasswordPropertyChanged;
                if (newPassword != null) newPassword.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(main.OnPasswordPropertyChanged);
                main.SelectedPasswordField = null;
                main.IsPasswordModified = newPassword != null && newPassword.IsModified;
                main.IsPasswordSelected = newPassword != null;

            }
        }

        void OnPasswordPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // some bindings have IsAsync=True, so we need to do this invoked:
            Dispatcher.BeginInvoke((Func<int>)delegate()
            {
                Password password = (Password)sender;
                IsPasswordModified = password != null ? password.IsModified : false;
                return 0;
            });
        }



        /// <summary>
        /// Gets whether a password is selected.
        /// This is a dependency property.
        /// </summary>
        public bool IsPasswordSelected
        {
            get { return (bool)GetValue(IsPasswordSelectedProperty); }
            private set { SetValue(IsPasswordSelectedPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey IsPasswordSelectedPropertyKey =
            DependencyProperty.RegisterReadOnly("IsPasswordSelected", typeof(bool), typeof(Main), new UIPropertyMetadata(false));

        private static readonly DependencyProperty IsPasswordSelectedProperty = IsPasswordModifiedPropertyKey.DependencyProperty;



        /// <summary>
        /// Gets or sets the selected node for  the breadcrumb.
        /// </summary>
        public NodeBase SelectedNode
        {
            get { return (NodeBase)GetValue(SelectedNodeProperty); }
            set { SetValue(SelectedNodeProperty, value); }
        }

        public static readonly DependencyProperty SelectedNodeProperty =
            DependencyProperty.Register("SelectedNode", typeof(NodeBase), typeof(Main), new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedNodePropertyChanged));

        private static void OnSelectedNodePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Main)d).OnSelectedNodeChanged((NodeBase)e.OldValue, (NodeBase)e.NewValue);
        }

        protected virtual void OnSelectedNodeChanged(NodeBase oldNode, NodeBase newNode)
        {
            SelectedCategory = newNode as Category;
            SelectedFolder = newNode as Folder;
        }



        /// <summary>
        /// Selects the textbox to edit the name of the selected Password Field.
        /// </summary>
        public void SelectPasswordFieldName()
        {
            fieldName.Focus();
            fieldName.SelectAll();
        }


        public void FocusEditFolderName()
        {
            folderName.Focus();
            folderName.SelectAll();
        }

        public void FocusEditCategoryName()
        {
            if (!TemplateTab.IsVisible)
            {
                categoryName.Focus();
                categoryName.SelectAll();
            }
            else
            {
                categoryName2.Focus();
                categoryName2.SelectAll();
            }
        }


        internal void SelectTemplateFieldName()
        {
            TemplateFieldName.Focus();
            TemplateFieldName.SelectAll();

        }

        private void LaunchConfiguration(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Configuration...");
        }

        public bool Login(string password)
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(Settings.Default.PasswordsConnectionString);
            try
            {
                sb.PersistSecurityInfo = false;
                sb.Password = password;
                LoadData(sb.ConnectionString);
                DisplayType = DisplayType.Passwords;
                IsLocked = false;
                sb.Password = "";
                return true;
            }
            catch (SqlCeException)
            {
                sb.Password = "";
                DisplayType = DisplayType.Login;
                IsLocked = true;
                return false;
            }
        }



        public DisplayType DisplayType
        {
            get { return (DisplayType)GetValue(DisplayTypeProperty); }
            set { SetValue(DisplayTypeProperty, value); }
        }

        public static readonly DependencyProperty DisplayTypeProperty =
            DependencyProperty.Register("DisplayType", typeof(DisplayType), typeof(Main), new UIPropertyMetadata(DisplayType.Login, OnDisplayTypePropertyChanged));

        private static void OnDisplayTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Main)d).OnDisplayTypeChanged(d, (DisplayType)e.OldValue, (DisplayType)e.NewValue);
        }

        protected virtual void OnDisplayTypeChanged(object sender, DisplayType oldDisplayType, DisplayType newDisplayType)
        {
            switch (newDisplayType)
            {
                case DisplayType.Login:
                    BizContext.CloseConnection();
                    lockScreen.Dispatcher.BeginInvoke((Func<int>)delegate()
                    {
                        lockScreen.FocusPassword();
                        return 0;
                    });
                    break;

                case DisplayType.ChangePassword:
                    BizContext.CloseConnection();
                    this.changePassword.Dispatcher.BeginInvoke((Func<int>)delegate()
                    {
                        changePassword.FocusPassword();
                        return 0;
                    });
                    break;
            }
        }




        public IEnumerable<object> Categories
        {
            get { return (IEnumerable<object>)GetValue(CategoriesProperty); }
            set { SetValue(CategoriesProperty, value); }
        }

        public static readonly DependencyProperty CategoriesProperty =
            DependencyProperty.Register("Categories", typeof(IEnumerable<object>), typeof(Main), new UIPropertyMetadata(null));




    }
}
