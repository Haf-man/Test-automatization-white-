using System.Windows.Controls;
using AutomationTree;

namespace AppWindow
{
    public class NodeTreeViewItem : TreeViewItem
    {
        public NodeTreeViewItem()
            : base()
        {
            //It's needed for right work
            Items.Add("*");
        }

        public ControlNode CurrentControl { get; set; }
    }
}