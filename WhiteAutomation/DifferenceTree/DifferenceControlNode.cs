using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AutomationTree;

namespace DifferenceTree
{
  public class DifferenceControlNode
  {
    public string Name { get; set; }

    public string ExpectedName { get; set; }
    public string Id { get; set; }
    public string Type { get; set; }
    public List<string> Path { get; set; }


    public List<DifferencePropertyNode> DifferentProperties { get; set; }

    public List<DifferenceControlNode> Children { get; set; }

    public List<int> TechPath { get; set; }

    public DifferenceControlNode(string name)
    {
      Name = name;
      DifferentProperties = new List<DifferencePropertyNode>();
      Children = new List<DifferenceControlNode>();
    }

    public DifferenceControlNode(string name, string id, string type, List<string> path, List<int> techPath)
    {
      Name = name;
      Id = id;
      Type = type;
      Path = path;
      DifferentProperties = new List<DifferencePropertyNode>();
      Children = new List<DifferenceControlNode>();
      TechPath = techPath;
    }

    public DifferenceControlNode(string name, string exeptedName, string id, string type, List<string> path,
      List<int> techPath) : this(name, id, type, path, techPath)
    {
      ExpectedName = exeptedName;
      Children = new List<DifferenceControlNode>();
    }

    public DifferenceControlNode(XmlNode xmlNode)
    {
      if (xmlNode.Attributes == null)
        throw new Exception("Null at xmlNode.Attributes in DifferenceControlNode constuctor");

      Name = xmlNode.Attributes.GetNamedItem("Name").Value;
      Id = xmlNode.Attributes.GetNamedItem("Id").Value;
      Type = xmlNode.Attributes.GetNamedItem("Type").Value;
      Path = PathToList(xmlNode.Attributes.GetNamedItem("Path").Value);
      ExpectedName = xmlNode.Attributes.GetNamedItem("ExpectedName").Value;
      TechPath = PathToIntList(xmlNode.Attributes.GetNamedItem("TechPath").Value);
      DifferentProperties = new List<DifferencePropertyNode>();
      Children = new List<DifferenceControlNode>();
    }

    public XmlNode RootToXmlNode(XmlDocument xmlDocument)
    {
      return xmlDocument.CreateElement(Name);
    }

    public XmlNode ToXmlNode(XmlDocument xmlDocument)
    {
      XmlNode differenceNode = xmlDocument.CreateElement("Control");

      XmlAttribute nameAttribute = xmlDocument.CreateAttribute("Name");
      nameAttribute.Value = Name;
      XmlAttribute expectedNameAttribute = xmlDocument.CreateAttribute("ExpectedName");
      expectedNameAttribute.Value = ExpectedName;
      XmlAttribute idAttribute = xmlDocument.CreateAttribute("Id");
      idAttribute.Value = Id;
      XmlAttribute typeAttribute = xmlDocument.CreateAttribute("Type");
      typeAttribute.Value = Type;
      XmlAttribute pathAttribute = xmlDocument.CreateAttribute("Path");
      pathAttribute.Value = PathToString();
      XmlAttribute techPathAttribute = xmlDocument.CreateAttribute("TechPath");
      techPathAttribute.Value = TechPathToString();

      if (differenceNode.Attributes == null)
        throw new Exception("Null at controlNode.Attributes in ControlNode.ToXmlNode");
      differenceNode.Attributes.Append(nameAttribute);
      differenceNode.Attributes.Append(expectedNameAttribute);
      differenceNode.Attributes.Append(idAttribute);
      differenceNode.Attributes.Append(typeAttribute);
      differenceNode.Attributes.Append(pathAttribute);
      differenceNode.Attributes.Append(techPathAttribute);

      return differenceNode;
    }

    public void AddChild(DifferenceControlNode child)
    {
      Children.Add(child);
    }

    public void AddDifferentProperty(DifferencePropertyNode node)
    {
      DifferentProperties.Add(node);
    }

    public string PathToString()
    {
      return Path.Aggregate("", (current, node) => current + ("/" + node));
    }

    public string TechPathToString()
    {
      return TechPath.Aggregate("", (current, node) => current + ("/" + node.ToString()));
    }

    public List<string> PathToList(string path)
    {
      List<string> convertedPath = path.Split('/').ToList();
      convertedPath.RemoveAt(0);

      return convertedPath;
    }

    public List<int> PathToIntList(string path)
    {
      List<string> convertedPath = path.Split('/').ToList();
      convertedPath.RemoveAt(0);
      List<int> intsPath = new List<int>();
      foreach (var node in convertedPath)
      {
        intsPath.Add(Convert.ToInt32(node));
      }
      return intsPath;
    }
  }
}
