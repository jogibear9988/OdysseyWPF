using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PasswordSafe.Data.Biz;

namespace PasswordSafe.Classes
{
    /// <summary>
    /// Gets that from <see cref="T:BizContext"/> and is used by <see cref="T:ObjectDataProvider"/>s.
    /// </summary>
    public class UIContext
    {
        private BizContext context
        {
            get
            {
                return BizContext.Instance;
            }
        }

        /// <summary>
        /// Gets the root <see cref="T:Category"/>.
        /// </summary>
        public Category RootCategory
        {
            get
            {
                return context.Categories.FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the root <see cref="T:Folder"/>.
        /// </summary>
        public Folder RootFolder
        {
            get
            {
                return context.Folders.FirstOrDefault();
            }
        }


        /// <summary>
        /// Gets an enumeration of all root <see cref="T:Category"/>s.
        /// </summary>
        public IEnumerable<Category> GetCategories()
        {
            return context.Categories;
        }

        /// <summary>
        /// Gets an enumeration of all root <see cref="T:Folder"/>s.
        /// </summary>
        public IEnumerable<Folder> GetFolders()
        {
            return context.Folders;
        }

        /// <summary>
        /// Gets an enumeration of all <see cref="T:FieldTypes"/>s.
        /// </summary>
        /// <returns>An enumeration of see cref="T:FieldType"/>.</returns>
        public IEnumerable<FieldType> Types()
        {
            return Enum.GetValues(typeof(FieldType)).Cast<FieldType>();
        }

        private NodeBase breadcrumbRoot;

        /// <summary>
        /// Get the root Node of type <see cref="T:NodeBase"/> for the main breadcrumb.
        /// </summary>
        /// <returns>An enumeration of see cref="T:NodeBase"/>.</returns>
        public NodeBase GetBreadcrumbRoot()
        {
            lock (this)
            {
                if (breadcrumbRoot == null)
                {
                    if (RootFolder != null && RootCategory != null)
                    {
                        breadcrumbRoot = new CustomNode(
                            "Home",
                            new NodeBase[] 
                        {
                            RootFolder,
                            RootCategory
                        }
                        );
                    }
                    else return null;
                }
                return breadcrumbRoot;
            }
        }


        /// <summary>
        /// Gets the root Category.
        /// </summary>
        /// <returns>The root Category.</returns>
        public Category GetCategoryRoot()
        {
            return RootCategory;
        }

        /// <summary>
        /// Gets an enumeration of strings of all avaiable <see cref="T:NodeBase"/>s used for 
        /// a breadcrumb bar.
        /// </summary>
        /// <returns>An enumeration of <see cref="T:NodeBase"/></returns>
        public static IEnumerable<string> GetBreadcrumbDropDownData(NodeBase root)
        {
            if (root != null && root.Children != null)
            {
                foreach (NodeBase node in root.Children)
                {
                    yield return node.GetPath(1);
                    var subNodes = GetSubPath(node);
                    if (subNodes != null)
                    {
                        foreach (string subPath in subNodes)
                        {
                            yield return subPath;
                        }
                    }
                }
            }
        }

        private static IEnumerable<string> GetSubPath(NodeBase node)
        {
            foreach (NodeBase child in GetChildren(node))
            {
                yield return child.GetPath(1);
                foreach (string subPath in GetSubPath(child)) yield return subPath;
            }
        }

        private static IEnumerable<NodeBase> GetChildren(NodeBase node)
        {
            Category c = node as Category;
            if (c != null) return c.Categories.Cast<NodeBase>();

            Folder f = node as Folder;
            if (f != null) return f.Folders.Cast<NodeBase>();

            CustomNode n = node as CustomNode;
            return n.Children;
        }
    }
}
