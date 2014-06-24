using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PasswordSafe.Data.Biz
{
    public class Folder : NodeBase
    {
        public Folder(int id, string name)
            : base(id, name)
        {
        }

        public Folder(int id, string name, Folder parent)
            : base(id, name)
        {
            this.parent = parent;
        }

        public Folder(int id, string name, int? parentId)
            : base(id, name)
        {
            this.parentId = parentId;
        }

        public int title;

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case "Name":
                    Save();
                    break;

                case "Passwords":
                    OnPasswordPropertyChanged();
                    break;

                case "NestedPasswords":
                    OnPropertyChanged("NestedPasswordCount");
                    break;
            }
        }


        private int? parentId;
        private Folder parent;

        /// <summary>
        /// Gets the parent <see cref="T:Folder"/>, otherwise null.
        /// </summary>
        public Folder Parent
        {
            get
            {
                if (parent == null && parentId.HasValue)
                {
                    parent = Context.GetFolderById(parentId.Value);
                }
                return parent;
            }
        }

        private NotifyList<Folder> folders;
        private NotifyList<Password> passwords;

        /// <summary>
        /// Resets the list of <see cref="T:Password"/>s.
        /// </summary>
        public void ResetPasswords()
        {
            passwords = null;
            OnPropertyChanged("Passwords");
        }

        /// <summary>
        /// Gets the list of all <see cref="T:Password"/>s associated to this Folder.
        /// </summary>
        public NotifyList<Password> Passwords
        {
            get
            {
                EnsurePasswords();
                return passwords;
            }
        }

        private void EnsurePasswords()
        {
            if (passwords == null)
            {
                passwords = Id != 0 ? new NotifyList<Password>(Context.GetPasswordsByFolderId(Id)) : new NotifyList<Password>();
                passwords.Changed += new EventHandler<NotifyEventArgs<Password>>(OnPasswordsChanged);
            }
        }

        /// <summary>
        /// Gets the list of all <see cref="T:Password"/>s that are either associated to this <see cref="T:Folder"/>.
        /// or any descendend <see cref="T:Folder"/>.
        /// </summary>
        private IEnumerable<Password> AllNestedPasswords()
        {
            foreach (var p in Passwords) yield return p;
            foreach (var folder in Folders)
            {
                foreach (var p in folder.NestedPasswords)
                {
                    yield return p;
                }
            }
        }

        public IEnumerable<Password> NestedPasswords
        {
            get
            {
                var array = AllNestedPasswords().ToArray();
                return array.Distinct().OrderBy(p => p.Order);
            }
        }

        public int NestedPasswordCount
        {
            get { return this.NestedPasswords.Count(); }
        }


        void OnPasswordsChanged(object sender, NotifyEventArgs<Password> e)
        {
            OnPropertyChanged("Passwords");
        }

        void OnPasswordPropertyChanged()
        {
            Folder folder = this;
            while (folder != null)
            {
                folder.EnsurePasswords();
                folder.OnPropertyChanged("NestedPasswords");
                folder = folder.Parent;
            }
        }

        /// <summary>
        /// Gets the list of child <see cref="T:Folder"/>s.
        /// </summary>
        public NotifyList<Folder> Folders
        {
            get
            {
                if (folders == null)
                {
                    folders = Id == 0 ? new NotifyList<Folder>() : new NotifyList<Folder>(Context.GetFoldersByParentFolder(this));
                    folders.Changed += new EventHandler<NotifyEventArgs<Folder>>(OnFoldersChanged);
                }
                return folders;
            }
        }

        void OnFoldersChanged(object sender, NotifyEventArgs<Folder> e)
        {
            ReorderChildren();

            Folder folder = e.Item;
            if (folder != null) folder.NotifyPasswordChanged();
            OnPropertyChanged("Folders");
        }

        private void ReorderChildren()
        {
            short order = 0;
            foreach (Folder folder in Folders)
            {
                short oldOrder = folder.Order;
                folder.Order = order;
                if (oldOrder != order) folder.Save();
                order++;
            }
        }

        /// <summary>
        /// Saves the <see cref="T:Folder"/> to the data repository.
        /// </summary>
        public void Save()
        {
            Context.SaveFolder(this);
            IsModified = false;
        }

        /// <summary>
        /// Notify the folder that the list of <see cref="T:Password"/>s has changed.
        /// </summary>
        public void NotifyPasswordChanged()
        {
            OnPropertyChanged("Passwords");
        }

        /// <summary>
        /// Resets all data.
        /// </summary>
        protected internal override void Reset()
        {
            folders = null;
            passwords = null;
            OnPropertyChanged("Folders");
            OnPropertyChanged("Passwords");
            base.Reset();
        }


        /// <summary>
        /// Gets the parent of this <see cref="T:NodeBase"/>
        /// </summary>
        protected internal override NodeBase ParentNode
        {
            get
            {
                return Parent != null ? Parent : base.ParentNode;
            }
            set
            {
                base.ParentNode = value;
            }
        }

        /// <summary>
        /// Moves this <see cref="T:Folder"/> on level up in order.
        /// </summary>
        public void Up()
        {
            if (Parent != null) Parent.Folders.MoveUp(this);
        }

        /// <summary>
        /// Moves this <see cref="T:Folder"/> on level down in order.
        /// </summary>
        public void Down()
        {
            if (Parent != null) Parent.Folders.MoveDown(this);
        }

        /// <summary>
        /// Moves this <see cref="T:Folder"/> to the top of order.
        /// </summary>
        public void Top()
        {
            if (Parent != null) Parent.Folders.MoveToTop(this);
        }

        /// <summary>
        /// Moves this <see cref="T:Folder"/> to the bottom of order.
        /// </summary>
        public void Bottom()
        {
            if (Parent != null) Parent.Folders.MoveToBottom(this);
        }

        public Folder AddFolder(string name)
        {
            Folder folder = new Folder(0, name, this);
            Folders.Add(folder);
            folder.Save();
            return folder;
        }

        public void Delete()
        {
            if (Folders.Any()) throw new ArgumentOutOfRangeException("Folder has descendants.");
            if (Parent != null)
            {
                foreach (Password pw in Passwords)
                {
                    foreach (PasswordFolder pf in pw.PasswordFolders.Where(x => x.Folder == this))
                    {
                        pf.IsFavorite = false;
                    }
                }
                Parent.Folders.Remove(this);
                Context.DeleteFolder(this);
            }
        }
    }
}
