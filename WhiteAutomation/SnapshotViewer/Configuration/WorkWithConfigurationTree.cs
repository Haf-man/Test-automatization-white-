using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ConfigurationReader;
using AutomationTree;
using SnapshotViewer.Description;
using SnapshotViewer.Snapshot;
using CompareScope = ConfigurationReader.CompareScopes;


namespace SnapshotViewer.Configuration
{
  public static class WorkWithConfigurationTree
  {
    #region public

    #region SnapshotContextMenuHandlers

    public static void HandleAddControlToIncluded(object sender, RoutedEventArgs e)
    {
      NodeTreeViewItem item = MainWindow.SelectedSnapshotControl;
      AddItemToConfiguration(item, CongigurationHeader.IncludedControls);
    }

    public static void HandleAddControlToExcluded(object sender, RoutedEventArgs e)
    {
      NodeTreeViewItem item = MainWindow.SelectedSnapshotControl;
      AddItemToConfiguration(item, CongigurationHeader.ExcludedControls);
    }

    #endregion SnapshotContextMenuHandlers

    #region DescriptionContextMenuHandlers

    public static void HandleAddPropertyToInclude(object sender, RoutedEventArgs e)
    {
      Label property = MainWindow.CurrentUndermouseElement as Label;
      if (property == null)
        return;

      AddItemToConfiguration(DescriptionUtil.GetFormattedNameFromLabel(property), CongigurationHeader.IncludedProperty);
    }

    public static void HandleAddPropertyToExclude(object sender, RoutedEventArgs e)
    {
      Label property = MainWindow.CurrentUndermouseElement as Label;
      if (property == null)
        return;

      AddItemToConfiguration(DescriptionUtil.GetFormattedNameFromLabel(property), CongigurationHeader.ExcludedProperty);
    }

    public static void HandleAddPropertyToSelectedNode(object sender, RoutedEventArgs e)
    {
      Label propertyLabel = MainWindow.CurrentUndermouseElement as Label;
      if (propertyLabel == null)
        return;

      if (LastSelectedConfigurationNode == null)
        return;

      AddPropertyToSelectedNode(DescriptionUtil.GetFormattedNameFromLabel(propertyLabel),
        DescriptionUtil.GetValueForCurrentProperty(propertyLabel, MainWindow.CurrentDescriptionGrid),
        LastSelectedConfigurationNode);
    }

    #endregion DescriptionContextMenuHandlers

    #region ConfigurationContextMenuHandlers

    public static void HandleDeleteControl(object sender, RoutedEventArgs e)
    {
      MainWindow.CurrentConfiguration.DeleteFromControls(GetCurrentConfigurationNodeIdentifier(CurrentConfigurationNode));
      DeleteEntryFromConfiguration(CurrentConfigurationNode);
    }

    public static void HandleDeleteControlProperty(object sender, RoutedEventArgs e)
    {
      object currentNode = MainWindow.CurrentUndermouseElement;
      StackPanel stack = currentNode as StackPanel;
      PropertyTreeViewItem property = stack.Parent as PropertyTreeViewItem;

      property.ParentControl.DeleteProperty(property);
    }

    public static void HandleDeleteGlobalPropery(object sender, RoutedEventArgs e)
    {
      TreeViewItem currentNode = MainWindow.CurrentUndermouseElement as TreeViewItem;

      MainWindow.CurrentConfiguration.DeleteGlobalPropery((string) currentNode.Header);

      DeleteEntryFromConfiguration(currentNode);
    }

    public static void HandleAddPropertyFromListToControl(object sender, RoutedEventArgs e)
    {
      ControlTreeViewItem controlNode = CurrentConfigurationNode as ControlTreeViewItem;

      if (controlNode == null)
        return;
      MenuItem menuItem = sender as MenuItem;
      string property = (string) menuItem.Header;

      AddPropertyToSelectedNode(property, String.Empty, controlNode);
    }

    public static void HandleAddPropertyFromListToGlobal(object sender, RoutedEventArgs e)
    {
      MenuItem menuItem = sender as MenuItem;
      string property = (string) menuItem.Header;

      MainWindow.CurrentConfiguration.AddProperties(SelectedGlobalPropertiesBranch, new[] {property});
    }

    public static void HandleGoToSnapshotNode(object sender, RoutedEventArgs e)
    {
      ControlTreeViewItem controlNode = CurrentConfigurationNode as ControlTreeViewItem;

      if (controlNode == null)
        return;

      controlNode.SelectCurrentItemInSnapshot();
    }

    public static void ControlOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      TreeViewItem node = sender as TreeViewItem;
      LastSelectedConfigurationNode = node as ControlTreeViewItem;
    }

    public static void ControlOnMouseRightButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
    {
      TreeViewItem node = sender as TreeViewItem;
      CurrentConfigurationNode = node;
    }

    public static void GlobalPropertiesMouseButtonUp(object sender, MouseButtonEventArgs e)
    {
      SelectedGlobalPropertiesBranch = sender as TreeViewItem;
    }

    #endregion ConfigurationContextMenuHandlers

    #endregion

    #region private

    public enum CongigurationHeader : int
    {
      IncludedControls = 0,
      ExcludedControls,
      IncludedProperty,
      ExcludedProperty
    }

    private static readonly string[] ConfigurationHeaders = new string[4]
    {"Included controls", "Excluded controls", "Included properties", "Excluded properties"};

    private static TreeViewItem CurrentConfigurationNode { get; set; }
    private static ControlTreeViewItem LastSelectedConfigurationNode { get; set; }
    private static TreeViewItem SelectedGlobalPropertiesBranch { get; set; }

    private static void AddItemToConfiguration(object itemToAddObj, CongigurationHeader header)
    {
      var currentConfig = MainWindow.CurrentConfiguration.ConfigurationTree;

      TreeViewItem neededSubTree = currentConfig.Find(x => (string) x.Header == ConfigurationHeaders[(int) header]);

      switch (header)
      {
        case CongigurationHeader.IncludedControls:
          AddControlToConfiguration(neededSubTree, itemToAddObj,
            ConfigurationControlType.Included);
          return;
        case CongigurationHeader.ExcludedControls:
          AddControlToConfiguration(neededSubTree, itemToAddObj,
            ConfigurationControlType.Excluded);
          return;
        case CongigurationHeader.IncludedProperty:
        case CongigurationHeader.ExcludedProperty:
          AddPropertyToConfiguration(neededSubTree, itemToAddObj);
          return;
      }
    }

    private static void AddControlToConfiguration(TreeViewItem node, object item,
      ConfigurationControlType configurationControlType)
    {
      NodeTreeViewItem snapshotNode = item as NodeTreeViewItem;

      if (snapshotNode == null)
        return;

      ControlNode controlNode = snapshotNode.CurrentControl;

      ControlConfigurationsControlConfiguration control = new ControlConfigurationsControlConfiguration
      {
        AutomationId = controlNode.Id,
        Name = controlNode.Name,
        Path = controlNode.PathToString(),
        ControlType = controlNode.Type,
        InterestingProperties = new ControlPropertiesControlPropertie[controlNode.Properties.Count],
        CompareScopeSpecified = false,
        TechPath = snapshotNode.TechPath
      };

      for (int i = 0, propertiesAmount = controlNode.Properties.Count; i < propertiesAmount; ++i)
      {
        control.InterestingProperties[i] = new ControlPropertiesControlPropertie
        {
          Name = controlNode.Properties[i].Name,
          Value = controlNode.Properties[i].Value,
          CompareScope = CompareScopes.OnlyControl
        };
      }

      MainWindow.CurrentConfiguration.AddControls(node, new ControlConfigurationsControlConfiguration[1] {control},
        configurationControlType, snapshotNode);
    }

    private static ControlIdentifier GetCurrentConfigurationNodeIdentifier(TreeViewItem node)
    {
      return ((ControlTreeViewItem) node).Identifier;
    }

    private static void AddPropertyToConfiguration(TreeViewItem node, object item)
    {
      MainWindow.CurrentConfiguration.AddProperties(node, new string[1] {(string) item});
    }

    private static void AddPropertyToSelectedNode(string propertyName, string propertyValue,
      ControlTreeViewItem selectedNode)
    {
      var propertyNode = new PropertyTreeViewItem(propertyName, propertyValue, CompareScope.OnlyControl, selectedNode);
      if (!selectedNode.ControlProperties.Contains(propertyName))
      {
        propertyNode.IsCompareScopeChangeEnabled = false;
        propertyNode.Scope = CompareScope.OnlyChildren;
      }

      selectedNode.AddProperty(propertyNode);
    }

    private static void DeleteEntryFromConfiguration(TreeViewItem node)
    {
      TreeViewItem parentNode = node.Parent as TreeViewItem;

      if (parentNode == null)
        throw new ArgumentNullException("node", "Node doesn't have valid parent");

      parentNode.Items.Remove(node);
    }

    #endregion
  }
}