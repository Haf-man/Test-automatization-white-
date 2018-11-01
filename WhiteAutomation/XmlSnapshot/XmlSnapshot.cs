using System;
using System.Text;
using System.Xml;
using AutomationTree;
using DifferenceTree;


namespace XmlSnapshot
{
    public static class XmlSnapshot
    {
#region public
        
        public static void WriteToXmlFile(ControlNode rootNode, string path)
        {             
            XmlTextWriter xmlWriter = new XmlTextWriter(path, Encoding.UTF8); 
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("head");  
            xmlWriter.WriteEndElement();
            xmlWriter.Close();  
            XmlDocument xmlDocument = CreateXmlTree(rootNode, path);  
            
            xmlDocument.Save(path);
                       
        }

        public static ControlNode ReadFromXmlFile(string path)
        {

            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(path);
            }
            catch (Exception ex)
            {                
                throw ex;
            }
            XmlNode node = xmlDocument.DocumentElement;
            XmlNode rootXmlNode = node;
            if (node ==null)
                throw new Exception("Incorrect  Xml file structure");
            if (node.Name != "Control")
                rootXmlNode = node.ChildNodes.Item(0);
            

           return CreateTree(rootXmlNode);

        }

        public static DifferenceControlNode ReadFromDifferenceXmlFile(string path)
        {
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            XmlNode node = xmlDocument.DocumentElement;
            XmlNode rootXmlNode = node;
            if (node == null)
                throw new Exception("Incorrect  Xml file structure");
            if (node.Name != "ChangedControls")
                rootXmlNode = node.ChildNodes.Item(0);


            return CreateDifferenceTree(rootXmlNode);
        }

#endregion
#region private

       static XmlDocument CreateXmlTree(ControlNode rootNode, string path)
        {
            XmlDocument xmlDocument = new XmlDocument();        
            xmlDocument.Load(path);
            XmlNode rootXmlNode = CreateRootXmlNode(rootNode, xmlDocument);

            AddChilds(rootNode, rootXmlNode, xmlDocument);

            return xmlDocument;
        }

        private static DifferenceControlNode CreateDifferenceTree(XmlNode rootXmlNode)
        {
            DifferenceControlNode rootNode = new DifferenceControlNode("ChangedControls");
            XmlNodeList xmlChildNodeList = rootXmlNode.ChildNodes;
            foreach (XmlNode xmlChildNode in xmlChildNodeList)
            {
                DifferenceControlNode node = new DifferenceControlNode(xmlChildNode);
                if(xmlChildNode.ChildNodes.Count != 0)
                AddDifferencePropertiesFromXml(node, xmlChildNode);
                rootNode.AddChild(node);
            }
            return rootNode;
        }

        private static void AddDifferencePropertiesFromXml(DifferenceControlNode node, XmlNode xmlNode)
        {
            XmlNodeList xmlPropertiesList = xmlNode.ChildNodes[0].ChildNodes;
            foreach (XmlNode xmlProperNode in xmlPropertiesList)
            {
                node.AddDifferentProperty(new DifferencePropertyNode(xmlProperNode));
            }
        }
        private static ControlNode CreateTree(XmlNode rootXmlNode)
        {
            ControlNode rootNode = new ControlNode(rootXmlNode);
            XmlNodeList xmlChildNodeList = rootXmlNode.ChildNodes;
            foreach (XmlNode xmlChildNode in xmlChildNodeList )
            {
                switch (xmlChildNode.Name)
                {
                    case "Properties":
                        AddPropertiesFromXml(xmlChildNode, rootNode);
                        break;
                    case "Patterns":
                        AddPatternsFromXml(xmlChildNode, rootNode);
                        break;
                    case "Childs":
                        AddChildsFromXml(xmlChildNode, rootNode);
                        break;
                }
            }
            
            return rootNode;
        }
        
        private static void AddPropertiesFromXml(XmlNode xmlNode, ControlNode node)
        {
            XmlNodeList xmlNodeList = xmlNode.ChildNodes;
            foreach (XmlNode propertyXmlNode in xmlNodeList)
            {
                PropertiesNode propertyNode = new PropertiesNode(propertyXmlNode);
                node.AddProperty(propertyNode);
            }
        }
        //private static void AddPropertiesFromXml(XmlNode xmlNode, PatternNode node)
        //{
        //    XmlNodeList xmlNodeList = xmlNode.ChildNodes;
        //    xmlNodeList = xmlNodeList[0].ChildNodes;
        //    foreach (XmlNode propertyXmlNode in xmlNodeList)
        //    {
        //        PropertiesNode propertyNode = new PropertiesNode(propertyXmlNode);
        //        node.AddProperty(propertyNode);
        //    }
        //}
        private static void AddPatternsFromXml(XmlNode xmlNode, ControlNode node)
         {
            XmlNodeList xmlNodeList = xmlNode.ChildNodes;
            foreach (XmlNode patternXmlNode in xmlNodeList)
            {
                PatternNode patternNode = new PatternNode(patternXmlNode);
                node.AddPatern(patternNode);
                //AddPropertiesFromXml(patternXmlNode,patternNode);
            }
        }

        private static void AddChildsFromXml(XmlNode xmlNode, ControlNode node)
        {
             
            XmlNodeList xmlChildNodeList = xmlNode.ChildNodes;
            foreach (XmlNode xmlChildNode in xmlChildNodeList)
            {
                ControlNode controlNode = new ControlNode(xmlChildNode);
                XmlNodeList xmlChildChildNodeList = xmlChildNode.ChildNodes;
                
               
                foreach (XmlNode xmlChildChildNode in xmlChildChildNodeList)
                {
                    switch (xmlChildChildNode.Name)
                    { 
                        case "Properties":
                            AddPropertiesFromXml(xmlChildChildNode, controlNode);
                            break;
                        case "Patterns":
                            AddPatternsFromXml(xmlChildChildNode, controlNode);
                            break;
                        case "Childs":
                            AddChildsFromXml(xmlChildChildNode, controlNode);
                            break;
                    }
                }
               node.AddChild(controlNode);
            }
        }
       
        static void AddChilds(ControlNode node, XmlNode xmlNode, XmlDocument xmlDocument)
        {
            if (node.Children.Count!=0)
            {
                XmlNode childsXmlNode = xmlDocument.CreateElement("Childs");
                xmlNode.AppendChild(childsXmlNode);

                foreach(var childNode in node.Children)
                {
                    XmlNode childXmlNode = childNode.ToXmlNode(xmlDocument);
                     
                    AddProperties(childNode, childXmlNode, xmlDocument);
                    AddPatterns(childNode, childXmlNode, xmlDocument);

                    childsXmlNode.AppendChild(childXmlNode);

                    AddChilds(childNode, childXmlNode, xmlDocument);
                }
            }
        }
        static XmlNode CreateRootXmlNode(ControlNode node, XmlDocument xmlDocument)
        {
            XmlNode xmlNode = node.ToXmlNode(xmlDocument);
            
            AddProperties(node, xmlNode, xmlDocument);
            AddPatterns(node, xmlNode, xmlDocument);
            xmlDocument.DocumentElement.AppendChild(xmlNode);
            return xmlNode;
        }
       
        static void AddProperties(ControlNode node, XmlNode xmlNode, XmlDocument xmlDocument)
        {
            if (node.Properties.Count != 0)
            {
                XmlNode propertiesNode = xmlDocument.CreateElement("Properties");
                xmlNode.AppendChild(propertiesNode);
                foreach (var property in node.Properties)
                {
                    XmlNode propertyNode = property.ToXmlNode(xmlDocument);
                    propertiesNode.AppendChild(propertyNode);
                }
            }
        }

        //static void AddProperties(PatternNode node, XmlNode xmlNode, XmlDocument xmlDocument)
        //{
        //    XmlNode propertiesNode = xmlDocument.CreateElement("Properties");
        //    xmlNode.AppendChild(propertiesNode);
        //    foreach (var property in node.Properties)
        //    {
        //        XmlNode propertyNode = property.ToXmlNode(xmlDocument);
        //        propertiesNode.AppendChild(propertyNode);
        //    }
        //}
       static  void AddPatterns(ControlNode node, XmlNode xmlNode, XmlDocument xmlDocument) 
        {
           if (node.Paterns.Count != 0)
           {
               XmlNode patternsNode = xmlDocument.CreateElement("Patterns");
               xmlNode.AppendChild(patternsNode);
               foreach (var pattern in node.Paterns)
               {
                   XmlNode patternNode = pattern.ToXmlNode(xmlDocument);
                   patternsNode.AppendChild(patternNode);

                   // AddProperties(pattern,patternNode,xmlDocument);
               }
           }
        }
#endregion
    }
}
