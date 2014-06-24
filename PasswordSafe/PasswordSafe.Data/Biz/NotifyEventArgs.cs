using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PasswordSafe.Data.Biz
{
    /// <summary>
    /// EventArgs for <see cref="T:NotfiyList.Changed"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NotifyEventArgs<T> : EventArgs
    {
        public NotifyEventArgs(ChangedEventType type, T item)
            : base()
        {
            Type = type;
            Item = item;
        }

        /// <summary>
        /// Gets the change type.
        /// </summary>       
         public ChangedEventType Type { get; private set; }

        /// <summary>
        /// Gets the item that has changed, otherwise null
        /// </summary>
        public T Item { get; private set; }
    }

    /// <summary>
    /// Enumeration of possible changes.
    /// </summary>
    public enum ChangedEventType
    {
        Added, 
        Removed, 
        Modified,
        Reset
    }
}
