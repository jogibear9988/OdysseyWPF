using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace PasswordSafe.Data.Biz
{

    /// <summary>
    /// Represents the base type of all business objects.
    /// </summary>
    public class BaseObject : INotifyPropertyChanged
    {
        public BaseObject()
            : base()
        {
        }

        public BaseObject(int id, string name)
            : base()
        {
            this.name = name;
            this.Id = id;
        }

        #region INotifyPropertyChanged Members


        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (!Notifying)
            {
                //                Notifying = true;
                try
                {
                    //Debug.WriteLine(string.Format("> Notify on {0}.{1}:  {2}", GetType().Name, Name, propertyName));
                    //Debug.Indent();
                    IsModified = true;
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                    //Debug.Unindent();
                }
                finally
                {
                    Notifying = false;
                }
            }
        }


        protected bool Notifying { get; private set; }

        public void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private bool isModified;

        /// <summary>
        /// Gets whether data is modified.
        /// </summary>
        public bool IsModified
        {
            get { return isModified; }
            internal set
            {
                if (isModified != value)
                {
                    isModified = value;
                    OnModifiedChanged();
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsModified"));
                    isModified = value;
                }
            }
        }

        protected virtual void OnModifiedChanged()
        {
        }

        /// <summary>
        /// Gets the id of the data repository.
        /// </summary>
        public int Id { get; internal set; }

        protected short order;

        /// <summary>
        /// Gets the order in the repository.
        /// </summary>
        public short Order
        {
            get { return order; }
            internal set
            {
                if (order != value)
                {
                    order = value;
                    IsModified = true;
                }
            }
        }

        private string name;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public virtual string Name
        {
            get
            {
                return name != null ? name : string.Empty;
            }
            set
            {
                if (name != null)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="T:BizContext"/>.
        /// </summary>
        internal protected virtual BizContext Context
        {
            get { return BizContext.Instance; }
        }

        /// <summary>
        /// Resets all data.
        /// </summary>
        internal protected virtual void Reset()
        {
            IsModified = false;
        }

    }
}
