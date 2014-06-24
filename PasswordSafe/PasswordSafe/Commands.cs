using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Odyssey.Controls;
using System.Windows;
using PasswordSafe.Controls;
using System.Data.Linq;
using System.Diagnostics;
using System.Windows.Controls;
using PasswordSafe.Classes;
using PasswordSafe.Data;
using PasswordSafe.Data.Biz;
using System.Windows.Controls.Primitives;
using PasswordSafe.Export;
using Microsoft.Win32;
using System.IO;

namespace PasswordSafe.Controls
{
    public class Commands
    {
        public static readonly RoutedUICommand TestCommand = new RoutedUICommand("Test", "TestCommand", typeof(Main));
        public static readonly RoutedUICommand ExportCommand = new RoutedUICommand("Test", "ExportCommand", typeof(Main));

        public static readonly RoutedUICommand SelectNextFieldCommand = new RoutedUICommand("Next", "SelectNextFieldCommand", typeof(Main));
        public static readonly RoutedUICommand SelectPreviousFieldCommand = new RoutedUICommand("Previous", "SelectPreviousFieldCommand", typeof(Main));

        public static readonly RoutedUICommand LogoutCommand = new RoutedUICommand("Logout", "LogoutCommand", typeof(Main),
            new InputGestureCollection(new KeyGesture[] { 
                new KeyGesture(Key.L,ModifierKeys.Control,"Ctrl+L")}));

        public static readonly RoutedUICommand ChangePasswordCommand = new RoutedUICommand("New", "ChangePasswordCommand", typeof(Main));
        public static readonly RoutedUICommand NewPasswordCommand = new RoutedUICommand("New", "NewPasswordCommand", typeof(Main));
        public static readonly RoutedUICommand DuplicateCommand = new RoutedUICommand("Duplicate", "DuplicateCommand", typeof(Main));

        public static readonly RoutedUICommand SaveTemplateCommand = new RoutedUICommand("Save", "SaveTemplateCommand", typeof(Main),
            new InputGestureCollection(new KeyGesture[] { new KeyGesture(Key.S, ModifierKeys.Control, "Ctrl+S") }));

        public static readonly RoutedUICommand SaveCommand = new RoutedUICommand("Save", "SaveCommand", typeof(Main),
            new InputGestureCollection(new KeyGesture[] { new KeyGesture(Key.S, ModifierKeys.Control, "Ctrl+S") }));

        /// <summary>
        /// Used for <see cref="T:Field"/> templates to copy the Value property of the <see cref="T:Field"/> DataContext of a FrameworkControl to clipboard.
        /// </summary>
        public static readonly RoutedUICommand CopyCommand = new RoutedUICommand("Copy", "CopyCommand", typeof(Main),
            new InputGestureCollection(new KeyGesture[] { new KeyGesture(Key.C, ModifierKeys.Control, "Ctrl+C") }));

        public static readonly RoutedUICommand UndoAllCommand = new RoutedUICommand("Undo All", "UndoAllCommand", typeof(Main),
            new InputGestureCollection(new KeyGesture[] { 
                new KeyGesture(Key.Z,ModifierKeys.Control|ModifierKeys.Shift,"Shift+Ctrl+Z"),
                new KeyGesture(Key.U,ModifierKeys.Control|ModifierKeys.Shift,"Shift+Ctrl+U") 
            }));

        public static readonly RoutedUICommand SaveAllCommand = new RoutedUICommand("Save All", "SaveAllCommand", typeof(Main),
    new InputGestureCollection(new KeyGesture[] { new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift, "Shift+Ctrl+S") }));

        public static readonly RoutedUICommand UndoTemplateCommand = new RoutedUICommand("Undo", "UndoTemplateCommand", typeof(Main),
            new InputGestureCollection(new KeyGesture[] { 
                new KeyGesture(Key.Z,ModifierKeys.Control,"Ctrl+Z"),
                new KeyGesture(Key.U,ModifierKeys.Control,"Ctrl+U") 
            }));

        public static readonly RoutedUICommand UndoCommand = new RoutedUICommand("Undo", "UndoCommand", typeof(Main),
            new InputGestureCollection(new KeyGesture[] { 
                new KeyGesture(Key.Z,ModifierKeys.Control,"Ctrl+Z"),
                new KeyGesture(Key.U,ModifierKeys.Control,"Ctrl+U") 
            }));


        public static readonly RoutedUICommand DeleteCommand = new RoutedUICommand("Delete", "DeleteCommand", typeof(Main));
        public static readonly RoutedUICommand SelectItemCommand = new RoutedUICommand("Select", "SelectItemCommand", typeof(Main));
        public static readonly RoutedUICommand SelectTemplateItemCommand = new RoutedUICommand("Select", "SelectTemplateItemCommand", typeof(Main));
        public static readonly RoutedUICommand CopyUserNameCommand = new RoutedUICommand("Copy User Name", "CopyUserNameCommand", typeof(Main));
        public static readonly RoutedUICommand DataModifiedCommand = new RoutedUICommand("", "DataModifiedCommand", typeof(Main));

        public static readonly RoutedUICommand FieldUpCommand = new RoutedUICommand("Up", "FieldUpCommand", typeof(Main));
        public static readonly RoutedUICommand FieldTopCommand = new RoutedUICommand("Down", "FieldDownCommand", typeof(Main));
        public static readonly RoutedUICommand FieldBottomCommand = new RoutedUICommand("Top", "FieldTopCommand", typeof(Main));
        public static readonly RoutedUICommand FieldDownCommand = new RoutedUICommand("Bottom", "FieldBottomCommand", typeof(Main));
        public static readonly RoutedUICommand FieldDeleteCommand = new RoutedUICommand("Delete", "FieldDeleteCommand", typeof(Main));
        public static readonly RoutedUICommand NewFieldCommand = new RoutedUICommand("New Field", "NewFieldCommand", typeof(Main),
            new InputGestureCollection(new KeyGesture[] { new KeyGesture(Key.N, ModifierKeys.Control, "Ctrl+N") }));

        public static readonly RoutedUICommand NewTemplateFieldCommand = new RoutedUICommand("New Field", "NewTemplateFieldCommand", typeof(Main),
            new InputGestureCollection(new KeyGesture[] { new KeyGesture(Key.N, ModifierKeys.Control, "Ctrl+N") }));
        public static readonly RoutedUICommand DeleteTemplateFieldCommand = new RoutedUICommand("Delete", "DeleteTemplateFieldCommand", typeof(Main));

        public static readonly RoutedUICommand CategoryUpCommand = new RoutedUICommand("Up", "CategoryUpCommand", typeof(Main));
        public static readonly RoutedUICommand CategoryTopCommand = new RoutedUICommand("Down", "CategoryTopCommand", typeof(Main));
        public static readonly RoutedUICommand CategoryBottomCommand = new RoutedUICommand("Top", "CategoryBottomCommand", typeof(Main));
        public static readonly RoutedUICommand CategoryDownCommand = new RoutedUICommand("Bottom", "CategoryDownCommand", typeof(Main));

        public static readonly RoutedUICommand TemplateUpCommand = new RoutedUICommand("Up", "TemplateUpCommand", typeof(Main));
        public static readonly RoutedUICommand TemplateTopCommand = new RoutedUICommand("Down", "TemplateTopCommand", typeof(Main));
        public static readonly RoutedUICommand TemplateBottomCommand = new RoutedUICommand("Top", "TemplateBottomCommand", typeof(Main));
        public static readonly RoutedUICommand TemplateDownCommand = new RoutedUICommand("Bottom", "TemplateDownCommand", typeof(Main));


        public static readonly RoutedUICommand FolderUpCommand = new RoutedUICommand("Up", "FolderUpCommand", typeof(Main));
        public static readonly RoutedUICommand FolderTopCommand = new RoutedUICommand("Down", "FolderTopCommand", typeof(Main));
        public static readonly RoutedUICommand FolderBottomCommand = new RoutedUICommand("Top", "FolderBottomCommand", typeof(Main));
        public static readonly RoutedUICommand FolderDownCommand = new RoutedUICommand("Bottom", "FolderDownCommand", typeof(Main));

        /// <summary>
        /// Does nothing but changing IsEnabled.
        /// </summary>
        public static readonly RoutedUICommand PasswordSelectedCommand = new RoutedUICommand("", "PasswordSelectedCommand", typeof(Main));
        public static readonly RoutedUICommand FieldSelectedCommand = new RoutedUICommand("", "FieldSelectedCommand", typeof(Main));

        /// <summary>
        /// Toggles the IsFavorite state of a <see ref="T:PasswordFolder"/> class which is the DataContext of a FrameworkControl.
        /// This command is used for the TreeViews to change the favorite association of a <see cref="T:Password"/>.
        /// </summary>
        public static readonly RoutedUICommand ToggleFaveCommand = new RoutedUICommand("", "ToggleFaveCommand", typeof(Main));

        public static readonly RoutedUICommand AddFolderCommand = new RoutedUICommand("Add Folder", "AddFolderCommand", typeof(Main));
        public static readonly RoutedUICommand DeleteFolderCommand = new RoutedUICommand("Delete Folder", "DeleteFolderCommand", typeof(Main));

        public static readonly RoutedUICommand AddCategoryCommand = new RoutedUICommand("Add Category", "AddCategoryCommand", typeof(Main));
        public static readonly RoutedUICommand DeleteCategoryCommand = new RoutedUICommand("Delete Category", "DeleteCategoryCommand", typeof(Main));

        static Commands()
        {
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(TestCommand, Test));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(ExportCommand, Export));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(SelectNextFieldCommand, SelectNextPasswordField, IsPasswordSelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(SelectPreviousFieldCommand, SelectPrevPasswordField, IsPasswordSelected));

            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(LogoutCommand, Logout));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(ChangePasswordCommand, ChangePassword));

            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(SaveAllCommand, SaveAll, IsAnyPasswordOrTemplateModified));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(UndoAllCommand, UndoAll, IsAnyPasswordOrTemplateModified));

            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(SaveTemplateCommand, SaveTemplate, IsTemplateModified));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(UndoTemplateCommand, UndoTemplate, IsTemplateModified));

            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(NewPasswordCommand, NewPassword));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(DuplicateCommand, Duplicate, IsPasswordSelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(SaveCommand, SavePassword, IsPasswordModified));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(UndoCommand, UndoPassword, IsPasswordModified));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(DeleteCommand, DeletePassword, IsPasswordSelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(SelectItemCommand, SelectItem));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(SelectTemplateItemCommand, SelectTemplateItem));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(CopyUserNameCommand, CopyUserName));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(DataModifiedCommand, DataModified));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(FieldUpCommand, FieldUp, IsFieldSelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(FieldDownCommand, FieldDown, IsFieldSelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(FieldTopCommand, FieldTop, IsFieldSelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(FieldBottomCommand, FieldBottom, IsFieldSelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(FieldDeleteCommand, FieldDelete, IsFieldSelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(NewFieldCommand, NewField, IsPasswordSelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(PasswordSelectedCommand, Nothing, IsPasswordSelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(FieldSelectedCommand, Nothing, IsFieldSelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(NewTemplateFieldCommand, NewTemplateField, IsCategorySelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(DeleteTemplateFieldCommand, DeleteTemplateField, IsTemplateSelected));

            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(CategoryUpCommand, CategoryUp, IsCategorySelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(CategoryDownCommand, CategoryDown, IsCategorySelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(CategoryTopCommand, CategoryTop, IsCategorySelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(CategoryBottomCommand, CategoryBottom, IsCategorySelected));

            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(TemplateUpCommand, TemplateUp, IsTemplateSelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(TemplateDownCommand, TemplateDown, IsTemplateSelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(TemplateTopCommand, TemplateTop, IsTemplateSelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(TemplateBottomCommand, TemplateBottom, IsTemplateSelected));

            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(FolderUpCommand, FolderUp, IsFolderSelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(FolderDownCommand, FolderDown, IsFolderSelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(FolderTopCommand, FolderTop, IsFolderSelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(FolderBottomCommand, FolderBottom, IsFolderSelected));

            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(CopyCommand, CopyField, IsCopyEnabled));

            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(ToggleFaveCommand, ToggleFave));

            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(AddFolderCommand, AddFolder, IsFolderSelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(DeleteFolderCommand, DeleteFolder, CanDeleteFolder));

            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(AddCategoryCommand, AddCategory, IsCategorySelected));
            CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(DeleteCategoryCommand, DeleteCategory, CanDeleteCategory));
        }



        private static void Nothing(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
        }

        private static void SaveAll(object sender, ExecutedRoutedEventArgs e)
        {
            BizContext.Instance.Save();
        }

        private static void UndoAll(object sender, ExecutedRoutedEventArgs e)
        {
            BizContext.Instance.Undo();
        }

        private static void Export(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = ".xml";
            saveDialog.Title = "Export to...";
            saveDialog.AddExtension = true;
            if (saveDialog.ShowDialog(main).Value)
            {
                using (FileStream stream = new FileStream(saveDialog.FileName, FileMode.Create, FileAccess.Write))
                {
                    XmlExporter.Export(stream);
                }
            }
        }


        private static void SelectPrevPasswordField(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            Password pw = main.SelectedPassword;


            if (pw != null && pw.Fields.Count > 0)
            {
                int n = pw.Fields.Count - 1;
                int index = n;
                for (int i = n; i > 0; i--)
                {
                    if (pw[i].IsSelected)
                    {
                        index = i - 1;
                        break;
                    }
                }
                main.SelectedPasswordField = pw[index];
            }
        }


        private static void SelectNextPasswordField(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            Password pw = main.SelectedPassword;

            if (pw != null && pw.Fields.Count > 0)
            {
                int index = 0;
                for (int i = 0; i < pw.Fields.Count - 1; i++)
                {
                    if (pw[i].IsSelected)
                    {
                        index = i + 1;
                        break;
                    }
                }
                main.SelectedPasswordField = pw[index];
            }
        }


        private static void Logout(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            main.IsLocked = true;
            main.DisplayType = DisplayType.Login;
        }

        private static void AddCategory(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;

            Category category = main.SelectedCategory;
            if (category != null)
            {
                Category newCategory = category.AddCategory("New Category");
                if (newCategory != null)
                {
                    main.SelectedCategory = newCategory;
                    main.FocusEditCategoryName();
                }
            }
        }

        private static void DeleteCategory(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;

            Category category = main.SelectedCategory;
            if (category != null)
            {
                category.Delete();
                main.SelectedCategory = category.Parent;
            }
        }

        private static void CanDeleteCategory(object sender, CanExecuteRoutedEventArgs e)
        {
            Main main = (Main)sender;
            Category category = main.SelectedCategory;
            e.CanExecute = category != null && category.Parent != null && !category.Categories.Any() && !category.Passwords.Any();
        }


        private static void AddFolder(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;

            Folder folder = main.SelectedFolder;
            if (folder != null)
            {
                Folder newFolder = folder.AddFolder("New Folder");
                if (newFolder != null)
                {
                    main.SelectedFolder = newFolder;
                    main.FocusEditFolderName();
                }
            }
        }

        private static void DeleteFolder(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;

            Folder folder = main.SelectedFolder;
            if (folder != null)
            {
                folder.Delete();
                main.SelectedFolder = folder.Parent;
            }
        }

        private static void CanDeleteFolder(object sender, CanExecuteRoutedEventArgs e)
        {
            Main main = (Main)sender;
            Folder folder = main.SelectedFolder;
            e.CanExecute = folder != null && folder.Parent != null && !folder.Folders.Any();
        }


        private static void ToggleFave(object sender, ExecutedRoutedEventArgs e)
        {
            FrameworkElement fe = (e.OriginalSource as FrameworkElement);
            PasswordFolder folder = fe != null ? fe.DataContext as PasswordFolder : null;
            if (folder != null) folder.IsFavorite ^= true;

        }

        private static void CopyField(object sender, ExecutedRoutedEventArgs e)
        {
            FrameworkElement fe = (e.OriginalSource) as FrameworkElement;
            Field field = (fe != null) ? fe.DataContext as Field : null;
            if (field != null && field.Value != null) Clipboard.SetData(DataFormats.Text, field.ValueAsString());
        }

        private static void IsCopyEnabled(object sender, CanExecuteRoutedEventArgs e)
        {
            FrameworkElement fe = (e.OriginalSource) as FrameworkElement;
            Field field = (fe != null) ? fe.DataContext as Field : null;
            e.CanExecute = field != null && field.Value != null && !string.IsNullOrEmpty(field.Value.ToString());
        }

        private static void FolderTop(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            Folder folder = main.SelectedFolder;
            if (folder != null)
            {
                folder.Top();
                main.SelectedFolder = folder;

                EnsureFolderTreeDoesNotSelectRoot(main);
            }
        }


        private static void FolderBottom(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            Folder folder = main.SelectedFolder;
            if (folder != null)
            {
                folder.Bottom();
                main.SelectedFolder = folder;
                EnsureFolderTreeDoesNotSelectRoot(main);
            }
        }

        private static void FolderUp(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            Folder folder = main.SelectedFolder;
            if (folder != null)
            {
                folder.Up();
                main.SelectedFolder = folder;
                EnsureFolderTreeDoesNotSelectRoot(main);

            }
        }

        private static void FolderDown(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            Folder folder = main.SelectedFolder;
            if (folder != null)
            {
                folder.Down();
                main.SelectedFolder = folder;
                EnsureFolderTreeDoesNotSelectRoot(main);
            }
        }

        private static void EnsureFolderTreeDoesNotSelectRoot(Main main)
        {
            main.foldersTreeView.Focus();
        }

        private static void TemplateTop(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            TemplateField field = main.SelectedTemplateField;
            if (field != null)
            {
                field.Top();
                main.SelectedTemplateField = field;

                EnsureTemplateTreeDoesNotSelectRoot(main);
            }
        }

        private static void TemplateBottom(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            TemplateField field = main.SelectedTemplateField;
            if (field != null)
            {
                field.Bottom();
                main.SelectedTemplateField = field;
                EnsureTemplateTreeDoesNotSelectRoot(main);
            }
        }

        private static void TemplateUp(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            TemplateField field = main.SelectedTemplateField;
            if (field != null)
            {
                field.Up();
                main.SelectedTemplateField = field;
                EnsureTemplateTreeDoesNotSelectRoot(main);

            }
        }

        private static void TemplateDown(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            TemplateField field = main.SelectedTemplateField;
            if (field != null)
            {
                field.Down();
                main.SelectedTemplateField = field;
                EnsureTemplateTreeDoesNotSelectRoot(main);
            }
        }

        private static void EnsureTemplateTreeDoesNotSelectRoot(Main main)
        {
            main.foldersTreeView.Focus();
        }

        private static void IsTemplateSelected(object sender, CanExecuteRoutedEventArgs e)
        {
            Main main = (Main)sender;
            e.CanExecute = main.SelectedTemplateField != null;
        }

        private static void CategoryTop(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            Category category = main.SelectedCategory;
            if (category != null)
            {
                category.Top();
                main.SelectedCategory = category;

                EnsureCategoryTreeDoesNotSelectRoot(main);
            }
        }

        private static void CategoryBottom(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            Category category = main.SelectedCategory;
            if (category != null)
            {
                category.Bottom();
                main.SelectedCategory = category;
                EnsureCategoryTreeDoesNotSelectRoot(main);
            }
        }

        private static void CategoryUp(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            Category category = main.SelectedCategory;
            if (category != null)
            {
                category.Up();
                main.SelectedCategory = category;
                EnsureCategoryTreeDoesNotSelectRoot(main);

            }
        }

        private static void CategoryDown(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            Category category = main.SelectedCategory;
            if (category != null)
            {
                category.Down();
                main.SelectedCategory = category;
                EnsureCategoryTreeDoesNotSelectRoot(main);
            }
        }

        /// <summary>
        /// When performing an Up,Down, Top or Bottom to the Category TreeView,
        /// the TreeView automatically selects the root node.
        /// To prevent this, thr workaround is to Focus the TreeView directly after one of this operations.
        /// </summary>
        private static void EnsureCategoryTreeDoesNotSelectRoot(Main main)
        {
            main.categoriesTree.Focus();
        }

        private static void NewTemplateField(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;

            Category category = main.SelectedCategory;
            if (category != null)
            {
                FieldType type = (e.Parameter is FieldType) ? (FieldType)e.Parameter : (FieldType)Enum.Parse(typeof(FieldType), e.Parameter as string);
                TemplateField field = category.AddField(type, "New Field");
                main.SelectedTemplateField = field;
                main.SelectTemplateFieldName();
            }
        }


        private static void NewField(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;

            Password password = main.SelectedPassword;
            if (password != null)
            {
                FieldType type = (e.Parameter is FieldType) ? (FieldType)e.Parameter : (FieldType)Enum.Parse(typeof(FieldType), e.Parameter as string);
                password.AddField(type, "New Field");
                Field field = password.Fields[password.Fields.Count - 1];
                main.SelectedPasswordField = field;
                main.SelectPasswordFieldName();
            }
        }

        private static void DeleteTemplateField(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            TemplateField field = main.SelectedTemplateField;
            if (field != null)
            {
                field.Delete();
                main.SelectedTemplateField = null;
            }
        }



        private static void FieldDelete(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            Field field = main.SelectedPasswordField;
            if (field != null)
            {
                field.Delete();
                main.SelectedPasswordField = null;
            }
        }


        private static void FieldTop(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            Field field = main.SelectedPasswordField;
            if (field != null)
            {
                field.Top();
            }
        }

        private static void FieldBottom(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            Field field = main.SelectedPasswordField;
            if (field != null)
            {
                field.Bottom();
            }
        }

        private static void FieldUp(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            Field field = main.SelectedPasswordField;
            if (field != null)
            {
                field.Up();
            }
        }

        private static void FieldDown(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            Field field = main.SelectedPasswordField;
            if (field != null)
            {
                field.Down();
            }
        }

        private static void DataModified(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            BaseObject entity = (e.OriginalSource as FrameworkElement).DataContext as BaseObject;
        }



        private static void CopyUserName(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            //main.IsModified ^= true;

        }

        private static void SelectItem(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonToggleButton btn = e.OriginalSource as RibbonToggleButton;
            if (btn != null)
            {
                if (!btn.IsChecked.Value) btn = null;
                Main main = (Main)sender;
                main.SelectedPasswordField = btn != null ? btn.DataContext as Field : null;
            }
        }

        private static void SelectTemplateItem(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonToggleButton btn = e.OriginalSource as RibbonToggleButton;
            if (btn != null)
            {
                if (!btn.IsChecked.Value) btn = null;
                Main main = (Main)sender;
                main.SelectedTemplateField = btn != null ? btn.DataContext as TemplateField : null;
            }
        }
        private static void NewPassword(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            FrameworkElement fe = e.OriginalSource as FrameworkElement;
            Category category = fe != null ? fe.DataContext as Category : null;
            if (category != null)
            {
                main.NewPassword(category);
            }
        }

        private static void IsFolderSelected(object sender, CanExecuteRoutedEventArgs e)
        {
            Main main = (Main)sender;
            e.CanExecute = main.SelectedFolder != null;
        }

        private static void IsPasswordSelected(object sender, CanExecuteRoutedEventArgs e)
        {
            Main main = (Main)sender;
            e.CanExecute = main.SelectedPassword != null;
        }

        private static void IsFieldSelected(object sender, CanExecuteRoutedEventArgs e)
        {
            Main main = (Main)sender;
            e.CanExecute = main.SelectedPasswordField != null;
        }

        private static void IsCategorySelected(object sender, CanExecuteRoutedEventArgs e)
        {
            Main main = (Main)sender;
            e.CanExecute = main.SelectedCategory != null;
        }

        private static void IsPasswordModified(object sender, CanExecuteRoutedEventArgs e)
        {
            Main main = (Main)sender;
            e.CanExecute = main.IsPasswordModified;
        }

        private static void Test(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
        }

        private static void ChangePassword(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            main.DisplayType = DisplayType.ChangePassword;
        }


        private static void Duplicate(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
        }

        private static void SavePassword(object sender, ExecutedRoutedEventArgs e)
        {

            Main main = (Main)sender;
            Password password = main.SelectedPassword;
            if (password != null) password.Save();

        }

        private static void UndoPassword(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            Password password = main.SelectedPassword;
            if (password != null)
            {
                main.SelectedPasswordField = null;
                password.Undo();
            }
        }

        private static void SaveTemplate(object sender, ExecutedRoutedEventArgs e)
        {

            Main main = (Main)sender;
            Category category = main.SelectedCategory;
            if (category != null) category.SaveTemplate();

        }

        private static void UndoTemplate(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            Category category = main.SelectedCategory;
            if (category != null)
            {
                main.SelectedPasswordField = null;
                category.UndoTemplate();
            }
        }

        private static void IsTemplateModified(object sender, CanExecuteRoutedEventArgs e)
        {
            Main main = (Main)sender;

            e.CanExecute = main.IsTemplateModified;
        }

        private static void IsAnyPasswordOrTemplateModified(object sender, CanExecuteRoutedEventArgs e)
        {
            Main main = (Main)sender;

            e.CanExecute = main.IsModified;
        }

        private static void DeletePassword(object sender, ExecutedRoutedEventArgs e)
        {
            Main main = (Main)sender;
            Password password = main.SelectedPassword;
            if (password != null) password.Delete();

        }
    }
}
