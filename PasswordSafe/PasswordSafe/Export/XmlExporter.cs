using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PasswordSafe.Data.Biz;
using System.Xml.Serialization;
using System.IO;

namespace PasswordSafe.Export
{
    static class XmlExporter
    {

        public static string Export()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PasswordExport));

            PasswordExport data = new PasswordExport { Passwords = GetPasswords() };
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, data);
                writer.Flush();
                string s = writer.ToString();
                return s;
            }
        }

        public static void Export(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PasswordExport));

            PasswordExport data = new PasswordExport { Passwords = GetPasswords() };
            serializer.Serialize(stream, data);
        }


        private static XmlPassword[] GetPasswords()
        {
            BizContext context = BizContext.Instance;

            return context.Categories.FirstOrDefault().NestedPasswords.Select(p => new XmlPassword
            {
                Name = p.Name,
                Category = p.Category.GetPath(2),
                Fields = p.Fields.Where(f => f.Type != FieldType.Separator).Select(f => new XmlField { Name = f.Name, Value = f.StringValue, Type = f.Type.ToString() }).ToArray()
            }).ToArray();
        }


    }

    [XmlRoot(ElementName = "PasswordSafe")]
    public class PasswordExport
    {
        [XmlArrayItem("Password")]
        public XmlPassword[] Passwords { get; set; }
    }
}
