using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ConfigurationReader;

namespace SnapshotViewer.Configuration
{
  public class MainAtributeTreeViewItem : TreeViewItem
  {
    #region public

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

    public bool IsTextInputEnabled
    {
      get { return _textBox.IsEnabled; }
      set { _textBox.IsEnabled = value; }
    }

    public bool IsCompareScopeChangeEnabled
    {
      get { return _scopeBox.IsEnabled; }
      set { _scopeBox.IsEnabled = value; }
    }

    public object GetControlScope()
    {
      return _scopeBox.GetScopeWithBlankField();
    }

    public void SetControlScope(string value)
    {
      _scopeBox.SetScopeWithBlankField(value);
    }

    public MainAtributeTreeViewItem()
      : base()
    {
      CreateTreeViewItemTemplate();
    }

    public MainAtributeTreeViewItem(string name, string inputText = "")
      : this()
    {
      Name = name;
      InputText = inputText;
    }

    public static MainAtributeTreeViewItem GetMainItemWithScope(string name, string scope)
    {
      MainAtributeTreeViewItem item = new MainAtributeTreeViewItem(name);

      item._textBox.Visibility = Visibility.Collapsed;
      item._scopeBox.AddBlankField();
      item._scopeBox.Visibility = Visibility.Visible;
      item.SetControlScope(scope);

      return item;
    }

    #endregion

    #region private

    private TextBox _textBox = null;
    private TextBlock _nameBlock = null;
    private ScopeCombobox _scopeBox = null;

    private void CreateTreeViewItemTemplate()
    {
      StackPanel stack = new StackPanel
      {
        Orientation = Orientation.Horizontal,
      };
      
      _nameBlock = new TextBlock {Margin = new Thickness(2), VerticalAlignment = VerticalAlignment.Center};
      stack.Children.Add(_nameBlock);

      _textBox = new TextBox
      {
        Margin = new Thickness(2),
        VerticalAlignment = VerticalAlignment.Center,
        MinWidth = 20,
        IsEnabled = false
      };
      stack.Children.Add(_textBox);

      _scopeBox = new ScopeCombobox
      {
        Margin = new Thickness(2),
        VerticalAlignment = VerticalAlignment.Center,
        Visibility = Visibility.Collapsed
      };
      stack.Children.Add(_scopeBox);

      stack.MouseRightButtonUp += MainWindow.HandleMouseRightButtonUp;

      Header = stack;
    }

    #endregion

  }
}