using System.Windows;
using System.Windows.Controls;

namespace AppWindow
{
    public class InputTreeViewItem : TreeViewItem
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

        public InputTreeViewItem()
            : base()
        {
            CreateTreeViewItemTemplate();
        }

        public InputTreeViewItem(string name, string inputText) : base()
        {
            CreateTreeViewItemTemplate();
            Name = name;
            InputText = inputText;
        }
#endregion
#region private
        private TextBox _textBox = null;
        private TextBlock _nameBlock = null;

        private void CreateTreeViewItemTemplate()
        {
            StackPanel stack = new StackPanel {Orientation = Orientation.Horizontal};

            _nameBlock = new TextBlock{ Margin = new Thickness(2), VerticalAlignment = VerticalAlignment.Center};
            stack.Children.Add(_nameBlock);

            _textBox = new TextBox {Margin = new Thickness(2), VerticalAlignment = VerticalAlignment.Center};
            stack.Children.Add(_textBox);

            Header = stack;
        }

#endregion
    }
}