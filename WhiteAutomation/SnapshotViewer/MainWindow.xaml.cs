using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Navigation;
using DifferenceTree;
using SnapshotViewer.Configuration;
using SnapshotViewer.Description;
using SnapshotViewer.Snapshot;

namespace SnapshotViewer
{
  /// <summary>
  /// Логика взаимодействия для MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      ConfigurationKeeper.ConfigurationUiTree = ConfigurationTree;
      CurrentConfiguration = new ConfigurationKeeper();
      CurrentPath = string.Empty;
      LoadDiffMenu.IsEnabled = false;

      ConfigurationTree.ItemsSource = FileHelper.EmptyConfiguration();
      LastSavedConfiguration = ConfigurationSaver.GetCurrentConfiguration(CurrentConfiguration.ConfigurationTree);
    }


    private void MenuItem_Load_OnClick(object sender, RoutedEventArgs e)
    {
      OpenFileDialog fileDialog = new OpenFileDialog
      {
        Title = "Open snapshot",
        DefaultExt = ControlPoint.ControlPoint.SnapshotExtension,
        Filter = "XML documents|*" + ControlPoint.ControlPoint.SnapshotExtension + "|All files|*.*"
      };

      bool? result = fileDialog.ShowDialog();

      if (result == true)
      {
        string fileName = fileDialog.FileName;
        List<NodeTreeViewItem> treeViewItems = FileHelper.LoadSnapshot(fileName);
        if (treeViewItems == null)
        {
          MessageBox.Show("Something wrong with snapshot file");
        }
        else
        {
          if (treeViewItems.Count == 0)
          {
            MessageBox.Show("This snapshot is empty!");
            return;
          }
          treeViewItems.FirstOrDefault().IsExpanded = true;
          CurrentSnapshot = treeViewItems.FirstOrDefault();
          AutomationNodesTree.ItemsSource = treeViewItems;
          LoadDiffMenu.IsEnabled = true;
        }
      }
    }

    private void MenuItem_LoadAllByOne(object sender, RoutedEventArgs e)
    {
      OpenFileDialog fileDialog = new OpenFileDialog
      {
        Title = "Choose snapshot",
        DefaultExt = ControlPoint.ControlPoint.SnapshotExtension,
        Filter = "XML documents|*" + ControlPoint.ControlPoint.SnapshotExtension + "|All files|*.*"
      };

      bool? result = fileDialog.ShowDialog();

      if (result == true)
      {
        string fileName = fileDialog.FileName;
        List<NodeTreeViewItem> snapshot;
        List<TreeViewItem> config;
        DifferenceControlNode diffRoot;
        FileHelper.LoadAll(fileName, out snapshot, out config, out diffRoot);
        if (snapshot == null)
        {
          MessageBox.Show("Something wrong with snapshot");
          return;
        }
        if (config == null)
        {
          MessageBox.Show("Something wrong with config file");
          return;
        }

        CurrentSnapshot = snapshot.FirstOrDefault();
        AutomationNodesTree.ItemsSource = snapshot;
        ConfigurationTree.ItemsSource = config;
        FileHelper.ApplyDifference(snapshot, diffRoot);
        LastSavedConfiguration = ConfigurationSaver.GetCurrentConfiguration(CurrentConfiguration.ConfigurationTree);
      }
    }

    private void nodeTreeView_Item_Selected(object sender, RoutedEventArgs e)
    {
      NodeTreeViewItem treeViewItem = e.OriginalSource as NodeTreeViewItem;
      SelectedSnapshotControl = treeViewItem;

      DescriptionGrid.Children.Clear();


      WorkWithGrid.AddHeader("General atributes", DescriptionGrid);


      WorkWithGrid.AddProperty("NameProperty", treeViewItem.CurrentControl.DisplayedName,
        isFieldChanged("Name", treeViewItem), DescriptionGrid, true);
      WorkWithGrid.AddProperty("AutomationId", treeViewItem.CurrentControl.DisplayedId, DescriptionGrid);
      WorkWithGrid.AddProperty("Type", treeViewItem.CurrentControl.DisplayedType.Split('.').Last(), DescriptionGrid);
      WorkWithGrid.AddProperty("Path", treeViewItem.CurrentControl.DisplayedPathToString(), DescriptionGrid);

      WorkWithGrid.AddHeader("Properties", DescriptionGrid);
      foreach (var propertiesNode in treeViewItem.CurrentControl.DisplayedProperties)
      {
        WorkWithGrid.AddProperty(propertiesNode.DisplayedName.Split('.').Last(), propertiesNode.DisplayedValue,
          isFieldChanged(propertiesNode.Name, treeViewItem), DescriptionGrid,
          true, propertiesNode.IsValueSequence);
      }

      WorkWithGrid.AddHeader("ControlPatterns", DescriptionGrid);
      foreach (var patternsNode in treeViewItem.CurrentControl.Paterns)
      {
        WorkWithGrid.AddProperty(patternsNode.Name.Split('.').First(), String.Empty, false, DescriptionGrid);
      }

      CurrentDescriptionGrid = DescriptionGrid;
    }

    private bool isFieldChanged(string fieldName, NodeTreeViewItem nodeTreeViewItem)
    {
      if (nodeTreeViewItem.ChangedProperties.Contains(fieldName))
        return true;
      else
        return false;
    }

    private void MenuItem_LoadDiff_OnClick(object sender, RoutedEventArgs e)
    {
      OpenFileDialog fileDialog = new OpenFileDialog
      {
        Title = "Open difference",
        DefaultExt = ControlPoint.ControlPoint.DiffExtension,
        Filter = "XML documents|*" + ControlPoint.ControlPoint.DiffExtension + "|All files|*.*"
      };
      var treeView = AutomationNodesTree.ItemsSource;

      bool? result = fileDialog.ShowDialog();

      if (result == true)
      {
        string fileName = fileDialog.FileName;
        DifferenceControlNode root = FileHelper.LoadDifference(fileName);
        if (root == null)
        {
          MessageBox.Show("Something wrong with diff file");
        }
        else
        {
          if (treeView == null)
          {
            MessageBox.Show("No loaded snapshot");
            return;
          }
          FileHelper.ApplyDifference(treeView.Cast<NodeTreeViewItem>().ToList(), root);
        }
      }
    }


    private void MenuItem_LoadConfiguration_OnClick(object sender, RoutedEventArgs e)
    {
      OpenFileDialog fileDialog = new OpenFileDialog
      {
        Title = "Open config",
        DefaultExt = ControlPoint.ControlPoint.ConfigExtenstion,
        Filter = "XML documents|*" + ControlPoint.ControlPoint.ConfigExtenstion + "|All files|*.*"
      };

      bool? result = fileDialog.ShowDialog();

      if (result == true)
      {
        string fileName = fileDialog.FileName;
        List<TreeViewItem> treeViewItems = FileHelper.LoadConfiguration(fileName);
        if (treeViewItems == null)
        {
          MessageBox.Show("Something wrong with configuration file");
        }
        else
        {
          ConfigurationTree.ItemsSource = CurrentConfiguration.ConfigurationTree;
          CurrentPath = fileName;
          SaveConfiguration.IsEnabled = true;
          LastSavedConfiguration = ConfigurationSaver.GetCurrentConfiguration(CurrentConfiguration.ConfigurationTree);
        }
      }

    }


    public static ConfigurationKeeper CurrentConfiguration { get; set; }
    public static Grid CurrentDescriptionGrid { get; set; }
    public static NodeTreeViewItem SelectedSnapshotControl { get; set; }

    public static void ShowMessage(string message)
    {
      MessageBox.Show(message);
    }

    public static NodeTreeViewItem CurrentSnapshot { get; set; }

    private static UIElement _currentUndermouseElement;
    private static ConfigurationReader.Configuration LastSavedConfiguration { get; set; }

    public static void HandleMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      _currentUndermouseElement = sender as UIElement;
    }

    public static UIElement CurrentUndermouseElement
    {
      get { return _currentUndermouseElement; }
    }

    private void Hyperlink_OnRequestNavigateerlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
      e.Handled = true;
    }

    private static string CurrentPath { get; set; }

    private void MenuItem_SaveConfigAs_OnClick(object sender, RoutedEventArgs e)
    {
      if (!CheckConfiguration())
        return;

      SaveFileDialog fileDialog = new SaveFileDialog
      {
        Title = "Save configuration as...",
        DefaultExt = ControlPoint.ControlPoint.ConfigExtenstion,
        Filter = "XML documents|*" + ControlPoint.ControlPoint.ConfigExtenstion + "|All files|*.*"
      };
      bool? result = fileDialog.ShowDialog();
      if (result == true)
      {
        string path = fileDialog.FileName;
        ConfigurationSaver.SaveConfigurationFromUi(path, CurrentConfiguration.ConfigurationTree);
        CurrentPath = path;
        SaveConfiguration.IsEnabled = true;
        LastSavedConfiguration = ConfigurationSaver.GetCurrentConfiguration(CurrentConfiguration.ConfigurationTree);
      }
      else
      {
        MessageBox.Show("Can't save configuration");
      }

    }

    private bool CheckConfiguration()
    {
      if (!CurrentConfiguration.IsConfigurationGood)
        MessageBox.Show("You have unresolved conflicts in config");
      return CurrentConfiguration.IsConfigurationGood;
    }

    private void MenuItem_SaveConfig_OnClick(object sender, RoutedEventArgs e)
    {
      if (!CheckConfiguration())
        return;
      ConfigurationSaver.SaveConfigurationFromUi(CurrentPath, CurrentConfiguration.ConfigurationTree);
      LastSavedConfiguration = ConfigurationSaver.GetCurrentConfiguration(CurrentConfiguration.ConfigurationTree);
    }

    private void Exit_OnClick(object sender, RoutedEventArgs e)
    {

      Close();
    }

    private void saveConfig(CancelEventArgs e)
    {
      if (!CheckConfiguration())
      {
        e.Cancel = true;
        return;
      }

      SaveFileDialog fileDialog = new SaveFileDialog
      {
        Title = "Save configuration as...",
        DefaultExt = ControlPoint.ControlPoint.ConfigExtenstion,
        Filter = "XML documents|*" + ControlPoint.ControlPoint.ConfigExtenstion + "|All files|*.*"
      };
      bool? result = fileDialog.ShowDialog();
      if (result == true)
      {
        string path = fileDialog.FileName;
        ConfigurationSaver.SaveConfigurationFromUi(path, CurrentConfiguration.ConfigurationTree);
        CurrentPath = path;
        SaveConfiguration.IsEnabled = true;
        LastSavedConfiguration = ConfigurationSaver.GetCurrentConfiguration(CurrentConfiguration.ConfigurationTree);
      }
      else
      {
        MessageBox.Show("Can't save configuration");
        e.Cancel = true;
      }
    }

    private void MainWindow_OnClosing(object sender, CancelEventArgs e)
    {
      if (!ConfigurationKeeper.CompareConfigurationSaved(LastSavedConfiguration,
        ConfigurationSaver.GetCurrentConfiguration(CurrentConfiguration.ConfigurationTree)))
      {
        var result = MessageBox.Show("Do you want save unsaved configuration?", "Unsaved changes in configuration",
          MessageBoxButton.YesNoCancel);
        switch (result)
        {
          case MessageBoxResult.Cancel:
            e.Cancel = true;
            break;
          case MessageBoxResult.Yes:
            saveConfig(e);
            break;
        }
      }
    }
  }
}
