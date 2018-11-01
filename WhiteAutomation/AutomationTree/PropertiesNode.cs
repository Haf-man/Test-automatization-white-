using System;
using System.Xml;

namespace AutomationTree
{
    public class PropertiesNode:IAutomationNode
    {
        public string Name
        {
            get; 
            set;
        }
        public string Value
        {
            get;
            set;
        }
        public bool IsValueSequence { get; set; }
        public PropertiesNode(string name, string value, bool isValueSequence = false)
        {
            Name = name;
            Value = value;
            IsValueSequence = isValueSequence;
        }
        public PropertiesNode(XmlNode xmlNode)
        {
            if ( xmlNode.Attributes == null)
                throw new Exception("Null at xmlNode.Attributes in PropertiesNode constuctor");

            Name = xmlNode.Attributes.GetNamedItem("Name").Value;
            Value = xmlNode.Attributes.GetNamedItem("Value").Value;
            IsValueSequence = bool.Parse(xmlNode.Attributes.GetNamedItem("IsValueSequence").Value);
        }
        public XmlNode ToXmlNode(XmlDocument xmlDocument)
        {
            XmlNode propertyNode = xmlDocument.CreateElement("Property");
            XmlAttribute nameAttribute = xmlDocument.CreateAttribute("Name");
            nameAttribute.Value = Name;
            XmlAttribute valueAttribute = xmlDocument.CreateAttribute("Value");
            valueAttribute.Value = Value;
            XmlAttribute isValueSequenceAttribute = xmlDocument.CreateAttribute("IsValueSequence");
            isValueSequenceAttribute.Value = IsValueSequence.ToString();

            if (propertyNode.Attributes==null)
                throw new Exception("Null at propertyNode.Attributes in PropertiesNode.ToXmlNode");

            propertyNode.Attributes.Append(nameAttribute);
            propertyNode.Attributes.Append(valueAttribute);
            propertyNode.Attributes.Append(isValueSequenceAttribute);

            return propertyNode;
        }
        
    }
}
