using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using PasswordSafe.Data.Biz;
using System.Windows;

namespace PasswordSafe.DataTemplateSelectors
{
    public class TemplateFieldSelector : DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            TemplateField field = item as TemplateField;
            Window window = Application.Current.MainWindow;
            if (field != null && field.Type != FieldType.Unknown)
            {
                switch (field.Type)
                {
                    case FieldType.Bool:
                        return window.FindResource("BoolFieldTempTemplate") as DataTemplate;

                    case FieldType.Int:
                        return window.FindResource("IntFieldTempTemplate") as DataTemplate;

                    case FieldType.Time:
                    case FieldType.Date:
                        return window.FindResource("DateFieldTempTemplate") as DataTemplate;

                    case FieldType.Memo:
                        return window.FindResource("MemoFieldTempTemplate") as DataTemplate;

                    case FieldType.Separator:
                        return window.FindResource("SeparatorFieldTempTemplate") as DataTemplate;

                    default:
                        return window.FindResource("PasswordFieldTempTemplate") as DataTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
