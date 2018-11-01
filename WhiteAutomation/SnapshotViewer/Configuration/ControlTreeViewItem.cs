using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using ConfigurationReader;
using SnapshotViewer.Snapshot;

namespace SnapshotViewer.Configuration
{
  /// <summary>
  /// Represents a control node in configuration tree
  /// </summary>
  public class ControlTreeViewItem : TreeViewItem
  {
    #region public

    public event ConfigurationChangedEventHandler ConfigurationChanged;
    public event MessageEventHandler Message;

    /// <summary>
    /// Unique identifier for control
    /// </summary>
    public ControlIdentifier Identifier { get; private set; }

    public List<PropertyTreeViewItem> Properties { get; set; }

    /// <summary>
    /// Contains the property that suitable to current control
    /// </summary>
    public List<string> ControlProperties { get; set; }

    public string TechPath
    {
      get { return _techPath;  }
    }

    public ControlTreeViewItem(ControlIdentifier identifier)
    {
      Identifier = identifier;
      ControlProperties = new List<string>();
      Properties = new List<PropertyTreeViewItem>();
      
      MouseRightButtonUp += WorkWithConfigurationTree.ControlOnMouseRightButtonUp;
      MouseLeftButtonUp += WorkWithConfigurationTree.ControlOnMouseLeftButtonUp;
    }

    public ControlTreeViewItem(ControlConfigurationsControlConfiguration controlNode, ConfigurationControlType type)
      : this(new ControlIdentifier {AutomationId = controlNode.AutomationId, Path = controlNode.Path})
    {
      Header = "\"" + controlNode.Name + "\"" + " (" + controlNode.ControlType.Split('.').Last() + ")";
      ContextMenu = ConfigurationContextMenu.GetMenuForControl();
      _techPath = controlNode.TechPath;

      AddMainAtributes(controlNode, type);

      AddProperties(controlNode);
    }


    public void SelectCurrentItemInSnapshot()
    {
      NodeTreeViewItem snapshotItem = FindNodeInSnapshot(_techPath);
      if (snapshotItem == null)
        return;

      snapshotItem.IsSelected = true;
      snapshotItem.ExpandTree();
    }

    public void AddProperty(PropertyTreeViewItem property)
    {
      if (!IsThatPropertyCanBeAdded(property.Name))
      {
        if (Message != null) Message(String.Format("\"{0}\" can't be added to this control anymore", property.Name));
        return;
      }
      Properties.Add(property);
      CheckForConflicts(property.Name);
      _propertiesBranch.Items.Add(property);
    }

    public void DeleteProperty(PropertyTreeViewItem property)
    {
      Properties.Remove(property);
      _propertiesBranch.Items.Remove(property);
    }

    public ControlConfigurationsControlConfiguration ToControlConfiguration()
    {
      ControlConfigurationsControlConfiguration control = new ControlConfigurationsControlConfiguration();

      return control;
    }

    public void CheckForConflicts(string propertyName)
    {
      List<PropertyTreeViewItem> properties = Properties.Where(property => property.Name == propertyName).ToList();

      bool isConfigurationGood = true;

      if (properties.Count == 1)
      {
        UnHighlightProperties(properties);
      } else if (properties.Count(property => property.Scope == CompareScopes.ChildrenWithControl) >= 1)
      {
        HighlightProperties(properties);
        isConfigurationGood = false;
      } else if (properties.Count(property => property.Scope == CompareScopes.OnlyChildren) == 2 ||
          properties.Count(property => property.Scope == CompareScopes.OnlyControl) == 2)
      {
        HighlightProperties(properties);
        isConfigurationGood = false;
      }

      if (isConfigurationGood)
      UnHighlightProperties(properties);
      if (ConfigurationChanged != null) 
        ConfigurationChanged(this, isConfigurationGood);
    }

    #endregion

    #region private

    private NodeTreeViewItem _snapshotNode = null;
    private string _techPath = String.Empty;
    private TreeViewItem _propertiesBranch = new TreeViewItem{ Header = "Properties"};
    private static readonly Brush ConflictedPropertiesColor = Brushes.Coral;
    private static readonly Brush NormalPropertiesColor = Brushes.Transparent;

    private void HighlightProperties(List<PropertyTreeViewItem> properties)
    {
      foreach (var propertyTreeViewItem in properties)
      {
        propertyTreeViewItem.Background = ConflictedPropertiesColor;
      }
    }

    private void UnHighlightProperties(List<PropertyTreeViewItem> properties)
    {
      foreach (var propertyTreeViewItem in properties)
      {
        propertyTreeViewItem.Background = NormalPropertiesColor;
      }
    }

    private void AddProperties(ControlConfigurationsControlConfiguration controlNode)
    {
      List<string> controlProperties = new List<string>();
      foreach (var property in controlNode.InterestingProperties)
      {
        string propertyName = property.Name.Split('.').Last();
        if (!IsThatPropertyCanBeAdded(propertyName))
          continue;

        PropertyTreeViewItem propertyItem = new PropertyTreeViewItem(propertyName,
          property.Value, property.CompareScope, this);

        AddProperty(propertyItem);
        controlProperties.Add(propertyItem.Name);
      }

      Items.Add(_propertiesBranch);
      controlProperties.Add("NameProperty");
      ControlProperties = controlProperties;
    }

    private void AddMainAtributes(ControlConfigurationsControlConfiguration controlNode, ConfigurationControlType type)
    {
      var item = new MainAtributeTreeViewItem("Automation ID:", controlNode.AutomationId);
      Items.Add(item);

      item = new MainAtributeTreeViewItem("Path:", controlNode.Path);
      Items.Add(item);

      item = new MainAtributeTreeViewItem("Type:", controlNode.ControlType);
      Items.Add(item);

      if (type == ConfigurationControlType.Excluded)
      {
        item = MainAtributeTreeViewItem.GetMainItemWithScope("Scope of exclusion:",
          scope: controlNode.CompareScopeSpecified ? controlNode.CompareScope.ToString() : null);
        Items.Add(item);
      }
    }

    private NodeTreeViewItem FindNodeInSnapshot(string techPath)
    {
      if (MainWindow.CurrentSnapshot == null)
      {
        if (Message != null) Message("Snapshot isn't loaded");
        return null;
      }

      if (string.IsNullOrEmpty(_techPath))
      {
        if (Message != null) Message("This configuration item doesn't have TechPath");
        return null;
      }
      if (techPath == @"/0")
        return MainWindow.CurrentSnapshot;

      List<string> convertedPath = techPath.Split('/').ToList();
      convertedPath.RemoveAt(0);
      convertedPath.RemoveAt(0);
      List<int> path = convertedPath.Select(node => Convert.ToInt32(node)).ToList();

      NodeTreeViewItem currentItem = null;
      try
      {
        currentItem = MainWindow.CurrentSnapshot;
        currentItem = path.Aggregate(currentItem, (current, i) => current.Items[i] as NodeTreeViewItem);
      }
      //if something bad happened
      catch (Exception)
      {
        if (Message != null) Message("Can't find element. Maybe wrong snapshot?");
        currentItem = null;
      }

      return currentItem;
    }

    private bool IsThatPropertyCanBeAdded(string propertyName)
    {
      return Properties.Count(property => property.Name == propertyName) < 2;
    }

    #endregion
  }
}