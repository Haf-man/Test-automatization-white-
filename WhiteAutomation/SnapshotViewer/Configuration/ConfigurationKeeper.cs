using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using ConfigurationReader;
using SnapshotViewer.Snapshot;

namespace SnapshotViewer.Configuration
{
  public delegate void ConfigurationChangedEventHandler(object sender, bool configurationGood);

  public delegate void MessageEventHandler(string message);

  public class ConfigurationKeeper
  {
    public static TreeView ConfigurationUiTree { set; get; }
    public bool IsConfigurationGood { get; private set; }

    private readonly ConfigurationReader.Configuration _configuration;
    private List<TreeViewItem> _configurationTree;
    private readonly HashSet<string> _globalProperties = new HashSet<string>();
    private readonly List<ControlIdentifier> _controls = new List<ControlIdentifier>();

    public List<TreeViewItem> ConfigurationTree
    {
      get
      {
        if (_configurationTree == null)
        { 
          _configurationTree = CreateConfigurationTree();
          ConfigurationUiTree.ItemsSource = _configurationTree;
        }
        return _configurationTree;
      }
      set { _configurationTree = value; }
    }

    public bool IsControlInConfig(ControlIdentifier identifier)
    {
      return _controls.FindLast(x => x.AutomationId == identifier.AutomationId && x.Path == identifier.Path) !=
             null;
    }

    public void AddToControls(ControlIdentifier identifier)
    {
      _controls.Add(identifier);
    }

    public void DeleteFromControls(ControlIdentifier identifier)
    {
      _controls.Remove(
        _controls.FindLast(x => x.AutomationId == identifier.AutomationId && x.Path == identifier.Path));
    }

    public bool IsThatPropertyGlobal(string propertyName)
    {
      return _globalProperties.Contains(propertyName);
    }

    public void AddToGlobalProperty(string properyName)
    {
      _globalProperties.Add(properyName);
    }

    public void DeleteGlobalPropery(string propertyName)
    {
      _globalProperties.Remove(propertyName);
    }

    public ConfigurationKeeper(ConfigurationReader.Configuration configuration)
    {
      _configuration = configuration;
      IsConfigurationGood = true;
    }

    public ConfigurationKeeper()
    {
      _configuration = new ConfigurationReader.Configuration();
      IsConfigurationGood = true;
    }

    private List<TreeViewItem> CreateConfigurationTree()
    {
      List<TreeViewItem> rootTreeViewItem = new List<TreeViewItem>();

      TreeViewItem includedControlsItem = new TreeViewItem {Header = "Included controls"};
      AddControls(includedControlsItem, _configuration.IncludedControls, ConfigurationControlType.Included);

      TreeViewItem excludedControlsItem = new TreeViewItem {Header = "Excluded controls"};
      AddControls(excludedControlsItem, _configuration.ExcludedControls, ConfigurationControlType.Excluded);

      TreeViewItem includedPropertiesItem = new TreeViewItem
      {
        Header = "Included properties",
        ContextMenu = ConfigurationContextMenu.GetMenuForGlobalPropertiesHandler()
      };
      includedPropertiesItem.MouseRightButtonUp += WorkWithConfigurationTree.GlobalPropertiesMouseButtonUp;
      includedPropertiesItem.MouseLeftButtonUp += WorkWithConfigurationTree.GlobalPropertiesMouseButtonUp;
      AddProperties(includedPropertiesItem, _configuration.IncludedProperties);

      TreeViewItem excludedPropertiesItem = new TreeViewItem
      {
        Header = "Excluded properties",
        ContextMenu = ConfigurationContextMenu.GetMenuForGlobalPropertiesHandler()
      };
      excludedPropertiesItem.MouseRightButtonUp += WorkWithConfigurationTree.GlobalPropertiesMouseButtonUp;
      excludedPropertiesItem.MouseLeftButtonUp += WorkWithConfigurationTree.GlobalPropertiesMouseButtonUp;
      AddProperties(excludedPropertiesItem, _configuration.ExcludedProperties);

      rootTreeViewItem.Add(includedControlsItem);
      rootTreeViewItem.Add(excludedControlsItem);
      rootTreeViewItem.Add(includedPropertiesItem);
      rootTreeViewItem.Add(excludedPropertiesItem);

      return rootTreeViewItem;
    }

    public void AddControls(TreeViewItem viewItem, ControlConfigurationsControlConfiguration[] controls,
      ConfigurationControlType configurationControlType, NodeTreeViewItem snapshotNode = null)
    {
      if (controls == null)
        return;

      foreach (var controlNode in controls)
      {
        ControlIdentifier controlIdentifier = new ControlIdentifier
        {
          AutomationId = controlNode.AutomationId,
          Path = controlNode.Path
        };
        if (IsControlInConfig(controlIdentifier))
        {
          MainWindow.ShowMessage(String.Format("This \"{0}\" control already in config", controlNode.AutomationId));
          return;
        }

        ControlTreeViewItem controlTreeViewItem = new ControlTreeViewItem(controlNode, configurationControlType);
        controlTreeViewItem.ConfigurationChanged += (sender, good) => { IsConfigurationGood = good; };
        controlTreeViewItem.Message += MainWindow.ShowMessage;

        viewItem.Items.Add(controlTreeViewItem);
        AddToControls(controlIdentifier);
      }
    }

    public void AddProperties(TreeViewItem viewItem, string[] properties)
    {
      if (properties == null)
        return;
      foreach (string property in properties)
      {
        if (!IsThatPropertyGlobal(property))
        {
          TreeViewItem item = new TreeViewItem
          {
            Header = property.Split('.').Last(),
            ContextMenu = ConfigurationContextMenu.GetMenuForGlobalProperty()
          };
          item.MouseRightButtonUp += MainWindow.HandleMouseRightButtonUp;
          viewItem.Items.Add(item);
          AddToGlobalProperty(property);
        }
        else
        {
          MainWindow.ShowMessage(string.Format("\"{0}\" already in config", property));
        }
      }
    }

    public static bool CompareConfigurationSaved(ConfigurationReader.Configuration lastSavedConfiguration,
      ConfigurationReader.Configuration currentConfiguration)
    {
      if (currentConfiguration.IncludedProperties.Length != lastSavedConfiguration.IncludedProperties.Length)
        return false;
      foreach (var property in currentConfiguration.IncludedProperties)
      {
        var lastSaved = lastSavedConfiguration.IncludedProperties.FirstOrDefault(x => x == property);
        if (lastSaved == null)
          return false;
      }
      if (currentConfiguration.ExcludedProperties.Length != lastSavedConfiguration.ExcludedProperties.Length)
        return false;
      foreach (var property in currentConfiguration.ExcludedProperties)
      {
        var lastSaved = lastSavedConfiguration.ExcludedProperties.FirstOrDefault(x => x == property);
        if (lastSaved == null)
          return false;
      }
      foreach (var control in currentConfiguration.IncludedControls)
      {
        var lastSavedControl =
          lastSavedConfiguration.IncludedControls.FirstOrDefault(x => x.AutomationId == control.AutomationId &&
                                                             x.ControlType == control.ControlType &&
                                                             x.Path == control.Path);
        if (lastSavedControl == null)
          return false;
        foreach (var property in control.InterestingProperties)
        {
          var lastSavedProperty = lastSavedControl.InterestingProperties.FirstOrDefault(x => x.Name == property.Name &&
                                                                                    x.Value == property.Value &&
                                                                                    x.IsValueRegExp ==
                                                                                    property.IsValueRegExp &&
                                                                                    x.CompareScope ==
                                                                                    property.CompareScope);
          if (lastSavedProperty == null)
            return false;
        }
      }
      foreach (var control in currentConfiguration.ExcludedControls)
      {
        var lastSavedControl =
          lastSavedConfiguration.ExcludedControls.FirstOrDefault(x => x.AutomationId == control.AutomationId &&
                                                             x.ControlType == control.ControlType &&
                                                             x.Path == control.Path);
        if (lastSavedControl == null)
          return false;
        foreach (var property in control.InterestingProperties)
        {
          var lastSavedProperty = lastSavedControl.InterestingProperties.FirstOrDefault(x => x.Name == property.Name &&
                                                                                    x.Value == property.Value &&
                                                                                    x.IsValueRegExp ==
                                                                                    property.IsValueRegExp &&
                                                                                    x.CompareScope ==
                                                                                    property.CompareScope);
          if (lastSavedProperty == null)
            return false;
        }
      }
      return true;
    }
  }

 
  public enum ConfigurationControlType
  {
    Included,
    Excluded
  }

  public class ControlIdentifier : IEqualityComparer<ControlIdentifier>
  {
    public string AutomationId { get; set; }
    public string Path { get; set; }

    public bool Equals(ControlIdentifier x, ControlIdentifier y)
    {
      return x.AutomationId == y.AutomationId && x.Path == y.Path;
    }

    public int GetHashCode(ControlIdentifier obj)
    {
      int hCode = obj.AutomationId.GetHashCode() ^ obj.Path.GetHashCode();
      return hCode.GetHashCode();
    }
  }
}
