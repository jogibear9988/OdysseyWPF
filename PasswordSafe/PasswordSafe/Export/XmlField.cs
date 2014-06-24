using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PasswordSafe.Export
{
    [XmlRoot(ElementName="Field")]
    public class XmlField
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlText]
        public string Value { get; set; }

        [XmlAttribute]
        public string Type { get; set; }
    }
}
