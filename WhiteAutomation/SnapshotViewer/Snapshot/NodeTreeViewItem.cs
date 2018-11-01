using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using AutomationTree;
using SnapshotViewer.Configuration;

namespace SnapshotViewer.Snapshot
{
  public class DisplayedControlNode : ControlNode
  {
    public DisplayedControlNode(ControlNode controlNode)
      : base(controlNode.Name, controlNode.Id, controlNode.Type, controlNode.Path)
    {
      DisplayedName = Name;
      DisplayedId = Id;
      DisplayedType = Type;
      DisplayedPath = Path;
      DisplayedProperties = new List<DisplayedPropertiesNode>();

      foreach (var child in controlNode.Children)
      {
        AddChild(child);
      }
      foreach (var patern in controlNode.Paterns)
      {
        AddPatern(patern);
      }
     
      foreach (var property in controlNode.Properties)
      {
        DisplayedProperties.Add(new DisplayedPropertiesNode(property));
        Properties.Add(property);
      }
    }
    public string DisplayedName
    {
      get;
      set;
    }
    public string DisplayedId
    {
      get;
      set;
    }
    public string DisplayedType
    {
      get;
      set;
    }
    public List<string> DisplayedPath
    {
      get;
      set;
    }
    public List<DisplayedPropertiesNode> DisplayedProperties
    {
      get;
      private set;
    }
    public string DisplayedPathToString()
    {
      return DisplayedPath.Aggregate("", (current, node) => current + ("/" + node));
    }

  }

  public class DisplayedPropertiesNode : PropertiesNode
  {
    public string DisplayedName
    {
      get;
      set;
    }
    public string DisplayedValue
    {
      get;
      set;
    }

    public DisplayedPropertiesNode(PropertiesNode propertiesNode) : base(propertiesNode.Name, propertiesNode.Value, propertiesNode.IsValueSequence)
    {
      DisplayedName = Name;
      DisplayedValue = Value; 
    }
  }
    public class NodeTreeViewItem : TreeViewItem
    {
#region public
        public DisplayedControlNode CurrentControl { get; set; }

        public string TechPath { get; set; }

        public HashSet<string> ChangedProperties { get; set; }
        public NodeTreeViewItem()
            : base()
        {
            ChangedProperties = new HashSet<string>();
            ContextMenu = SnapshotContextMenu.GetMenu();
            MouseDown += OnMouseDown;
        }

        public void ExpandTree()
        {
            NodeTreeViewItem parent = Parent as NodeTreeViewItem;

            if (parent == null)
                return;

            parent.IsExpanded = true;
            parent.ExpandTree();
        }
#endregion
#region private
        private void OnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            MainWindow.SelectedSnapshotControl = this;
            mouseButtonEventArgs.Handled = true;
        }
#endregion
    }
 
}