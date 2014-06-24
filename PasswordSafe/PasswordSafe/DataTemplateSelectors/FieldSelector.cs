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
    public class FieldSelector : DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            Field field = item as Field;
            Window window = Application.Current.MainWindow;
            if (field != null && field.Type != FieldType.Unknown)
            {
                switch (field.Type)
                {
                    case FieldType.Bool:
                        return window.FindResource("BoolFieldTemplate") as DataTemplate;

                    case FieldType.Int:
                        return window.FindResource("IntFieldTemplate") as DataTemplate;

                    case FieldType.Date:
                        return window.FindResource("DateFieldTemplate") as DataTemplate;

                    case FieldType.Time:
                        return window.FindResource("TimeFieldTemplate") as DataTemplate;

                    case FieldType.Memo:
                        return window.FindResource("MemoFieldTemplate") as DataTemplate;

                    case FieldType.Separator:
                        return window.FindResource("SeparatorFieldTemplate") as DataTemplate;

                    default:
                        return window.FindResource("PasswordFieldTemplate") as DataTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
