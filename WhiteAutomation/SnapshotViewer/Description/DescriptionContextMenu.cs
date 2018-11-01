using System.Windows.Controls;


namespace SnapshotViewer.Description
{
    public class DescriptionContextMenu
    {
        public static ContextMenu GetMenu()
        {
            ContextMenu menu = new ContextMenu();

            MenuItem item = new MenuItem {Header = "Add to included properties"};
            item.Click += Configuration.WorkWithConfigurationTree.HandleAddPropertyToInclude;
            menu.Items.Add(item);

            item = new MenuItem {Header = "Add to excluded properties"};
            item.Click += Configuration.WorkWithConfigurationTree.HandleAddPropertyToExclude;
            menu.Items.Add(item);

            item = new MenuItem{Header = "Add to selected configuration node"};
            item.Click += Configuration.WorkWithConfigurationTree.HandleAddPropertyToSelectedNode;
            menu.Items.Add(item);

            return menu;
        }
    }
}