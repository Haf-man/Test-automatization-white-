using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AutomationTree
{
    public class PatternNode:IAutomationNode
    {
        public string Name
        {
            get; 
            set;
        }

        //public List<PropertiesNode> Properties { get; private set; } 

        public PatternNode(string name)
        {
            Name = name; 
            //Properties = new List<PropertiesNode>();
        }
        public PatternNode(XmlNode xmlNode)
        {
            if ( xmlNode.Attributes == null)
                throw new Exception("Null at xmlNode.Attributes in PatternNode constuctor");
           // Properties = new List<PropertiesNode>();
            Name = xmlNode.Attributes.GetNamedItem("Name").Value;
           
        }
        public XmlNode ToXmlNode(XmlDocument xmlDocument)
        {
            XmlNode patternNode = xmlDocument.CreateElement("Pattern");

            XmlAttribute nameAttribute = xmlDocument.CreateAttribute("Name");
            nameAttribute.Value = Name;

            if(patternNode.Attributes==null)
                throw new Exception("Null at patternNode.Attribute in PatternNode.ToXmlNode");
            patternNode.Attributes.Append(nameAttribute);
             
            return patternNode;
        }

        //public void AddProperty(PropertiesNode propertiesNode)
        //{
        //    Properties.Add(propertiesNode);
        //}
    }
}
