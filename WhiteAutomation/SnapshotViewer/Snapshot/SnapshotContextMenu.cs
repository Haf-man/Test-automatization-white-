using System.Windows.Controls;
using SnapshotViewer.Configuration;

namespace SnapshotViewer.Snapshot
{
    public static class SnapshotContextMenu
    {
        public static ContextMenu GetMenu()
        {
            ContextMenu contextMenu = new ContextMenu();

            MenuItem menuItem = new MenuItem {Header = "Add control to included"};
            menuItem.Click += WorkWithConfigurationTree.HandleAddControlToIncluded;
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem { Header = "Add control to excluded" };
            menuItem.Click += WorkWithConfigurationTree.HandleAddControlToExcluded;
            contextMenu.Items.Add(menuItem);

            return contextMenu;
        }

        public enum CongigurationHeader : int
        {
            IncludedControls = 0,
            ExcludedControls,
            IncludedProperty,
            ExcludedProperty
        }
    }
}