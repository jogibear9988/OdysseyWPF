using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PasswordSafe.Data.Biz
{
    /// <summary>
    /// Enumeration of possible field types.S
    /// </summary>
    public enum FieldType
    {
        Unknown = 0,
        Text = 1,
        Password = 2,
        Date = 3,
        Time = 4,
        Int = 5,
        Memo = 6,
        Separator = 7,
        Bool = 8
    }
}
