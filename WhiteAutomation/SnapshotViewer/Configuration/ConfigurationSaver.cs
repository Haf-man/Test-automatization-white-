using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using ConfigurationReader;
using TreeGenerator;

namespace SnapshotViewer.Configuration
{
  public static class ConfigurationSaver
  {
    public static void SaveConfigurationFromUi(string path, List<TreeViewItem> configuratoinTree)
    {
      ConfigurationReader.Configuration config = LoadConfigurationFromUi(configuratoinTree);
      path = path.Split('.').First() + ControlPoint.ControlPoint.ConfigExtenstion;
      ConfigurationReaderWriter.WriteConfiguration(path, config);
    }

    public static ConfigurationReader.Configuration GetCurrentConfiguration(List<TreeViewItem> configurationTree)
    {
      return LoadConfigurationFromUi(configurationTree);
    }
    private static ConfigurationReader.Configuration LoadConfigurationFromUi(List<TreeViewItem> configurationTree)
    {
      ConfigurationReader.Configuration config = new ConfigurationReader.Configuration();

      foreach (TreeViewItem item in configurationTree)
      {
        switch (item.Header.ToString())
        {
          case "Included controls":
          case "Excluded controls":
            AddControlsToConfig(config, item);
            break;
          case "Included properties":
          case "Excluded properties":
            AddPropertiesToConfig(config, item);
            break;
        }
      }

      return config;
    }

    private static void AddControlsToConfig(ConfigurationReader.Configuration config, TreeViewItem handler)
    {

      string controlType = handler.Header.ToString();
      List<ControlConfigurationsControlConfiguration> controls = new List<ControlConfigurationsControlConfiguration>();
      if (controlType == "Included controls")
      {
        if (config.IncludedControls != null)
          controls = config.IncludedControls.ToList();
      }
      else
      {
        if (config.ExcludedControls != null)
          controls = config.ExcludedControls.ToList();
      }

      foreach (TreeViewItem control in handler.Items)
      {
        ControlConfigurationsControlConfiguration controlConfiguration =
          new ControlConfigurationsControlConfiguration();

        foreach (TreeViewItem property in control.Items)
        {
          if (property is MainAtributeTreeViewItem)
          {
            var inputProperty = property as MainAtributeTreeViewItem;

            switch (inputProperty.Name)
            {
              case "Automation ID:":
                controlConfiguration.AutomationId = inputProperty.InputText;
                break;
              case "Path:":
                controlConfiguration.Path = inputProperty.InputText;
                break;
              case "Type:":
                controlConfiguration.ControlType = inputProperty.InputText;
                break;
              case "Scope of exclusion:":
                object value = inputProperty.GetControlScope();
                if (value == null)
                {
                  controlConfiguration.CompareScopeSpecified = false;
                  break;
                }
                controlConfiguration.CompareScopeSpecified = true;
                controlConfiguration.CompareScope = (CompareScopes) value;
                break;
            }
          }
          else
          {
            AddControlProperties(controlConfiguration, property);
          }
        }

        controlConfiguration.TechPath = (control as ControlTreeViewItem).TechPath;
        controls.Add(controlConfiguration);
      }
      if (controlType == "Included controls")
        config.IncludedControls = controls.ToArray();
      else
        config.ExcludedControls = controls.ToArray();
    }

    private static void AddControlProperties(ControlConfigurationsControlConfiguration config, TreeViewItem propertiesBranch)
    {
      config.InterestingProperties =
        (from PropertyTreeViewItem property in propertiesBranch.Items
          select property.ToControlPropertiesControlPropertie()).ToArray();
    }

    private static void AddPropertiesToConfig(ConfigurationReader.Configuration config, TreeViewItem handler)
    {
      string propertiesType = handler.Header.ToString();

      List<string> properties = new List<string>();
      foreach (TreeViewItem property in handler.Items)
      {
        properties.Add(GetPropertyFullName(property.Header.ToString()));
      }
      if (propertiesType == "Included properties")
        config.IncludedProperties = properties.ToArray();
      else
        config.ExcludedProperties = properties.ToArray();
    }

    private static string GetPropertyFullName(string property)
    {
      List<string> properties = SnapshotGenerator.AllInterestingProperties;
      string fullName = properties.Find(x => x.Split('.').Last() == property);
      if (fullName == null)
        throw new Exception("Incorrect property name");
      return fullName;
    }
  }
}
