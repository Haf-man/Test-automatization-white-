using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using AutomationTree;
using ConfigurationReader;
using DifferenceTree;
using SimpleLogger;
using WhiteAutomation;
using Modes = WhiteAutomation.Modes;

namespace AutomationTreeComparator
{
    enum CompareModes
    {
        ExcludeThis, ExcludeChildren, ExcludeThisWithChildren, OnlyThis, ThisWithChildren, OnlyChildren
    }

    enum ExcludeModes
    {
        NoExclude, ExcludeThis, ExcludeChildren, ExcludeThisWithChildren
    }
    
    public static class Comparator
    {
#region public
        /// <summary>
        /// Compare two trees of app state
        /// </summary>
        /// <param name="tree1">Current tree of state</param>
        /// <param name="tree2">Expected tree of state</param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static DifferenceControlNode CompareTrees(ControlNode tree1, ControlNode tree2, Configuration configuration)
        {
            CorrectConfiguration(configuration);
            LoadConfig(configuration);
            Dictionary<string,Pair<CompareModes,string>> mode = new Dictionary<string, Pair<CompareModes, string>>();
            PreLoadConfiguration();
            List<int> techPath = new List<int> {0};
            DifferenceControlNode differenceControlNodes = new DifferenceControlNode("ChangedControls");
            CompareWithMode(tree1, tree2, mode, differenceControlNodes, techPath);
            ResultsToLog(differenceControlNodes);
            return differenceControlNodes;
        }

        /// <summary>
        /// Proove trees' nodes uniqueness 
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="isCorrect"> True if tree correct, must be true when method called</param>
        public static void ProoveToCorrect(ControlNode tree,ref bool isCorrect)
        {
            List<ControlNode> children = tree.Children;
            foreach (var child in children)
            {  
                List<ControlNode> anotherChildren =children.ToList();
                anotherChildren.Remove(child);
                 
                if (anotherChildren.Find(x => x.Id == child.Id)!=null)
                    isCorrect = false;
                 ProoveToCorrect(child,ref isCorrect);
            }
        }

       /// <summary>
       /// Save result of compare to file
       /// </summary>
       /// <param name="differentNodes">Result of compare</param>
       /// <param name="path">Path to file</param>
        public static void ResultsToFile(DifferenceControlNode differentNodes, string path)
        {
            XmlTextWriter xmlWriter = new XmlTextWriter(path, Encoding.UTF8);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("head");
            xmlWriter.WriteEndElement();
            xmlWriter.Close();
             
            XmlDocument xmlDocument = new XmlDocument();
            
            
            xmlDocument.Load(path);
            XmlNode parentNode = differentNodes.RootToXmlNode(xmlDocument);
            if ( xmlDocument.DocumentElement == null)
                throw  new Exception("Unexepted error in ResultToFile Method");
            xmlDocument.DocumentElement.AppendChild(parentNode);
            foreach (var node in differentNodes.Children)
            {
                XmlNode xmlNode = CreateInfoNode(node, xmlDocument);
                parentNode.AppendChild(xmlNode);
            }

            xmlDocument.Save(path);
        }

#endregion
#region private

         
        private static List<string> _includedProperties;
        private static List<string> _excludedProperties;
        private static List<ControlConfigurationsControlConfiguration> _includedControls;
        private static List<ControlConfigurationsControlConfiguration> _excludedControls;

        private static Dictionary<string, Pair<CompareModes, string>> _waitingModes;
      
      
#region InfoMethods

        private static void ResultsToLog(DifferenceControlNode diffenrentNodes)
        {
            string message = "";
            int numOfChangedProperties = 0;
            foreach (var differentNode in diffenrentNodes.Children)
            {
                message += CreateShortInfoLine(differentNode, ref numOfChangedProperties);
            }
            message += "Total: " + diffenrentNodes.Children.Count + " changed controls and " + numOfChangedProperties +
                       " changed properties" + "\n";
            Logger.WriteToLogNormally(message);
        }
       
        private static string DeleteExcessInformation(string info)
        {
               return info.Split('.').Last();
        }
        private static string CreateShortInfoLine(DifferenceControlNode differenceControlNode, ref int numOfChangedProperties)
        {
            string infoLine = "Control( \"" + differenceControlNode.Id + "\", \"" + differenceControlNode.PathToString()+"\"";
           
            infoLine += ")\n";

            foreach (var differentProperty in differenceControlNode.Children.First().DifferentProperties)
            {
                infoLine += "PropertyChange(\"" + DeleteExcessInformation(differentProperty.Name) + "\"; actually: \"" +
                                differentProperty.Value + "\", expected: \"" + differentProperty.ExpectedValue + "\")" + "\n";
                numOfChangedProperties++;
            }
            
            if (differenceControlNode.Name != differenceControlNode.ExpectedName)
                infoLine += " Name(actually: \"" + differenceControlNode.Name + "\", expected: \"" +
                            differenceControlNode.ExpectedName +
                            "\")";
            return infoLine;
        }
        private static XmlNode CreateInfoNode(DifferenceControlNode node, XmlDocument xmlDocument)
        {
            XmlNode differenceControlNode = node.ToXmlNode(xmlDocument);
            
            XmlNode changedPropetiesNode = node.Children.First().RootToXmlNode(xmlDocument);
            differenceControlNode.AppendChild(changedPropetiesNode);

            foreach (var differentProperty in node.Children.First().DifferentProperties)
            {
                XmlNode differentPropertyXmlNode = differentProperty.ToXmlNode(xmlDocument);
                changedPropetiesNode.AppendChild(differentPropertyXmlNode);
            }
           
            return differenceControlNode;

        }

        
#endregion

#region CompareMethods

      private static string _nameProperty = "AutomationElementIdentifiers.NameProperty";
      private static Dictionary<string, CompareModes> _globalModes;
      private static bool _haveIncludedModes;
        private static void LoadConfig(Configuration configuration)
        {
            _includedProperties = configuration.IncludedProperties.ToList();
            _excludedProperties = configuration.ExcludedProperties.ToList();
            _includedControls = configuration.IncludedControls.ToList();
            _excludedControls = configuration.ExcludedControls.ToList();
        }
        private static void CorrectConfiguration(Configuration configuration)
        {
            if (configuration.IncludedControls == null)
                configuration.IncludedControls = new ControlConfigurationsControlConfiguration[0];
            else
            {
                foreach (var control in configuration.IncludedControls)
                {
                    if (control.InterestingProperties ==null)
                        control.InterestingProperties = new ControlPropertiesControlPropertie[0];
                }
               
            }
            if(configuration.ExcludedControls ==null)
                configuration.ExcludedControls = new ControlConfigurationsControlConfiguration[0];
            if(configuration.IncludedProperties==null)
                configuration.IncludedProperties = new string[0];
            if ( configuration.ExcludedProperties ==null)
                configuration.ExcludedProperties = new string[0];
            

        }
      
        /// <summary>
        /// create init mode
        /// </summary>
        /// <param></param>
        private static void PreLoadConfiguration()
        {
           
          _waitingModes = new Dictionary<string, Pair<CompareModes, string>>();
            _globalModes = new Dictionary<string, CompareModes>();
          _haveIncludedModes = false;
          if (_excludedProperties.Count != 0)
          {

            foreach (var excludedProperty in _excludedProperties)
            {
              
                _globalModes.Add(excludedProperty, CompareModes.ExcludeThisWithChildren);
            }
          }
          if (_includedProperties.Count == 0) 
            return;
          foreach (var includedProperty in _includedProperties)
          {
            if (_globalModes.ContainsKey(includedProperty)) 
              continue;
            _globalModes.Add(includedProperty,  CompareModes.ThisWithChildren);
            _haveIncludedModes = true;
          }
        }

        private static void UpdateExclude(ExcludeModes excludeMode, ref bool isCheck, ref bool isCheckChildren)
        {
            switch (excludeMode)
            {
                case ExcludeModes.ExcludeThis:
                    isCheck = false;
                    break;
                case ExcludeModes.ExcludeChildren:
                    isCheckChildren = false;
                    break;
                case ExcludeModes.ExcludeThisWithChildren:
                    isCheck = false;
                    isCheckChildren = false;
                    break;
            }
        }
        private static void CompareWithMode(ControlNode node1, ControlNode node2,
            Dictionary<string, Pair<CompareModes, string>> mode, DifferenceControlNode differentNodes, List<int> techPath )
        {
            
            Dictionary<string, Pair<CompareModes, string>> localMode = new Dictionary<string, Pair<CompareModes, string>>(mode);
            ExcludeModes excludeMode = CheckConfiguration(node1, mode);
            bool isCheckThis = true;
            bool isCheckChildren = true;
            UpdateExclude(excludeMode, ref isCheckThis, ref isCheckChildren);

            if (isCheckThis)
            {
                if (!(node1.Id == node2.Id && node1.PathToString() == node2.PathToString() && node1.Type == node2.Type))
                    throw new Exception("Unexpected changes in tree in Comporator.CompareWithMode");
               
                DifferenceControlNode differentNode = new DifferenceControlNode(node1.Name, node2.Name, node2.Id,
                    node2.Type, node2.Path, new List<int>(techPath));
                differentNode.AddChild(new DifferenceControlNode("ChangedProperties"));
                if( mode.Count != 0)
                  CompareProperties(node1,node2, mode, differentNode.Children.First());
                else
                {
                  CompareGlobalProperties(node1, node2, differentNode.Children.First());
                }
                if (differentNode.Children.First().DifferentProperties.Count!= 0 )
                {
                    differentNodes.AddChild(differentNode);
                }
            }

            ChangeModes(mode);

            if (isCheckChildren)
            {
                var childrenNode1 = node1.Children;
                var childrenNode2 = node2.Children;

                int length1 = childrenNode1.Count;
                int length2 = childrenNode2.Count;
                if (length1 + length2 == 0)
                    return;
                if (length1 != length2)
                    throw new Exception("Unexpected changes in Tree");

                for (int i = 0; i < length1; i++)
                {
                    var childNode1 = childrenNode1[i];
                    var childNode2 = childrenNode2[i];
                    if (childNode1.Id != childNode2.Id)
                        throw new Exception("Unexpected changes in Tree in Comparator.CompareWithMode");
                    techPath.Add(i);
                    CompareWithMode(childrenNode1[i], childrenNode2[i], mode, differentNodes, techPath);
                    techPath.RemoveAt(techPath.Count - 1);
                }
            }

            mode = new Dictionary<string, Pair<CompareModes, string>>(localMode);
        }

        private static void ChangeModes(Dictionary<string, Pair<CompareModes, string>> mode)
        {
            List<string> removeList = new List<string>();

            HashSet<CompareModes> changingPropertiesType = new HashSet<CompareModes>
            {
                CompareModes.ExcludeThis,
                CompareModes.OnlyThis
            };

            foreach (var mod in mode )
            {
                if (mod.Value.Property1 == CompareModes.OnlyChildren)
                {
                    mod.Value.Property1 = CompareModes.ThisWithChildren;
                    continue;
                }
                if (mod.Value.Property1 == CompareModes.ExcludeChildren)
                {
                    mod.Value.Property1 = CompareModes.ExcludeThisWithChildren;
                    continue;
                }
               
                if (changingPropertiesType.Contains(mod.Value.Property1))
                {
                    removeList.Add(mod.Key);
                }

            }
            foreach (var removeKey in removeList )
            {
                mode.Remove(removeKey);
            }
            foreach (var waitingMode in _waitingModes)
            {
                if (mode.ContainsKey(waitingMode.Key))
                    mode[waitingMode.Key] = waitingMode.Value;
                else
                {
                    mode.Add(waitingMode.Key, waitingMode.Value);
                }
            }
            _waitingModes.Clear();
        }
       

       
        /// <summary>
        ///  configurate mode parametres for each step in compare
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        private static ExcludeModes CheckConfiguration(ControlNode node, Dictionary<string, Pair<CompareModes,string>> mode)
        {
            var excludedControl =
                _excludedControls.Find(
                    x => x.AutomationId == node.Id && x.Path == node.PathToString() && x.ControlType == node.Type);

            if (excludedControl != null)
            {
                if (!excludedControl.CompareScopeSpecified)
                { 
                    foreach (var excludedProperty in excludedControl.InterestingProperties)
                    {
                        CompareModes compareMode = CompareModes.ExcludeThis;
                        switch (excludedProperty.CompareScope)
                        {
                            case CompareScopes.OnlyControl:
                                compareMode = CompareModes.ExcludeThis;
                                break;
                            case CompareScopes.OnlyChildren:
                                compareMode = CompareModes.ExcludeChildren;
                                break;
                            case CompareScopes.ChildrenWithControl:
                                compareMode = CompareModes.ExcludeThisWithChildren;
                                break;
                        }
                        if (excludedProperty.CompareScope != CompareScopes.OnlyChildren)
                        {
                            if (mode.ContainsKey(excludedProperty.Name))
                                mode[excludedProperty.Name] =
                                    new Pair<CompareModes, string>(compareMode, "");
                            else
                            {
                                mode.Add(excludedProperty.Name,
                                    new Pair<CompareModes, string>(compareMode, ""));
                            }
                        }
                        else
                        {
                            if (_waitingModes.ContainsKey(excludedProperty.Name))
                                _waitingModes[excludedProperty.Name] =
                                    new Pair<CompareModes, string>(CompareModes.ExcludeThisWithChildren, "");
                            else
                            {
                                _waitingModes.Add(excludedProperty.Name,
                                    new Pair<CompareModes, string>(CompareModes.ExcludeThisWithChildren, ""));
                            }
                        }
                    }
                    return ExcludeModes.NoExclude;
                }
                if (excludedControl.CompareScope == CompareScopes.ChildrenWithControl)
                    return ExcludeModes.ExcludeThisWithChildren;
                if (excludedControl.CompareScope == CompareScopes.OnlyChildren)
                {
                    foreach (var excludedProperty in excludedControl.InterestingProperties)
                    {
                        if (excludedProperty.CompareScope == CompareScopes.OnlyControl)
                        {
                            if (mode.ContainsKey(excludedProperty.Name))
                                mode[excludedProperty.Name] =
                                    new Pair<CompareModes, string>(CompareModes.ExcludeThis, "");
                            else
                            {
                                mode.Add(excludedProperty.Name,
                                    new Pair<CompareModes, string>(CompareModes.ExcludeThis, ""));
                            }
                        }
                    }
                    return ExcludeModes.ExcludeChildren;
                }
                if (excludedControl.CompareScope == CompareScopes.OnlyControl)
                {
                    foreach (var excludedProperty in excludedControl.InterestingProperties)
                    {
                        if (excludedProperty.CompareScope == CompareScopes.OnlyChildren)
                        {
                            if (mode.ContainsKey(excludedProperty.Name))
                                mode[excludedProperty.Name] =
                                    new Pair<CompareModes, string>(CompareModes.ExcludeChildren, "");
                            else
                            {
                                mode.Add(excludedProperty.Name,
                                    new Pair<CompareModes, string>(CompareModes.ExcludeChildren, ""));
                            }
                        }
                    }
                    return ExcludeModes.ExcludeThis;
                }
            }

            var includedControl = _includedControls.Find(
               x => x.AutomationId == node.Id && x.Path == node.PathToString() && x.ControlType == node.Type);
            if (includedControl != null)
            {
                foreach (var property in includedControl.InterestingProperties)
                {
                    CompareModes currentCompareMode = CompareModes.OnlyThis;
                    switch (property.CompareScope)
                    {
                        case CompareScopes.OnlyControl:
                            currentCompareMode = CompareModes.OnlyThis;
                            break;
                        case CompareScopes.ChildrenWithControl:
                            currentCompareMode = CompareModes.ThisWithChildren;
                            break;
                        case CompareScopes.OnlyChildren:
                            currentCompareMode = CompareModes.OnlyChildren;
                            break;
                    }
                    Modes currentMode = Modes.Special;
                    string regexp = property.Value;
                    if (property.IsValueRegExp)
                    {
                        currentMode = Modes.SpecialWithRegularExpression;
                    }
                    if (property.CompareScope != CompareScopes.OnlyChildren)
                    {
                        if (mode.ContainsKey(property.Name))
                            mode[property.Name] = new Pair<CompareModes, string>(currentCompareMode, regexp,
                                currentMode);
                        else
                            mode.Add(property.Name,
                                new Pair<CompareModes, string>(currentCompareMode, regexp, currentMode));
                    }
                    else
                    {
                        if (_waitingModes.ContainsKey(property.Name))
                            _waitingModes[property.Name] =
                                new Pair<CompareModes, string>(CompareModes.ThisWithChildren, regexp,
                                    currentMode);
                        else
                            _waitingModes.Add(property.Name,
                                new Pair<CompareModes, string>(CompareModes.ThisWithChildren, regexp, currentMode));
                    }
                }
            }
            return ExcludeModes.NoExclude;
        }

      private static void CompareGlobalProperties(ControlNode node1, ControlNode node2,
        DifferenceControlNode differenceControlNode)
      {
        if(_haveIncludedModes)
          foreach (var mode in _globalModes)
          {
            if (mode.Value == CompareModes.ExcludeThisWithChildren)
              continue;
            if (mode.Key == _nameProperty)
            {
              if (node1.Name != node2.Name)
                differenceControlNode.AddDifferentProperty(new DifferencePropertyNode("Name", node1.Name, node2.Name,
                  DifferenceTree.Modes.Usual));
              continue;
            }
            var property1 = node1.Properties.Find(x=> x.Name == mode.Key);
            var property2 = node2.Properties.Find(x => x.Name == mode.Key);
            if ( property1 == null && property2 == null)
              continue;
            if (property1 == null )
            {
              differenceControlNode.AddDifferentProperty(new DifferencePropertyNode(property2.Name,
              "No such property", property2.Value, DifferenceTree.Modes.Usual));
              continue;
            }
            if ( property2 == null)
              throw new Exception("Incorrect configuration: no such include property");
            if( property1.Value != property2.Value)
              differenceControlNode.AddDifferentProperty(new DifferencePropertyNode(property2.Name,
                property1.Value, property2.Value, DifferenceTree.Modes.Usual));
          }
        else
        {
          var node1Properties = node1.Properties;
          var node2Properties = node2.Properties;
          foreach (var property2 in node2Properties)
          {
            if (_globalModes.ContainsKey(property2.Name))
              continue;
            
            var property1 = node1Properties.Find(x => x.Name == property2.Name);
            if (property1 == null)
            {
              differenceControlNode.AddDifferentProperty(new DifferencePropertyNode(property2.Name,
              "No such property", property2.Value, DifferenceTree.Modes.Usual));
              continue;
            }
            if( property1.Value != property2.Value)
              differenceControlNode.AddDifferentProperty(new DifferencePropertyNode(property2.Name,
                property1.Value, property2.Value, DifferenceTree.Modes.Usual));
          }
          
            if (node1.Name != node2.Name && !_globalModes.ContainsKey(_nameProperty))
              differenceControlNode.AddDifferentProperty(new DifferencePropertyNode("Name", node1.Name, node2.Name,
                DifferenceTree.Modes.Usual));
            
           
        }
      }
      private static void CompareProperties(ControlNode node1, ControlNode node2,
        Dictionary<string, Pair<CompareModes, string>> mode, DifferenceControlNode differenceControlNode)
      {
        List<PropertiesNode> propertiesOfNode1 = node1.Properties;
        List<PropertiesNode> propertiesOfNode2 = node2.Properties;


        HashSet<CompareModes> excludedPropertiesType = new HashSet<CompareModes>
        {
          CompareModes.ExcludeThisWithChildren,
          CompareModes.ExcludeThis,
          CompareModes.OnlyChildren
        };

        foreach (var property2 in propertiesOfNode2)
        {
          if (mode.ContainsKey(property2.Name))
          {
            if (excludedPropertiesType.Contains(mode[property2.Name].Property1) ||
                property2.Name == _nameProperty
              )
              continue;
          }
          else
          {
            if (_globalModes.ContainsKey(property2.Name))
              if(_globalModes[property2.Name] == CompareModes.ExcludeThisWithChildren)
                continue;
          }
          var property1 = propertiesOfNode1.Find(x => x.Name == property2.Name);
          if (property1 == null)
          {
            differenceControlNode.AddDifferentProperty(new DifferencePropertyNode(property2.Name,
              "No such property", property2.Value, DifferenceTree.Modes.Usual));
            continue;
          }
          if (mode.ContainsKey(property2.Name))
          {
            if (mode[property2.Name].Mode != Modes.SpecialWithRegularExpression)
            {
              if (!ComparePropertyWithLine(property1.Value, mode[property2.Name].Property2))
                differenceControlNode.AddDifferentProperty(new DifferencePropertyNode(property2.Name, property1.Value,
                  property2.Value, DifferenceTree.Modes.Special, mode[property2.Name].Property2));

            }
            else
            {
              if (!ComparePropertyWithRegExp(property1.Value, mode[property2.Name].Property2))
                differenceControlNode.AddDifferentProperty(new DifferencePropertyNode(property2.Name, property1.Value,
                  property2.Value, DifferenceTree.Modes.SpecialWithRegularExpression, mode[property2.Name].Property2));

            }
          }
          else
          {
            if (!CompareTwoProperties(property1.Value, property2.Value))
              differenceControlNode.AddDifferentProperty(new DifferencePropertyNode(property2.Name,
                property1.Value, property2.Value, DifferenceTree.Modes.Usual));

          }
        }
        bool nameProove = true;
        if (mode.ContainsKey(_nameProperty))
          if (excludedPropertiesType.Contains(mode[_nameProperty].Property1))
            nameProove = false;
        if (nameProove)
        {
          if (mode.ContainsKey(_nameProperty))
          {
            if (mode[_nameProperty].Mode == Modes.SpecialWithRegularExpression)
            {
              if (!ComparePropertyWithRegExp(node1.Name, mode[_nameProperty].Property2))
                differenceControlNode.AddDifferentProperty(new DifferencePropertyNode("Name", node1.Name,
                  node2.Name, DifferenceTree.Modes.SpecialWithRegularExpression, mode[_nameProperty].Property2));
            }
            if (!ComparePropertyWithLine(node1.Name, mode[_nameProperty].Property2))
              differenceControlNode.AddDifferentProperty(new DifferencePropertyNode("Name", node1.Name, node2.Name,
                DifferenceTree.Modes.Special, mode[_nameProperty].Property2));
          }
          else
          {
            if (_globalModes.ContainsKey(_nameProperty))
              if (_globalModes[_nameProperty] == CompareModes.ExcludeThisWithChildren)
                return;
            if (!CompareTwoProperties(node1.Name, node2.Name))
              differenceControlNode.AddDifferentProperty(new DifferencePropertyNode("Name", node1.Name, node2.Name,
                DifferenceTree.Modes.Usual));
          }
        }
        
      }

      private static bool CompareTwoProperties(string property1Value, string property2Value)
        {
            return property1Value == property2Value;
        }

      private static bool ComparePropertyWithLine(string propertyValue, string line)
      {
        return propertyValue == line;
      }
        private static bool ComparePropertyWithRegExp(string propertyValue, string regExp)
        {
            string regEx = regExp;
            if (string.IsNullOrEmpty(regExp))
                return true;
           
            string correctRegExpStart = "\\A(";
            string correctRegExpEnd = ")\\z";
            if (!regExp.StartsWith(correctRegExpStart))
                regEx = correctRegExpStart + regEx;
            if (!regExp.EndsWith(correctRegExpEnd))
                regEx = regEx + correctRegExpEnd;
            Regex regex = new Regex(regEx);

            if (!regex.IsMatch(propertyValue))
                return false;
            return true;
        }
         
#endregion
#endregion
    }
}
