using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;

namespace PasswordSafe.Data.Biz
{
    /// <summary>
    ///  <see cref=T:Field"/> is disposable since it contains confidential data that should be disposed from memory asap.
    /// </summary>
    public class Field : BaseObject, IDisposable
    {
        public Field(Password password, int id, string name, FieldType type, object value, short order)
            : base(id, name)
        {
            this.Password = password;
            this.Type = type;
            if (IsString) ChangeSecureString(value, false); else this.value = value;
            this.Order = order;
        }

        #region IDisposable Members

        public void Dispose()
        {
            secureStringValue.Dispose();
        }

        #endregion

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case "Value":
                    base.OnPropertyChanged("StringValue");
                    break;

                case "StringValue":
                    base.OnPropertyChanged("Value");
                    break;
            }
            if (propertyName != "IsSelected")
            {
                Password.IsModified = true;
                Password.RaisePropertyChanged("Fields");
            }
        }

        protected internal override BizContext Context
        {
            get
            {
                return Password.Context;
            }
        }

        public Password Password { get; private set; }

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

        public bool HasLines { get { return Type == FieldType.Memo; } }
        public bool HasRange { get { return Type != FieldType.Separator && Type != FieldType.Bool && Type != FieldType.Memo; } }

        private int? minLines;
        private int? maxLines;
        private object minRange;
        private object maxRange;

        /// <summary>
        /// Gets or sets the minimum number of lines, otherwise null.
        /// </summary>
        public int? MinLines
        {
            get { return minLines; }
            set
            {
                if (minLines != value)
                {
                    minLines = value;
                    OnPropertyChanged("MinLines");
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of lines, otherwise null.
        /// </summary>
        public int? MaxLines
        {
            get { return maxLines; }
            set
            {
                if (maxLines != value)
                {
                    maxLines = value;
                    OnPropertyChanged("MaxLines");
                }
            }
        }

        /// <summary>
        /// Gets or sets  the minimum range converted as string.
        /// </summary>
        public string MinRangeString
        {
            get { return ValueToString(minRange, RangeType); }
            set
            {
                minRange = StringToValue(value, RangeType);
                OnPropertyChanged("MinRangeString");
            }
        }

        /// <summary>
        /// Gets or sets  the maximum range converted as string.
        /// </summary>
        public string MaxRangeString
        {
            get { return ValueToString(maxRange, RangeType); }
            set
            {
                maxRange = StringToValue(value, RangeType);
                OnPropertyChanged("MaxRangeString");
            }
        }

        /// <summary>
        /// Gets or sets the minimum range. The value type is depending on <see cref="T:Field.Type"/>.
        /// </summary>
        public object MinRange
        {
            get { return minRange; }
            set
            {
                if (minRange != value)
                {
                    minRange = value;
                    OnPropertyChanged("MinRange");
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum range. The value type is depending on <see cref="T:Field.Type"/>.
        /// </summary>
        public object MaxRange
        {
            get { return maxRange; }
            set
            {
                if (maxRange != value)
                {
                    maxRange = value;
                    OnPropertyChanged("MaxRange");
                }
            }
        }

        /// <summary>
        /// Gets the type to be used for MimimumRange and MaximumRange.
        /// </summary>
        protected FieldType RangeType
        {
            get
            {
                switch (Type)
                {
                    case FieldType.Memo:
                    case FieldType.Text:
                    case FieldType.Password:
                        return FieldType.Int;

                    default: return Type;
                }
            }
        }

        /// <summary>
        /// Converts a string to a value type.
        /// </summary>
        /// <param name="str">The string representation of the value to convert. </param>
        /// <param name="type">The type to convert to.</param>
        /// <returns>The converted value.</returns>
        protected object StringToValue(string str, FieldType type)
        {
            switch (type)
            {
                case FieldType.Date:
                case FieldType.Time:
                    if (str == null) return null;
                    DateTime dval;
                    if (DateTime.TryParse(str, out dval)) return dval;
                    return null;

                case FieldType.Int:
                    if (str == null) return null;
                    int iVal;
                    if (int.TryParse(str, out iVal)) return iVal;
                    return null;

                default:
                    return str;
            }
        }

        /// <summary>
        /// Converts a value to string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="type">The value type.</param>
        /// <returns>The converted value as string representation.</returns>
        protected string ValueToString(object value, FieldType type)
        {
            if (value == null) return null;
            switch (type)
            {
                case FieldType.Date:
                    return ((DateTime)value).ToShortDateString();

                case FieldType.Time:
                    return ((DateTime)value).ToLongTimeString();

                case FieldType.Int:
                    return ((int)value).ToString();

                case FieldType.Text:
                case FieldType.Password:
                case FieldType.Memo:
                    return value.ToString();

                default:
                    return String.Empty;
            }
        }

        internal void SetRange(object min, object max)
        {
            minRange = min;
            maxRange = max;
        }

        internal void SetLines(int? min, int? max)
        {
            minLines = min;
            maxLines = max;
        }

        private object value;

        private SecureString secureStringValue = new SecureString();

        /// <summary>
        /// Gets or sets the Value of the Field. The type of the Value depends on <see cref="T:Field.Type"/>.
        /// </summary>
        public object Value
        {
            get
            {
                return IsString ? Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(secureStringValue)) : value;
            }
            set
            {
                if (IsString)
                {
                    ChangeSecureString(value, true);
                }
                else
                {
                    ChangeValue(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Value as string representation.
        /// This Property is usefull for instance for transfering a value from and to a TextBox.
        /// </summary>
        public string StringValue
        {
            get { return ValueToString(Value, Type); }
            set
            {
                object v = StringToValue(value, Type);
                if (v != this.value)
                {
                    this.Value = v;
                    OnPropertyChanged("StringValue");
                }
            }
        }

        public bool BoolValue
        {
            get { return (value != null && value.Equals(1)); }
            set
            {
                Value = value ? 1 : 0;
            }
        }

        private void ChangeValue(object value)
        {
            if (this.value != value)
            {
                this.value = value;
                OnPropertyChanged("Value");
            }
        }

        private void ChangeSecureString(object value, bool signal)
        {
            if (Value != value)
            {
                string s = value as string;
                secureStringValue.Clear();
                if (!string.IsNullOrEmpty(s))
                {
                    foreach (char c in s.ToCharArray()) secureStringValue.AppendChar(c);
                }
            }
            if (signal) OnPropertyChanged("Value");
        }

        private static FieldType[] stringTypes = new FieldType[] { FieldType.Text, FieldType.Password, FieldType.Memo };

        public bool IsString
        {
            get
            {
                return stringTypes.Contains(Type);
            }
        }

        public FieldType Type { get; private set; }

        /// <summary>
        /// Deletes the Field and all dependencies.
        /// </summary>
        public void Delete()
        {
            Password.Fields.Remove(this);
        }

        /// <summary>
        /// Moves the Field on level up in the Password.
        /// </summary>
        public void Up()
        {
            Password.Fields.MoveUp(this);
        }

        /// <summary>
        /// Moves the Field on level down in the Password.
        /// </summary>
        public void Down()
        {
            Password.Fields.MoveDown(this);
        }

        /// <summary>
        /// Moves the Field to the top of the Password.
        /// </summary>
        public void Top()
        {
            Password.Fields.MoveToTop(this);
        }

        /// <summary>
        /// Moves the Field to the bottom of the Password.
        /// </summary>
        public void Bottom()
        {
            Password.Fields.MoveToBottom(this);
        }

        /// <summary>
        /// Converts the value to string.
        /// </summary>
        /// <returns></returns>
        public string ValueAsString()
        {
            string value = ValueToString(this.value, Type);
            return value != null ? value : String.Empty;
        }

        /// <summary>
        /// Validates a value by Range.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>True, if the value is valid and not outside range, otherwise false.</returns>
        public bool Validate(string value)
        {
            object native = StringToValue(value, Type);
            if (native == null) return true;
            if (minRange != null && Compare(minRange, native) < 0) return false;
            if (maxRange != null && Compare(maxRange, native) > 0) return false;
            return true;
        }

        private int Compare(object a, object b)
        {
            if ((a is DateTime) && (b is DateTime)) return ((DateTime)a).CompareTo((DateTime)b);
            if ((a is int) && (b is int)) return ((int)a).CompareTo((int)b);

            if (a is int)
            {
                int n = (b.ToString()).Length;
                return ((int)a).CompareTo(n);
            }
            return 0;
        }

    }
}
