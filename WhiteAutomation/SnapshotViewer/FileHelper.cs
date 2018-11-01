using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AutomationTree;
using ConfigurationReader;
using DifferenceTree;
using SnapshotViewer.Configuration;
using SnapshotViewer.Snapshot;

namespace SnapshotViewer
{
    public static class FileHelper
    {
        private static readonly Brush DiffNodeColor = Brushes.Coral;
        private static readonly Brush DiffPathColor = Brushes.Yellow;
      /// <summary>
      /// try to load snapshot and return null when can't
      /// </summary>
      /// <param name="path"></param>
      /// <returns></returns>
        public static List<NodeTreeViewItem> LoadSnapshot(string path)
        {
            ControlNode rootNode;
            try
            {
                rootNode = XmlSnapshot.XmlSnapshot.ReadFromXmlFile(path);
            }
            catch (Exception)
            {
                return null;
            }

            List<NodeTreeViewItem> rootTreeViewItem = new List<NodeTreeViewItem>
            {
                new NodeTreeViewItem
                {
                    CurrentControl = new DisplayedControlNode(rootNode),
          Header = "\"" + rootNode.Name + "\"" + " (" + rootNode.Type.Split('.').Last() + ")",
          TechPath = @"/0"
                }
            };

            AddChildNodes(rootTreeViewItem.Last());
            
            return rootTreeViewItem;
        }

        private static void AddChildNodes(NodeTreeViewItem rootTreeViewItem)
        {
      int number = 0;
            foreach (var controlNode in rootTreeViewItem.CurrentControl.Children)
            {
                var treeItem = new NodeTreeViewItem
                {
                    Header = "\"" + controlNode.Name + "\"" + " (" + controlNode.Type.Split('.').Last() + ")",
          CurrentControl = new DisplayedControlNode(controlNode),
          TechPath = rootTreeViewItem.TechPath + @"/" + number
                };
        ++number;
                rootTreeViewItem.Items.Add(treeItem);
                AddChildNodes(treeItem);
            }
        }
      /// <summary>
      /// try to load configuration from file and return null when can't
      /// </summary>
      /// <param name="path"></param>
      /// <returns></returns>
        public static List<TreeViewItem> LoadConfiguration(string path)
        {
            ConfigurationReader.Configuration configuration;
            try
            {
                configuration = ConfigurationReaderWriter.ReadConfiguration(path);
            }
            catch (Exception)
            {
                return null;
            }
            MainWindow.CurrentConfiguration = new ConfigurationKeeper(configuration);

            return MainWindow.CurrentConfiguration.ConfigurationTree;
        }
      /// <summary>
      /// Create empty configuration
      /// </summary>
      /// <returns></returns>
      public static List<TreeViewItem> EmptyConfiguration()
      {
        MainWindow.CurrentConfiguration = new ConfigurationKeeper();

        return MainWindow.CurrentConfiguration.ConfigurationTree;

      }
      /// <summary>
      /// try to load diff and return null when can't
      /// </summary>
      /// <param name="path"></param>
      /// <returns></returns>
    public static DifferenceControlNode LoadDifference(string path)
        {
            DifferenceControlNode rootNode;
            try
            {
                rootNode = XmlSnapshot.XmlSnapshot.ReadFromDifferenceXmlFile(path);
            }
            catch (Exception)
            {
                return null;
            }
                 
            return rootNode;   
        }
      /// <summary>
      /// Load snapshot, config and diff from files with one name
      /// </summary>
      /// <param name="path"></param>
      /// <param name="snapshot"></param>
      /// <param name="config"></param>
      /// <param name="diff"></param>
        public static void LoadAll(string path, out List<NodeTreeViewItem> snapshot,
            out List<TreeViewItem> config, out DifferenceControlNode diff)
        {
            snapshot = LoadSnapshot(path);
            path = path.TrimEnd(ControlPoint.ControlPoint.SnapshotExtension.ToCharArray());
            config = LoadConfiguration(path + ControlPoint.ControlPoint.ConfigExtenstion);
            diff = LoadDifference(path + ControlPoint.ControlPoint.DiffExtension);
        }

        #region diffLoader

        public static void ClearTree(List<NodeTreeViewItem> root)
        {
            foreach (var node in root)
            {
                if (node.Background.Equals(DiffNodeColor))
                {
                  node.CurrentControl.DisplayedName = node.CurrentControl.Name;
                    foreach (var property in node.CurrentControl.DisplayedProperties)
                    {
                      property.DisplayedValue = property.Value;
                    }
                    node.ChangedProperties = new HashSet<string>();
                }
                node.Background = Brushes.Transparent;
                ClearTree(node.Items.Cast<NodeTreeViewItem>().ToList());
            }
        }
        
        private static bool CheckDiffenence(List<DifferenceControlNode> differenceControlNodes)
        {
      for (int i = 0; i < differenceControlNodes.Count; ++i)
            {
                var differenceControlNode = differenceControlNodes[i];
                if (
                    differenceControlNodes.FindAll(
                        x =>
                            x.TechPath == differenceControlNode.TechPath).Count > 1)
                {
                    return false;
                }
            }
            return true;
        }

      public static void MarkDiff(List<NodeTreeViewItem> defaultTreeViewList, DifferenceControlNode root)
      {
        var differenceControlNodes = root.Children;
        if (differenceControlNodes.Count == 0)
          throw new Exception("Empty diff");
        bool hasDuplicateNodes = !CheckDiffenence(differenceControlNodes);

        foreach (var differenceControlNode in differenceControlNodes)
        {
          List<NodeTreeViewItem> currentNodes = defaultTreeViewList;
          NodeTreeViewItem findingNode = new NodeTreeViewItem();
          foreach (var nextNodeNum in differenceControlNode.TechPath)
          {
            if (currentNodes.Count <= nextNodeNum)
            {
              ClearTree(defaultTreeViewList);
              throw new Exception("Incorrect diff");
            }
            findingNode = currentNodes[nextNodeNum];

            currentNodes = findingNode.Items.Cast<NodeTreeViewItem>().ToList();
          }
          if (!findingNode.Background.Equals(DiffNodeColor))
          {
            findingNode.Background = DiffNodeColor;
            ColorPossiblePath(findingNode);

            if (findingNode.CurrentControl.Name != differenceControlNode.ExpectedName)
              throw new Exception("Incorrect snapshot/diff loaded");


            foreach (var differentProperty in differenceControlNode.DifferentProperties)
            {
              if (differentProperty.Name == "Name")
              {
                if (differentProperty.Mode == Modes.SpecialWithRegularExpression)
                {
                  findingNode.CurrentControl.DisplayedName = differentProperty.Name;
                  findingNode.CurrentControl.DisplayedName += "\" doesn't much Reg Exp \"" +
                                                              differentProperty.RegExp;

                  findingNode.ChangedProperties.Add("Name");
                  continue;
                }
                if (differentProperty.Mode == Modes.Special)
                {
                  findingNode.CurrentControl.DisplayedName = differentProperty.Name;
                  findingNode.CurrentControl.DisplayedName += "\" doesn't much line \"" +
                                                              differentProperty.RegExp;

                  findingNode.ChangedProperties.Add("Name");
                  continue;
                }
                if (findingNode.CurrentControl.Name != differenceControlNode.Name)
                {
                  findingNode.CurrentControl.DisplayedName += "\" expected, but received \"" +
                                                              differenceControlNode.Name;

                  findingNode.ChangedProperties.Add("Name");
                  continue;
                }
              }

              var differentPropertyNode =
                findingNode.CurrentControl.DisplayedProperties.Find(x => x.Name == differentProperty.Name);

              if (differentPropertyNode == null)
                throw new Exception("Incorrect snapshot/diff loaded");

              if (differentProperty.Mode == Modes.SpecialWithRegularExpression)
              {
                differentPropertyNode.DisplayedValue = differentProperty.Value;
                differentPropertyNode.DisplayedValue = "\"" + differentProperty.Value + "\" doesn't much Reg Exp \"" +
                                                       differentProperty.RegExp;

                findingNode.ChangedProperties.Add(differentPropertyNode.Name);
                continue;
              }
              if (differentProperty.Mode == Modes.Special)
              {
                differentPropertyNode.DisplayedValue = differentProperty.Value;
                differentPropertyNode.DisplayedValue += "\" doesn't much line \"" +
                                                        differentProperty.RegExp;

                findingNode.ChangedProperties.Add(differentPropertyNode.Name);
                continue;
              }

              if (differentPropertyNode.Value != differentProperty.Value)
              {
                
                differentPropertyNode.DisplayedValue += "\" expected, but received \"" +
                                                        differentProperty.Value;
                findingNode.ChangedProperties.Add(differentProperty.Name);
              }

            }

          }
        }
        if (hasDuplicateNodes)
          throw new Exception("Duplicate Nodes");
      }

      private static void ColorPossiblePath(NodeTreeViewItem findingItem)
        {
            while (true)
            {
                ItemsControl parentItem = ItemsControl.ItemsControlFromItemContainer(findingItem);

                findingItem = parentItem as NodeTreeViewItem;
                if (findingItem == null)
                    return;
                if (!findingItem.Background.Equals(DiffNodeColor))
                    findingItem.Background = DiffPathColor;
            }
        }

#endregion
      /// <summary>
      /// Try to apply diff to snapshot and create error-messageBox when can't
      /// </summary>
      /// <param name="defaultTreeViewList"></param>
      /// <param name="root"></param>
        public static void ApplyDifference(List<NodeTreeViewItem> defaultTreeViewList, DifferenceControlNode root)
        {
            ClearTree(defaultTreeViewList);
            try
            {
                MarkDiff(defaultTreeViewList, root);
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case "Empty diff":
                        MessageBox.Show("Empty diff");
                        break;
                    case "Duplicate Nodes":
                        MessageBox.Show("Duplicate nodes in diff, some data can be lost");
                        break;
                    case "Incorrect snapshot/diff loaded":
                        MessageBox.Show("Incorrect snapshot/diff loaded");
                        break;
                    default:
                        MessageBox.Show("Error: incorrect diff");
                        break;
                }
            }
        }
    }
}