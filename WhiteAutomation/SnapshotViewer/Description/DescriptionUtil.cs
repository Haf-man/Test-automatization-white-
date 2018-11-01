using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SnapshotViewer.Description
{
    public static class DescriptionUtil
    {
        public static string GetFormattedNameFromLabel(Label label)
        {
            string name = label.ToString().Split(' ').Last().Replace(":", String.Empty).Replace("\"", String.Empty);
            
            return name;
        }

        public static string GetValueForCurrentProperty(Label property, Grid currentDescriptionGrid)
        {
            int row = Grid.GetRow(property);
            Label valueLabel =
                MainWindow.CurrentDescriptionGrid.Children.Cast<UIElement>()
                    .First(
                        element =>
                            Grid.GetRow(element) == row && Grid.GetColumn(element) == WorkWithGrid.PropertyValueColumn)
                    as Label;

            return GetFormattedNameFromLabel(valueLabel);
        }
    }
}