using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PasswordSafe.Data.Biz
{
    public class Category : NodeBase
    {
        public Category(int id, string name, short order, Category parent)
            : base(id, name)
        {
            this.parent = parent;
            this.order = order;
        }
        public override IEnumerable<NodeBase> Children
        {
            get
            {
                return Categories.Cast<NodeBase>();
            }
            set
            {
                base.Children = value;
            }
        }

        internal Category(int id, string name, short order, int? parentId)
            : base(id, name)
        {
            this.parentId = parentId;
            this.Order = order;
        }

        public Category(int id, string name, short order)
            : base(id, name)
        {
            this.Order = order;
        }
        protected override void OnModifiedChanged()
        {
            base.OnModifiedChanged();
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case "Order":
                    if (Parent != null) Parent.ReorderChildren();
                    break;

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

        private void ReorderChildren()
        {
            short order = 0;
            foreach (Category c in Categories)
            {
                short oldOrder = c.Order;
                c.Order = order;
                if (oldOrder != order) c.Save();
                order++;
            }
        }

        /// <summary>
        /// Saves the <see cref="T:Category"/> to the data repositoy.
        /// </summary>
        public void Save()
        {
            Context.SaveCategory(this);
            IsModified = false;
        }

        private void OnPasswordPropertyChanged()
        {
            Category category = this;
            while (category != null)
            {
                category.OnPropertyChanged("NestedPasswords");
                category = category.Parent;
            }
        }

        private Category parent;
        private int? parentId;

        /// <summary>
        /// Gets the parent <see cref="T:Category"/>, otherwise null.
        /// </summary>
        public Category Parent
        {
            get
            {
                if (parentId.HasValue && parent == null)
                {
                    parent = Context.GetCategory(parentId.Value);
                }
                return parent;
            }
        }

        private NotifyList<Password> passwords;

        /// <summary>
        /// Gets a list of all <see cref="T:Password"/>s associated with to category.
        /// </summary>
        public NotifyList<Password> Passwords
        {
            get
            {
                if (passwords == null)
                {
                    passwords = Id == 0 ? new NotifyList<Password>() : new NotifyList<Password>(Context.GetPasswords(this));
                    passwords.Changed += new EventHandler<NotifyEventArgs<Password>>(OnPasswordsChanged);
                }
                return passwords;
            }
        }

        private NotifyList<TemplateField> fields;

        /// <summary>
        /// Gets a list of all <see cref="T:TemplateField"/>s associated to this category.
        /// </summary>
        public NotifyList<TemplateField> Fields
        {
            get
            {
                if (fields == null)
                {
                    fields = Id == 0 ? new NotifyList<TemplateField>() : new NotifyList<TemplateField>(Context.GetTemplates(this));
                    fields.Changed += new EventHandler<NotifyEventArgs<TemplateField>>(OnFieldsChanged);
                }
                return fields;
            }
        }

        void OnFieldsChanged(object sender, NotifyEventArgs<TemplateField> e)
        {
            OnPropertyChanged("Fields");
        }

        /// <summary>
        /// Gets an enumeration of all nested <see cref="T:Password"/>s that are associated
        /// either to this Category or any descendent <see cref="T:Category"/>.
        /// </summary>
        public IEnumerable<Password> NestedPasswords
        {
            get
            {
                foreach (var p in Passwords) yield return p;
                foreach (var c in Categories)
                {
                    foreach (var p in c.NestedPasswords)
                    {
                        yield return p;
                    }
                }
            }
        }


        public NotifyList<Category> categories;

        /// <summary>
        /// Gets a list ofild ch <see cref="T:Category"/>s.
        /// </summary>
        public NotifyList<Category> Categories
        {
            get
            {
                if (categories == null)
                {
                    categories = Id == 0 ? new NotifyList<Category>() : new NotifyList<Category>(Context.GetCategories(this));
                    categories.Changed += new EventHandler<NotifyEventArgs<Category>>(OnCategoriesChanged);

                }
                return categories;
            }
        }

        /// <summary>
        /// Occurs when the list of categories has changed.
        /// </summary>
        protected void OnCategoriesChanged(object sender, NotifyEventArgs<Category> e)
        {
            ReorderChildren();
            OnPropertyChanged("Categories");
        }

        /// <summary>
        /// Occurs when the list of passwords has changed.
        /// </summary>
        protected void OnPasswordsChanged(object sender, NotifyEventArgs<Password> e)
        {
            OnPropertyChanged("Passwords");

            Category parent = Parent;
            while (parent != null)
            {
                parent.OnPropertyChanged("NestedPasswords");
                parent = parent.Parent;
            }
        }

        /// <summary>
        /// Gets the number of nested <see cref="T:Password"/>s.
        /// </summary>
        public int NestedPasswordCount
        {
            get { return this.NestedPasswords.Count(); }
        }

        /// <summary>
        /// Resets all data.
        /// </summary>
        protected internal override void Reset()
        {
            categories = null;
            passwords = null;
            fields = null;
            OnPropertyChanged("Categories");
            OnPropertyChanged("Passwords");
            OnPropertyChanged("Fields");
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
        /// Creates a new password for this <see cref="Category"/>.
        /// </summary>
        /// <returns>The new  <see cref="T:PasswordSafe.Data.Biz.Category"/></returns>
        public Password CreatePassword()
        {
            Password newPassword = new Password(this, 0, "New Password");

            short order = 0;
            foreach (TemplateField tempField in this.Fields)
            {
                Field field = new Field(newPassword, 0, tempField.Name, tempField.Type, null, order++);
                newPassword.Fields.Add(field);
            }

            this.Passwords.Add(newPassword);
            return newPassword;
        }


        internal void ResetPasswords()
        {
            OnPropertyChanged("Passwords");
        }

        /// <summary>
        /// Moves the <see cref="T:Category"/> on level up in order.
        /// </summary>
        public void Up()
        {
            if (Parent != null) Parent.Categories.MoveUp(this);
        }

        /// <summary>
        /// Moves the <see cref="T:Category"/> on level down in order.
        /// </summary>
        public void Down()
        {
            if (Parent != null) Parent.Categories.MoveDown(this);
        }

        /// <summary>
        /// Moves the <see cref="T:Category"/> to the top of order.
        /// </summary>
        public void Top()
        {
            if (Parent != null) Parent.Categories.MoveToTop(this);
        }


        /// <summary>
        /// Moves the <see cref="T:Category"/> to the bottom of order.
        /// </summary>
        public void Bottom()
        {
            if (Parent != null) Parent.Categories.MoveToBottom(this);
        }

        public void Delete()
        {
            if (Categories.Any()) throw new ArgumentOutOfRangeException("Category has descendants.");
            if (Passwords.Any()) throw new ArgumentOutOfRangeException("Category has associated Passwords.");
            if (Parent != null)
            {
                Parent.Categories.Remove(this);
                Context.DeleteCategory(this);
            }
        }

        /// <summary>
        /// Adds a new child Category.
        /// </summary>
        /// <param name="name">The name of the new child Category.</param>
        /// <returns>The new Category.</returns>
        public Category AddCategory(string name)
        {
            Category category = new Category(0, name, (short)Categories.Count, this);
            Categories.Add(category);
            category.Save();
            foreach (TemplateField field in Fields)
            {
                TemplateField copy = new TemplateField(0, field.Name, field.Type, category, field.MinRange, field.MaxRange, (short)Fields.Count);
                copy.IsModified = true;
                category.Fields.Add(copy);
            }
            category.SaveTemplate();
            return category;
        }

        public void SaveTemplate()
        {
            Context.SaveTemplate(this);
        }

        /// <summary>
        /// Undoing a template is very easy: 
        /// Since there are only the TemplateFields affected, the Fields list must only be recreated to gather them from Database again.
        /// </summary>
        public void UndoTemplate()
        {
            fields = null;
            RaisePropertyChanged("Fields");
            IsModified = false;
        }

        /// <summary>
        /// Adds a <see cref="T:TemplateField"/> to the template.
        /// </summary>
        /// <param name="type">The type of the new field.</param>
        /// <param name="name">The name of the new field.</param>
        /// <returns>The new <see cref="T:TemplateField/> added to <see cref="T:Category.Fields"/>.</returns>
        public TemplateField AddField(FieldType type, string name)
        {
            TemplateField field = new TemplateField(0, name, type, this, "", "", (short)Fields.Count);
            field.IsModified = true;
            Fields.Add(field);
            return field;
        }
    }
}
