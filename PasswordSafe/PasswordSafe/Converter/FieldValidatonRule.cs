using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;
using PasswordSafe.Data.Biz;

namespace PasswordSafe.Converter
{
    public class FieldValidationRule : ValidationRule
    {
        private FieldType type = FieldType.Text;

        public FieldType Type { get { return type; } set { type = value; } }


        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value is string)
            {
                switch (Type)
                {
                    case FieldType.Date:
                        DateTime dt;
                        if (!DateTime.TryParse(value as string, out dt) || dt.Hour != 0 || dt.Minute != 0) return new ValidationResult(false, "Invalid time format.");
                        break;

                    case FieldType.Time:
                        if (!DateTime.TryParse(value as string, out dt)) return new ValidationResult(false, "Invalid date format.");
                        break;

                    case FieldType.Int:
                        int ival;
                        if (!(int.TryParse(value as string, out ival))) return new ValidationResult(false, "Invalid integer format.");
                        break;
                }

            }
            return new ValidationResult(true, null);
        }
    }
}
