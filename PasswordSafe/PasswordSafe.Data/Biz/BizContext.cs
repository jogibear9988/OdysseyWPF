using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PasswordSafe.Data.Biz;
using System.Diagnostics;

namespace PasswordSafe.Data.Biz
{
    /// <summary>
    /// The business context to exchange business objects with the Data Access Layer.
    /// </summary>
    public class BizContext : IDisposable
    {
        private static object lockObject = new object();
        private static BizContext instance;

        /// <summary>
        /// Opens the connection to use for the database.
        /// </summary>
        /// <param name="connectionString">The connection string about the database.</param>
        /// <returns>True, if the connection is opened, otherwise false.</returns>
        public static bool OpenConnection(string connectionString)
        {
            lock (lockObject)
            {
                CloseConnection();
                instance = new BizContext(connectionString);
            }
            return true;
        }

        /// <summary>
        /// Closes an existing connection.
        /// </summary>
        public static void CloseConnection()
        {
            if (Instance != null)
            {
                Instance.Dispose();
                instance = null;
            }
        }

        /// <summary>
        /// Gets the instance of <see cref="T:BizContext"/>.
        /// </summary>
        public static BizContext Instance
        {
            get
            {
                if (instance == null) instance = new BizContext("");
                return instance;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dal.Dispose();
        }

        #endregion

        private BizContext(string connectionString)
            : base()
        {
            Dal = new PasswordSafe.Data.DAL.Context(connectionString);
        }

        /// <summary>
        /// Gets the data access layer.
        /// </summary>
        internal DAL.Context Dal { get; private set; }


        private NotifyList<Folder> folders;
        private NotifyList<Category> categories;

        /// <summary>
        /// Gets a list of all root <see cref="T:Folder"/>s.
        /// </summary>
        public NotifyList<Folder> Folders
        {
            get
            {
                if (folders == null)
                {
                    lock (Dal)
                    {
                        if (folders == null)
                        {
                            folders = new NotifyList<Folder>(PersistentFolders(Dal.GetRootFolders().ToArray()));
                            if (folders.Count == 0 && Dal.IsConnected)
                            {
                                // add a root folder if none exists:
                                folders.Add(new Folder(0, "Folders"));
                            }
                        }
                    }
                }
                return folders;
            }
        }

        /// <summary>
        /// Gets the root <see cref="T:Folder"/>.
        /// </summary>
        public Folder RootFolder
        {
            get
            {
                return Folders.FirstOrDefault();
            }
        }

        public Category RootCategory
        {
            get { return Categories.FirstOrDefault(); }
        }


        /// <summary>
        /// Gets a list of all root <see cref="T:Category"/>s.
        /// </summary>
        public NotifyList<Category> Categories
        {
            get
            {
                if (categories == null || categories.Count == 0)
                {
                    lock (Dal)
                    {
                        if (categories == null || categories.Count == 0)
                        {
                            categories = new NotifyList<Category>(PersistentCategories(Dal.GetRootCategories()));
                            if (categories.Count == 0 && Dal.IsConnected)
                            {
                                // add a root if it does not exist:
                                //categories.Add(new Category(0, "Categories", (short)categories.Count));
                            }
                        }
                    }
                }
                return categories;
            }
        }

        internal IEnumerable<Category> GetCategories(Category parent)
        {
            return PersistentCategories(Dal.GetCategories(parent));
        }

        internal void SavePassword(Password password)
        {
            using (var transaction = Dal.BeginTransaction())
            {
                if (password.Id != 0)
                {
                    Dal.UpdatePassword(password);
                }
                else
                {
                    Dal.CreatePassword(password);
                }
                ReorderObjects<Field>(password.Fields);
                UpdateFields(password);
                UpdateFavorites(password);
                password.IsModified = false;
                transaction.Commit();
            }
        }

        internal void UndoPassword(Password password)
        {
            if (password.Id != 0)
            {
                if (password.IsModified)
                {
                    Password original = Dal.GetPassword(password.Id);
                    password.Name = original.Name;
                    password.Category = original.Category;
                    password.Order = original.Order;
                    password.Reset();
                    password.IsModified = false;
                }
            }
            else
            {
                // if the Id is null, the the password is new created and not commited, hence undoing means deleting:
                password.Delete();
            }
        }

        internal IEnumerable<Password> GetPasswords(Category category)
        {
            return PersistentPasswords(Dal.GetPasswordsByCategory(category));
        }

        /// <summary>
        /// Using dictionaries to ensure that there is always only one instance of Password, Folder or Category with the same Id persistent
        /// </summary>
        private Dictionary<int, Password> passwordDictionary = new Dictionary<int, Password>();
        private Dictionary<int, Folder> folderDictionary = new Dictionary<int, Folder>();
        private Dictionary<int, Category> categoryDictionary = new Dictionary<int, Category>();

        /// <summary>
        /// Checks to not return duplicate Folders with same id.
        /// </summary>
        private IEnumerable<Category> PersistentCategories(IEnumerable<Category> categories)
        {
            foreach (var c in categories)
            {
                if (categoryDictionary.ContainsKey(c.Id)) yield return categoryDictionary[c.Id];
                else
                {
                    categoryDictionary.Add(c.Id, c);
                    yield return c;
                }
            }
        }
        /// <summary>
        /// Checks to not return duplicate Folders with same id.
        /// </summary>
        private IEnumerable<Folder> PersistentFolders(IEnumerable<Folder> folders)
        {
            foreach (var f in folders)
            {
                int key = f.Id;
                if (folderDictionary.ContainsKey(key)) yield return folderDictionary[key];
                else
                {
                    //folderDictionary[key] = f;
                    folderDictionary.Add(key, f);
                    yield return f;
                }
            }
        }

        /// <summary>
        /// Checks to not return duplicate passwords with same id.
        /// </summary>
        private IEnumerable<Password> PersistentPasswords(IEnumerable<Password> passwords)
        {
            foreach (var p in passwords)
            {
                if (passwordDictionary.ContainsKey(p.Id)) yield return passwordDictionary[p.Id];
                else
                {
                    passwordDictionary.Add(p.Id, p);
                    yield return p;
                }
            }
        }

        internal IEnumerable<Folder> GetFolders(int passwordId)
        {
            return PersistentFolders(Dal.GetFolderByPasswordId(passwordId)).ToArray();
        }

        internal IEnumerable<Folder> GetFoldersByParentFolder(Folder parent)
        {
            return PersistentFolders(Dal.GetFoldersByParentFolder(parent));
        }

        internal IEnumerable<Password> GetPasswordsByFolderId(int folderId)
        {
            return PersistentPasswords(Dal.GetPasswordsByFolderId(folderId));
        }

        internal void DeletePassword(Password password)
        {
            if (password.Id != 0)
            {
                passwordDictionary.Remove(password.Id);
                Dal.DeletePassword(password.Id);
            }
        }

        internal IEnumerable<Field> GetFields(Password password)
        {
            return Dal.GetFields(password);
        }

        private void ReorderObjects<T>(IEnumerable<T> objects) where T : BaseObject
        {
            short order = 0;
            foreach (BaseObject o in objects) o.Order = order++;
        }

        private void ResetModified<T>(IEnumerable<T> objects) where T : BaseObject
        {
            foreach (BaseObject o in objects) o.IsModified = false;
        }

        private void UpdateFavorites(Password password)
        {
            int pwId = password.Id;

            IEnumerable<int> existingFaves = Dal.GetExistingFaves(password.Id);
            IEnumerable<int> deletedFaves = existingFaves.Except(password.Folders.Select(f => f.Id));
            IEnumerable<int> newFaves = password.Folders.Select(f => f.Id).Except(existingFaves);

            foreach (int id in deletedFaves)
            {
                Dal.DeleteFavorite(password.Id, id);
            }

            foreach (int id in newFaves)
            {
                Dal.CreateFavorite(password.Id, id);
            }

        }

        private void UpdateFields(Password password)
        {
            foreach (FieldType type in new FieldType[] { FieldType.Text, FieldType.Date, FieldType.Int, FieldType.Memo })
            {
                UpdateFields(password, type);
            }
        }

        private void UpdateFields(Password password, FieldType type)
        {

            int[] fieldIds = Dal.GetFieldIds(password.Id, type).ToArray();

            // determine the field ids to delete:
            IEnumerable<int> deletedIds = fieldIds.Except(password.Fields.Where(f => f.Id != 0).Select(f => f.Id));
            foreach (var id in deletedIds)
            {
                Dal.DeleteField(id, type);
            }

            // modify existing fields:
            var existingFields = password.Fields.Where(f => f.Id > 0);
            foreach (var field in existingFields)
            {
                if (field.IsModified)
                {
                    Dal.UpdateField(field);
                }
            }

            // create the new fields:
            var newFields = password.Fields.Where(f => f.Id == 0);
            foreach (var field in newFields)
            {
                field.Id = Dal.CreateField(field);
            }

            ResetModified<Field>(password.Fields);
        }


        internal Category GetCategory(int categoryId)
        {
            if (categoryDictionary.ContainsKey(categoryId)) return categoryDictionary[categoryId];

            Category c = Dal.GetCategoryById(categoryId);
            categoryDictionary.Add(c.Id, c);
            return c;
        }

        internal Folder GetFolderById(int folderId)
        {
            if (folderDictionary.ContainsKey(folderId)) return folderDictionary[folderId];
            Folder folder = Dal.GetFolderById(folderId);
            folderDictionary.Add(folder.Id, folder);
            return folder;
        }

        public Folder CreateFolder(Folder folder)
        {
            Dal.CreateFolder(folder);
            folderDictionary.Add(folder.Id, folder);
            return folder;
        }

        internal void DeleteFolder(Folder folder)
        {
            if (folder.Folders.Count > 0)
            {
                throw new ArgumentException("Cannot delete Folder that has Children.");
            }


            if (folder.Id != 0)
            {
                Dal.DeleteFolder(folder.Id);
                if (folderDictionary.ContainsKey(folder.Id))
                {
                    folderDictionary.Remove(folder.Id);
                }
            }
        }

        internal Category CreateCategory(Category parent)
        {
            Category category = Dal.CreateCategory(parent);
            categoryDictionary.Add(category.Id, category);
            return category;
        }

        internal void DeleteCategory(Category category)
        {
            if (category.Passwords.Count > 0)
            {
                throw new ArgumentException("Cannot delete category that has passwords associated.");
            }
            if (category.Categories.Count > 0)
            {
                throw new ArgumentException("Cannot delete category that has Children.");
            }

            if (category.Id != 0)
            {
                Dal.DeleteCategory(category.Id);
                if (categoryDictionary.ContainsKey(category.Id))
                {
                    categoryDictionary.Remove(category.Id);
                }
            }
        }

        internal IEnumerable<TemplateField> GetTemplates(Category category)
        {
            return Dal.GetTemplates(category);
        }

        public void Commit()
        {
            //    throw new NotImplementedException();
        }


        internal void SaveCategory(Category category)
        {
            using (var transaction = Dal.BeginTransaction())
            {
                if (category.Id == 0) Dal.CreateCategory(category);
                Dal.UpdateCategory(category);
                category.IsModified = false;
                transaction.Commit();
            }
        }

        internal void SaveTemplate(Category category)
        {
            using (var transaction = Dal.BeginTransaction())
            {
                ReorderObjects<TemplateField>(category.Fields);
                UpdateTemplateFields(category.Id, category.Fields);
                category.IsModified = false;
                transaction.Commit();
            }
        }

        private void UpdateTemplateFields(int categoryId, IEnumerable<TemplateField> fields)
        {
            IEnumerable<int> ids = Dal.GetTemplateFields(categoryId);

            IEnumerable<int> deleted = ids.Except(fields.Select(f => f.Id));
            foreach (int deletedId in deleted)
            {
                Dal.DeleteTemplateField(deletedId);
            }

            foreach (TemplateField field in fields)
            {
                if (field.IsModified)
                {
                    if (field.Id == 0) Dal.CreateTemplateField(field);
                    Dal.UpdateTemplateField(field);
                    field.IsModified = false;
                }
            }
        }

        internal void SaveFolder(Folder folder)
        {
            using (var transaction = Dal.BeginTransaction())
            {
                if (folder.Id == 0) Dal.CreateFolder(folder);
                Dal.UpdateFolder(folder);
                folder.IsModified = false;
                transaction.Commit();
            }
        }

        public bool ChangePassword(string connectionString, string oldPassword, string newPassword)
        {
            return Dal.ChangePassword(connectionString, oldPassword, newPassword);
        }

        /// <summary>
        /// Saves all changes that where made to any password or template.
        /// </summary>
        public void Save()
        {
            Category root = RootCategory;
            if (root != null)
            {
                SaveCategoryNested(root);
                foreach (var password in root.NestedPasswords)
                {
                    if (password.IsModified) password.Save();
                }
            }
        }

        /// <summary>
        /// Undos all changes that where made to any password or template.
        /// </summary>
        public void Undo()
        {
            Category root = RootCategory;
            if (root != null)
            {
                UndoCategoryNested(root);
                foreach (var password in root.NestedPasswords)
                {
                    if (password.IsModified) password.Undo();
                }
            }
        }

        private void UndoCategoryNested(Category c)
        {
            if (c.IsModified) c.Save();
            foreach (var sub in c.Categories)
            {
                sub.UndoTemplate();
            }
        }

        private void SaveCategoryNested(Category c)
        {
            if (c.IsModified) c.Save();
            foreach (var sub in c.Categories)
            {
                sub.Save();
            }
        }
    }
}
