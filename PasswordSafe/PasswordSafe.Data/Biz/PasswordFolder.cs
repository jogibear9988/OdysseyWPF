using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PasswordSafe.Data.Biz
{
    /// <summary>
    /// Represents a link of one n:m relation between <see cref="T:Password"/> and <see cref="T:Folder"/>
    /// This is a helper class for UI.
    /// </summary>
    public class PasswordFolder:BaseObject
    {
        public PasswordFolder(Folder folder, Password password)
            : base()
        {
            this.Folder = folder;
            this.Password = password;
        }

        public PasswordFolder(Folder folder, Password password, PasswordFolder parent)
            : base()
        {
            this.Folder = folder;
            this.Password = password;
            this.Parent = parent;
        }

        public PasswordFolder Parent { get; private set; }
   

        protected override void OnPropertyChanged(string propertyName)
        {
            if (propertyName == "Folders")
            {
                folders = null;
                OnPropertyChanged("IsFavorite");
            }
            base.OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Gets the <see cref="T:Password"/>
        /// </summary>
        public Password Password { get; private set; }

        /// <summary>
        /// Gets the <see cref="T:Folder"/>
        /// </summary>
        public Folder Folder { get; private set; }

        /// <summary>
        /// Gets the name of the <see cref="T:Folder"/>.
        /// </summary>
        public override string Name { get { return Folder.Name; } }

        private IEnumerable<PasswordFolder> folders;

        /// <summary>
        /// Gets an enumeration of all child <see cref="T:Folder"/>s otherwise null.
        /// </summary>
        public IEnumerable<PasswordFolder> Folders
        {
            get
            {
                if (folders == null)
                {
                    folders = Folder.Folders.Select(x => new PasswordFolder(x, Password, this)).ToArray();
                }
                return folders;
            }
        }

        /// <summary>
        /// Gets sets whether the link is a favorite.
        /// </summary>
        public bool IsFavorite
        {
            get
            {
                return Password.Folders.Where(f => f.Id == Folder.Id).Any();                
            }
            set
            {
                if (IsFavorite != value)
                {

                    bool isPasswordFavorite = Password.IsFavorite;
                    Folder folder = Folder;
                    if (folder != null)
                    {
                        if (!value)
                        {
                            folder.Passwords.Remove(Password);
                            Password.Folders.Remove(folder);
                        }
                        else
                        {
                            folder.Passwords.Add(Password);
                            Password.Folders.Add(folder);
                        }
                        OnPropertyChanged("IsFavorite");

                        if (!value)
                        {
                            ForwardIsFaovriteChanged();
                        }
                        else
                        {
                            if (Parent != null) Parent.IsFavorite = true;
                        }
                    }
                    Password.IsModified = true;

                    //// allways notify, even if IsFavorite has not changed just to mark Password as being modified:
                    Password.RaisePropertyChanged("IsFavorite");
                    folder.RaisePropertyChanged("PasswordFolders");
                }
            }
        }


        private void ForwardIsFaovriteChanged()
        {
            foreach (PasswordFolder folder in this.Folders)
            {
                folder.IsFavorite = false;
            }
        }
    }
}
