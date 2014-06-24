using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace PasswordSafe.Data.Biz
{
    public class Password : BaseObject, IDisposable
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public Password(Category category)
            : base()
        {
            this.category = category;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public Password(Category category, int id, string name)
            : base(id, name)
        {
            this.category = category;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public Password(int categoryId, int id, string name)
            : base(id, name)
        {
            this.category = Context.GetCategory(categoryId);
        }

        #region IDisposable Members

        public void Dispose()
        {
            ClearFields();
        }

        #endregion

        protected override void OnModifiedChanged()
        {
            base.OnModifiedChanged();
        }


        private Category category;

        /// <summary>
        /// Gets or sets the category of this password. 
        /// </summary>
        public Category Category
        {
            get { return category; }
            set
            {
                if (category != value)
                {
                    Category oldCategory = category;
                    oldCategory.Passwords.RaiseListChangedEvents = false;
                    oldCategory.Passwords.Remove(this);
                    category = value;
                    category.Passwords.RaiseListChangedEvents = false;
                    category.Passwords.Add(this);
                    oldCategory.Passwords.RaiseListChangedEvents = true;
                    category.Passwords.RaiseListChangedEvents = true;
                    category.ResetPasswords();
                    oldCategory.ResetPasswords();
                    OnPropertyChanged("Category");
                }
            }
        }

        public string CategoryPath
        {
            get
            {
                if (Category == null) return string.Empty;
                string path = Category.Path;
                int index = path.IndexOf('\\');
                if (index > 0) path = path.Substring(index + 1);
                return path;
            }

            set
            {
                if (CategoryPath != value)
                {
                    SetCategoryByPath(value);
                }
            }
        }

        private void SetCategoryByPath(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                // first get the root:
                Category category = Category;
                while (category.Parent is Category) category = category.Parent;

                string[] names = value.Split('\\');

                foreach (string name in names)
                {
                    category = GetCategoryByName(category, name);
                    if (category == null) break;
                }
                if (category != null)
                {
                    Category = category;
                }
            }
        }

        private Category GetCategoryByName(Category category, string name)
        {
            return category.Categories.Where(c => c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }

        private NotifyList<Folder> folders;
        private NotifyList<Field> fields;

        public NotifyList<Folder> Folders
        {
            get
            {
                if (folders == null)
                {
                    lock (this)
                    {
                        if (folders == null)
                        {
                            folders = Id != 0 ? new NotifyList<Folder>(Context.GetFolders(Id)) : new NotifyList<Folder>();
                            folders.Changed += new EventHandler<NotifyEventArgs<Folder>>(OnFoldersChanged);
                        }
                    }
                }
                return folders;
            }
        }

        /// <summary>
        /// Gets whether this password is added to the favorites.
        /// </summary>
        public bool IsFavorite
        {
            get
            {
                return Folders.Any();
            }
        }

        void OnFoldersChanged(object sender, NotifyEventArgs<Folder> e)
        {
            OnPropertyChanged("Folders");
            switch (e.Type)
            {
                case ChangedEventType.Added:
                    e.Item.Passwords.Add(this);
                    break;

                case ChangedEventType.Removed:
                    e.Item.Passwords.Remove(this);
                    break;
            }
        }

        /// <summary>
        /// Gets a list with all fields of this password.
        /// </summary>
        public NotifyList<Field> Fields
        {
            get
            {
                if (fields == null)
                {
                    fields = Id == 0 ? new NotifyList<Field>() : new NotifyList<Field>(Context.GetFields(this));

                    fields.Changed += new EventHandler<NotifyEventArgs<Field>>(OnFieldsChanged);
                }
                return fields;
            }
        }

        private void OnFieldsChanged(object sender, NotifyEventArgs<Field> e)
        {
            OnPropertyChanged("Fields");
        }


        /// <summary>
        /// Gets an enumeration of <see ref="PasswordFolder"/> associated to this password.
        /// </summary>
        public IEnumerable<PasswordFolder> PasswordFolders
        {
            get
            {
                // this list is temporarily for good reasons:
                // since the folders might change this list is recreated every time to reflect the actual hierarchy of folders.
                return GetPasswordFolders();
            }
        }

        private NotifyList<PasswordFolder> GetPasswordFolders()
        {
            return new NotifyList<PasswordFolder>(Context.Folders.Select(f => new PasswordFolder(f, this)));
            //foreach (var folder in Context.Folders)
            //{
            //    yield return new PasswordFolder(folder, this);
            //}
        }

        /// <summary>
        /// Deletes the password from datastore.
        /// </summary>
        public void Delete()
        {
            Category.Passwords.Remove(this);

            foreach (var folder in Folders)
            {

                folder.Passwords.Remove(this);
            }
            Folders.Clear();
            Context.DeletePassword(this);
        }

        /// <summary>
        /// Saves the changes to datastore.
        /// </summary>
        public void Save()
        {
            Context.SavePassword(this);
            IsModified = false;
        }

        /// <summary>
        /// Undos the changes made.
        /// </summary>
        public void Undo()
        {
            Context.UndoPassword(this);
            IsModified = false;
        }

        protected internal override void Reset()
        {
            foreach (var folder in Folders)
            {
                folder.ResetPasswords();
            }

            fields = null;
            folders = null;
            OnPropertyChanged("Fields");
            OnPropertyChanged("Folders");
            OnPropertyChanged("Name");
            OnPropertyChanged("IsFavorite");
            OnPropertyChanged("PasswordFolders");
            base.Reset();
        }

        /// <summary>
        /// Adds a new <see cref="Field"/> to the password.
        /// </summary>
        /// <param name="type">The type of the new <see cref="Field"/></param>
        /// <param name="name">The name of the new <see cref="T:Field"/></param>
        public void AddField(FieldType type, string name)
        {
            Field field = new Field(this, 0, name, type, null, (short)Fields.Count);
            this.Fields.Add(field);
        }

        /// <summary>
        /// Clears all fields from this password.
        /// </summary>
        private void ClearFields()
        {
            if (fields != null)
            {
                foreach (Field field in fields)
                {
                    field.Dispose();
                }
                fields.Clear();
                fields = null;
            }
        }

        public Field this[int index]
        {
            get { return Fields[index]; }
        }
    }
}
