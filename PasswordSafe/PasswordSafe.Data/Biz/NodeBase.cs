using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PasswordSafe.Data.Biz
{
    /// <summary>
    /// Represents the base class for hierarchical objects.
    /// </summary>
    public class NodeBase:BaseObject
    {
        public NodeBase(int id, string name)
            : base(id, name)
        {
        }

        /// <summary>
        /// Gets the hierarchical Path of all descendant Name properties separated with a backslash.
        /// </summary>
        public string Path
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                NodeBase parent = this;

                while (parent != null)
                {
                    sb.Insert(0, parent.Name);
                    parent = parent.ParentNode;
                    if (parent != null) sb.Insert(0, '\\');
                }
                return sb.ToString();
            }
        }


        /// <summary>
        /// Gets the parent <see cref="T:NodeBase"/> otherwise null.
        /// </summary>
        internal protected virtual NodeBase ParentNode { get; set; }

        public string GetPath(int depth)
        {
            StringBuilder sb = new StringBuilder();
            NodeBase parent = this;

            while (parent != null && parent.ParentNode != null)
            {
                sb.Insert(0, parent.Name);
                parent = parent.ParentNode;
                if (parent != null && parent.ParentNode != null) sb.Insert(0, '\\');
            }
            string s = sb.ToString();
            if (depth > 1)
            {
                while (--depth > 0)
                {
                    int idx = s.IndexOf('\\');
                    if (idx > 0) s = s.Substring(idx + 1);
                }
            }
            return s;
        }

        private IEnumerable<NodeBase> children;

        /// <summary>
        /// Gets all child <see cref="T:NodeBase"/> of this NodeBase.
        /// </summary>
        public virtual IEnumerable<NodeBase> Children
        {
            get { return children; }
            set
            {
                children = value;
                foreach (var child in children) child.ParentNode = this;
            }
        }

    }
}