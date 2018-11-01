using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DifferenceTree
{

    public enum Modes
    {
      Usual,
      Special,
      SpecialWithRegularExpression
    };
    public class DifferencePropertyNode
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

        public string ExpectedValue
        {
            get;
            set;
        }

        public Modes Mode { get; set; }

        public string RegExp { get; set; }
        public DifferencePropertyNode(string name, string value, string expectedValue, Modes mode, string regExp = "")
        {
            Name = name;
            Value = value;
            ExpectedValue = expectedValue;
            Mode = mode;
            if (mode != Modes.Usual)
                RegExp = regExp;
             
        }

        public DifferencePropertyNode(XmlNode xmlNode)
        {
            if ( xmlNode.Attributes == null)
                throw new Exception("Null at xmlNode.Attributes in DifferencePropertiesNode constuctor");

            Name = xmlNode.Attributes.GetNamedItem("Name").Value;
            Value = xmlNode.Attributes.GetNamedItem("Value").Value;
            ExpectedValue = xmlNode.Attributes.GetNamedItem("ExpectedValue").Value;
          string mode = xmlNode.Attributes.GetNamedItem("Mode").Value;
          switch (mode)
          {
            case "Special":
              Mode = Modes.Special;
              break;
            case "SpecialWithRegularExpression":
              Mode = Modes.SpecialWithRegularExpression;
              break;
            default:
              Mode = Modes.Usual;
              break;
          }
            
            if (Mode != Modes.Usual)
                RegExp = xmlNode.Attributes.GetNamedItem("RegExp").Value;
        }
        public XmlNode ToXmlNode(XmlDocument xmlDocument)
        {
            XmlNode differenceNode = xmlDocument.CreateElement("Property");

            XmlAttribute nameAttribute = xmlDocument.CreateAttribute("Name");
            nameAttribute.Value = Name;
            XmlAttribute valueAttribute = xmlDocument.CreateAttribute("Value");
            valueAttribute.Value = Value;
            XmlAttribute expectedValueAttribute = xmlDocument.CreateAttribute("ExpectedValue");
            expectedValueAttribute.Value = ExpectedValue;
            XmlAttribute modeAttribute = xmlDocument.CreateAttribute("Mode");
            modeAttribute.Value = Mode.ToString();
            XmlAttribute regExpAttribute = xmlDocument.CreateAttribute("RegExp");
            regExpAttribute.Value = RegExp;

            if (differenceNode.Attributes == null)
                throw new Exception("Null at controlNode.Attributes in ControlNode.ToXmlNode");

            differenceNode.Attributes.Append(nameAttribute);
            differenceNode.Attributes.Append(valueAttribute);
            differenceNode.Attributes.Append(expectedValueAttribute);
            differenceNode.Attributes.Append(modeAttribute);
            differenceNode.Attributes.Append(regExpAttribute); 

            return differenceNode;
        }
    }
}
