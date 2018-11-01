using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TreeGenerator;

namespace SnapshotViewer.Configuration
{
    public static class ConfigurationContextMenu
    {
#region public

        public static ContextMenu GetMenuForControl(bool isHaveLinkToSnapshot = false)
        {
            ContextMenu menu = new ContextMenu();

            MenuItem item = new MenuItem { Header = "Delete control" };
            item.Click += Configuration.WorkWithConfigurationTree.HandleDeleteControl;
            menu.Items.Add(item);

            item = new MenuItem() {Header = "Go to snapshot node..."};
            item.Click += Configuration.WorkWithConfigurationTree.HandleGoToSnapshotNode;
            menu.Items.Add(item);

            item = new MenuItem
            {
                Header = "Add property from a list...",
                ItemsSource = GetPropertyMenu(WorkWithConfigurationTree.HandleAddPropertyFromListToControl)
            };
            
            menu.Items.Add(item);

            return menu;
        }
        
        public static ContextMenu GetMenuForControlProperty()
        {
            ContextMenu menu = new ContextMenu();

            MenuItem item = new MenuItem { Header = "Delete property" };
            item.Click += Configuration.WorkWithConfigurationTree.HandleDeleteControlProperty;
            menu.Items.Add(item);

            return menu;            
        }

        public static ContextMenu GetMenuForGlobalProperty()
        {
            ContextMenu menu = new ContextMenu();

            MenuItem item = new MenuItem { Header = "Delete property" };
            item.Click += Configuration.WorkWithConfigurationTree.HandleDeleteGlobalPropery;
            menu.Items.Add(item);

            return menu;
        }

        public static ContextMenu GetMenuForGlobalPropertiesHandler()
        {
            ContextMenu menu = new ContextMenu();

            var item = new MenuItem
            {
                Header = "Add property from a list...",
                ItemsSource = GetPropertyMenu(WorkWithConfigurationTree.HandleAddPropertyFromListToGlobal)
            };

            menu.Items.Add(item);

            return menu;
        }
#endregion
#region private
        private static readonly List<string> AllProperties =
            SnapshotGenerator.AllInterestingProperties.Select(propertie => propertie.Split('.').Last()).ToList();

        private static ObservableCollection<MenuItem> GetPropertyMenu(RoutedEventHandler clickHandler)
        {
            ObservableCollection<MenuItem> collection = new ObservableCollection<MenuItem>();

            foreach (var property in AllProperties)
            {
                MenuItem item = new MenuItem { Header = property };
                item.Click += clickHandler;
                collection.Add(item);
            }

            return collection;
        }
#endregion
    }
}