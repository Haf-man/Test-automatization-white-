using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AutomationTree
{
    public class ControlNode: IAutomationNode
    {
        public string Name
        {
            get;
            set;
        }
        public string Id
        {
            get;
            set;
        }
        public string Type
        {
            get;
            set;            
        }
        public List<string> Path
        {
            get;
            set;
        }
  
        public ControlNode Parent
        {
            get;
            private set;
        }

     
        public List<ControlNode> Children 
        {
            get;
            private set;
        }

        public List<PropertiesNode> Properties
        {
            get;
            private set;
        }
        public List<PatternNode> Paterns
        {
            get;
            private set;
        }
       
        public ControlNode(string name, string id, string type,  List<string> path )
        {
            Name = name;
            Id = id;
            Type = type;
            Path = path;
            Parent = null;
            Children = new List<ControlNode>();
            Properties = new List<PropertiesNode>();
            Paterns = new List<PatternNode>();
        }
        public ControlNode(ControlNode parent, string name, string automationId, string controlType )
        {
            Name = name;
            Id = automationId;
            Type = controlType;
            Parent = parent;
            List<string> path = new List<string>();
            path = parent.Path.ToList();
            path.Add(parent.Id + parent.Type.Split('.').Last());
            Path = path;
            Children = new List<ControlNode>();
            Properties = new List<PropertiesNode>();
            Paterns = new List<PatternNode>();
        }

        public ControlNode(XmlNode xmlNode)
        {
            if ( xmlNode.Attributes == null)
                throw new Exception("Null at xmlNode.Attributes in ControlNode constuctor");

            Name = xmlNode.Attributes.GetNamedItem("Name").Value;
            Id = xmlNode.Attributes.GetNamedItem("Id").Value;
            Type = xmlNode.Attributes.GetNamedItem("Type").Value;
            Path = PathToList(xmlNode.Attributes.GetNamedItem("Path").Value);

            Children = new List<ControlNode>();
            Properties = new List<PropertiesNode>();
            Paterns = new List<PatternNode>();
        }
        public void AddChild( string name,string id, string type)
        { 
            ControlNode childNode = new ControlNode(this, name, id, type);
            Children.Add(childNode);
        }
        public void AddChild(ControlNode childNode)
        {
            Children.Add(childNode);
        }
        public void AddProperty(string name, string value)
        {
            PropertiesNode propertyNode = new PropertiesNode(name, value);
            Properties.Add(propertyNode);
        }
        public void AddProperty( PropertiesNode propertyNode)
        {
            Properties.Add(propertyNode);
        }
        public void AddPatern(string name)
        {
            PatternNode patternNode = new PatternNode(name);
            Paterns.Add(patternNode);
        }
        public void AddPatern( PatternNode patternNode)
        {
            Paterns.Add(patternNode);
        }
        
        public XmlNode ToXmlNode(XmlDocument xmlDocument)
        {
            XmlNode controlNode = xmlDocument.CreateElement("Control");

            XmlAttribute nameAttribute = xmlDocument.CreateAttribute("Name");
            nameAttribute.Value = Name;
            XmlAttribute idAttribute = xmlDocument.CreateAttribute("Id");
            idAttribute.Value = Id;
            XmlAttribute typeAttribute = xmlDocument.CreateAttribute("Type");
            typeAttribute.Value = Type;
            XmlAttribute hashIdAttribute = xmlDocument.CreateAttribute("Path");
            hashIdAttribute.Value = PathToString();

            if (controlNode.Attributes==null)
                throw new Exception("Null at controlNode.Attributes in ControlNode.ToXmlNode");
            controlNode.Attributes.Append(nameAttribute);
            controlNode.Attributes.Append(idAttribute);
            controlNode.Attributes.Append(typeAttribute);
            controlNode.Attributes.Append(hashIdAttribute);
            return controlNode;
        }
 
        public string PathToString()
        {
            return Path.Aggregate("", (current, node) => current + ("/" + node));
        }

        public List<string> PathToList(string path)
        {
            List<string> convertedPath = path.Split('/').ToList();
            convertedPath.RemoveAt(0);
             return convertedPath;
        }
    }
}
