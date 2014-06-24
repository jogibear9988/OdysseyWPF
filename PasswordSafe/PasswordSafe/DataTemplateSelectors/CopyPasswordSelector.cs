using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using PasswordSafe.Classes;
using PasswordSafe.Data.Biz;

namespace PasswordSafe.DataTemplateSelectors
{
    public class CopyPasswordSelector:DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            Field field = item as Field;
            Window window = Application.Current.MainWindow;
            if (field != null)
            {
                switch (field.Type)
                {
                    case FieldType.Separator:
                        return window.FindResource("DropDownPasswordSeparatorFieldTemplate") as DataTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
