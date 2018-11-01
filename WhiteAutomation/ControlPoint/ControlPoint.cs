using System;
using System.IO;
using System.Windows;
using AutomationTree;
using AutomationTreeComparator;
using ConfigurationReader;
using TestStack.White.UIItems;
using TreeGenerator;

namespace ControlPoint
{
  public static class ControlPoint
  {
    #region public

    public enum ActionTypes
    {
      WriteTree,
      CheckTree
    }

    public static readonly string SnapshotExtension = ".xml";
    public static readonly string ConfigExtenstion = ".config.xml";
    public static readonly string DiffExtension = ".out.xml";

    public static bool IsContinueMessageboxEnabled { get; set; } 

    public static void PerformAction(string pointName, ActionTypes actionType, string parentAutomationId,
      string childAutomationId = null)
    {
      if (string.IsNullOrEmpty(pointName))
        throw new ArgumentException("pointName length must be more than 0", "pointName");
      if (string.IsNullOrEmpty(parentAutomationId))
        throw new ArgumentException("parentAutomationId length must be more than 0", "parentAutomationId");
      if (string.Empty == childAutomationId)
        throw new ArgumentException("childAutomationId length must be more than 0", "childAutomationId");

      var currentTree = SnapshotGenerator.GenerateTreeByAutomationId(parentAutomationId, childAutomationId);

      Action(pointName, currentTree, actionType);
    }

    public static void PerformAction(string pointName, ActionTypes actionType, IUIItem currentElement)
    {
      if (string.IsNullOrEmpty(pointName))
        throw new ArgumentException("pointName length must be more than 0", "pointName");
      if (currentElement == null)
        throw new ArgumentNullException("currentElement");

      var currentTree = SnapshotGenerator.GenerateTree(currentElement.AutomationElement);

      Action(pointName, currentTree, actionType);
    }

    static ControlPoint()
    {
      IsContinueMessageboxEnabled = true;
    }

    #endregion

    #region private

    private static void Action(string pointName, ControlNode currentTree, ActionTypes actionType)
    {
      switch (actionType)
      {
        case ActionTypes.CheckTree:
          CompareTrees(currentTree, pointName + SnapshotExtension, pointName + ConfigExtenstion,
            pointName + DiffExtension);
          break;
        case ActionTypes.WriteTree:
          XmlSnapshot.XmlSnapshot.WriteToXmlFile(currentTree, pointName + SnapshotExtension);
          break;
      }

      if (IsContinueMessageboxEnabled)
      {
        MessageBox.Show(String.Format("\"{0}\" passed. Continue?", pointName), "Test is over");
      }
    }

    private static void CompareTrees(ControlNode tree1, string xmlPath, string configPath, string outPath)
    {
      ControlNode tree2 = XmlSnapshot.XmlSnapshot.ReadFromXmlFile(xmlPath);
      Configuration configuration;
      try
      {
        configuration = ConfigurationReaderWriter.ReadConfiguration(configPath);
      }
      // Problem with file. Checking all
      catch (FileNotFoundException)
      {
        configuration = new Configuration();
      }

      var result = Comparator.CompareTrees(tree1, tree2, configuration);
      Comparator.ResultsToFile(result, outPath);
    }

    #endregion
  }
}
