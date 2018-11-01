using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Controls;
using ConfigurationReader;
using TreeGenerator;

namespace SnapshotViewer.Configuration
{
  public class PropertyTreeViewItem : TreeViewItem
  {
    #region public

    public ControlTreeViewItem ParentControl { get; private set; }

    public string InputText
    {
      get { return _textBox.Text; }
      set { _textBox.Text = value; }
    }

    public new string Name
    {
      get { return _nameBlock.Text; }
      set { _nameBlock.Text = value; }
    }

    public bool IsCompareScopeChangeEnabled
    {
      get { return _scopeBox.IsEnabled; }
      set { _scopeBox.IsEnabled = value; }
    }

    public CompareScopes Scope
    {
      get { return _scopeBox.Scope; }
      set { _scopeBox.Scope = value; }
    }

    public bool IsValueRegularExpression 
    {
      get { return _checkBox.IsChecked.GetValueOrDefault(); }
      set { _checkBox.IsChecked = value; }
    }

    public ControlPropertiesControlPropertie ToControlPropertiesControlPropertie()
    {
      ControlPropertiesControlPropertie propertie = new ControlPropertiesControlPropertie()
      {
        Name = GetPropertyFullName(Name),
        Value = InputText,
        CompareScope = Scope,
        IsValueRegExp = IsValueRegularExpression
      };
      return propertie;
    }

    private PropertyTreeViewItem()
    {
      CreateTreeViewItemTemplate();
    }

    public PropertyTreeViewItem(string name, string inputText,
      CompareScopes compareScope, ControlTreeViewItem parentControl) : this()
    {
      Name = name;
      InputText = inputText;
      Scope = compareScope;
      ParentControl = parentControl;

      _scopeBox.SelectionChanged += (sender, args) => { ParentControl.CheckForConflicts(Name); };
    }


    #endregion

    #region private

    private TextBox _textBox = null;
    private TextBlock _nameBlock = null;
    private ScopeCombobox _scopeBox = null;
    private CheckBox _checkBox = null;
    private static readonly string TipForCheckBox = "Check this if you use regular expression";
    private static readonly string TipForRegExpression = "URL with info about Regular Expression you can see in Help";

    private void CreateTreeViewItemTemplate()
    {
      StackPanel stack = new StackPanel
      {
        Orientation = Orientation.Horizontal,
        ContextMenu = ConfigurationContextMenu.GetMenuForControlProperty()
      };

      _nameBlock = new TextBlock {Margin = new Thickness(2), VerticalAlignment = VerticalAlignment.Center};
      stack.Children.Add(_nameBlock);

      _textBox = new TextBox
      {
        Margin = new Thickness(2),
        VerticalAlignment = VerticalAlignment.Center,
        MinWidth = 20,
        ToolTip = new ToolTip {Content = TipForRegExpression}
      };
      stack.Children.Add(_textBox);

      _checkBox = new CheckBox()
      {
        VerticalAlignment = VerticalAlignment.Center,
        ToolTip = new ToolTip() {Content = TipForCheckBox},
        IsChecked = false
      };
      stack.Children.Add(_checkBox);

      _scopeBox = new ScopeCombobox
      {
        Margin = new Thickness(2),
        VerticalAlignment = VerticalAlignment.Center,
      };
      stack.Children.Add(_scopeBox);

      stack.MouseRightButtonUp += MainWindow.HandleMouseRightButtonUp;

      Header = stack;
    }

    private static string GetPropertyFullName(string property)
    {
      List<string> properties = SnapshotGenerator.AllInterestingProperties;
      string fullName = properties.Find(x => x.Split('.').Last() == property);
      if (fullName == null)
        throw new Exception("Incorrect property name");

      return fullName;
    }

    #endregion

  }
}