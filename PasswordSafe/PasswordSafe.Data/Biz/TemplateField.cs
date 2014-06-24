using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PasswordSafe.Data.Biz
{
    /// <summary>
    /// Represents a field for a categegory template.
    /// </summary>
    public class TemplateField : BaseObject
    {
        /// <summary>
        /// Creates a new <see cref="T:TemplateField"/>.
        /// </summary>
        /// <param name="id">The id from the database, 0 if not attaced.</param>
        /// <param name="name">The name of the field.</param>
        /// <param name="type">The <see cref="T:Type">type</see> of the field.</param>
        /// <param name="category"></param>
        public TemplateField(int id, string name, FieldType type, Category category, string minRange, string maxRange, short order)
            : base(id, name)
        {
            this.Category = category;
            this.Type = type;
            this.minRange = minRange;
            this.maxRange = maxRange;
            this.order = order;
        }

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }


        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName != "IsSelected")
            {
                Category.IsModified = true;
                Category.RaisePropertyChanged("Fields");
            }
        }

        private string minRange;
        private string maxRange;

        public string MinRange
        {
            get { return FormatRange(minRange); }
            set
            {
                if (minRange != value)
                {
                    minRange = value;
                    OnPropertyChanged("MinRange");
                }
            }
        }

        public string MaxRange
        {
            get { return FormatRange(maxRange); }

            set
            {
                if (maxRange != value)
                {
                    maxRange = value;
                    OnPropertyChanged("MaxRange");
                }
            }
        }

        private static DateTime? ToDateTime(string value)
        {
            if (value == null) return null;
            DateTime result;
            if (DateTime.TryParse(value, out result)) return result;
            return null;
        }

        private static int? ToInt(string value)
        {
            if (value == null) return null;
            int result;
            if (int.TryParse(value,out result)) return result;
            return null;
        }


        private string FormatRange(string value)
        {
            if (value == null) return null;
            switch (Type)
            {
                case FieldType.Date:
                    DateTime? dt = ToDateTime(value);
                    return dt.HasValue ? dt.Value.ToShortDateString() : "";

                case FieldType.Time:
                    dt = ToDateTime(value);
                    return dt.HasValue ? dt.Value.ToLongTimeString() : "";

                case FieldType.Int:
                    int? ival = ToInt(value);
                    return ival.HasValue ? ival.Value.ToString() : "";


                default:
                    return value;

            }
        } 

        /// <summary>
        /// Gets the <see cref="T:BizContext"/>.
        /// </summary>
        protected internal override BizContext Context
        {
            get
            {
                return Category.Context;
            }
        }

        /// <summary>
        /// Gets the <see cref="T:Category"/>.
        /// </summary>
        public Category Category { get; private set; }

        /// <summary>
        /// Gets the <see cref="T:FieldType"/>.
        /// </summary>
        public FieldType Type { get; private set; }



        public void Top()
        {
            Category.Fields.MoveToTop(this);
        }

        public void Bottom()
        {
            Category.Fields.MoveToBottom(this);
        }

        public void Up()
        {
            Category.Fields.MoveUp(this);

        }

        public void Down()
        {
            Category.Fields.MoveDown(this);
        }


        public void Delete()
        {
            Category.Fields.Remove(this);
        }
    }
}
