using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PasswordSafe.Export
{
    [XmlRoot(ElementName="Password")]
    public class XmlPassword
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Category { get; set; }

        [XmlArrayItem("Field")]
        public XmlField[] Fields { get; set; }

    }
}
