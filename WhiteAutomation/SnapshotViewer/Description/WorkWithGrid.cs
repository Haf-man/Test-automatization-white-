using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TreeGenerator;

namespace SnapshotViewer.Description
{
  public static class WorkWithGrid
  {
    #region public

    public static readonly int HeaderColumn = 0;
    public static readonly int PropertyNameColumn = 1;
    public static readonly int PropertyValueColumn = 2;

    private static readonly Brush DiffNodeColor = Brushes.Coral;

    public static void AddHeader(string headerName, Grid grid)
    {
      Label header = new Label {Content = headerName + ":"};

      grid.RowDefinitions.Add(new RowDefinition());
      grid.Children.Add(header);

      header.SetValue(Grid.ColumnProperty, HeaderColumn);
      header.SetValue(Grid.RowProperty, grid.RowDefinitions.Count - 1);
    }

    public static void AddProperty(string propertyName, string propertyValue, Grid grid, bool isCanAddedToConfig = false)
    {
      grid.RowDefinitions.Add(new RowDefinition());
      int currentRow = grid.RowDefinitions.Count - 1;

      Label nameLabel = new Label {Content = propertyName + ":"};
      if (isCanAddedToConfig)
      {
        nameLabel.ContextMenu = DescriptionContextMenu.GetMenu();
      }
      nameLabel.MouseRightButtonUp += MainWindow.HandleMouseRightButtonUp;
      AddItemToGrid(nameLabel, PropertyNameColumn, currentRow, grid);

      Label valueLabel = new Label {Content = "\"" + propertyValue + "\""};

      AddItemToGrid(valueLabel, PropertyValueColumn, currentRow, grid);
    }

    public static void AddProperty(string propertyName, string propertyValue, bool isChanged, Grid grid,
      bool isCanAddedToConfig = false)
    {
      grid.RowDefinitions.Add(new RowDefinition());
      int currentRow = grid.RowDefinitions.Count - 1;

      Label nameLabel = new Label {Content = propertyName + ":"};
      if (isCanAddedToConfig)
      {
        nameLabel.ContextMenu = DescriptionContextMenu.GetMenu();
      }
      nameLabel.MouseRightButtonUp += MainWindow.HandleMouseRightButtonUp;
      AddItemToGrid(nameLabel, PropertyNameColumn, currentRow, grid);

      Label valueLabel = new Label {Content = "\"" + propertyValue + "\""};

      if (isChanged)
        valueLabel.Background = DiffNodeColor;

      AddItemToGrid(valueLabel, PropertyValueColumn, currentRow, grid);
    }

    public static void AddProperty(string propertyName, string propertyValue, Grid grid, bool isCanAddedToConfig,
      bool isValueSequence)
    {
      AddProperty(propertyName, propertyValue, grid, isCanAddedToConfig);
    }

    public static void AddProperty(string propertyName, string propertyValue, bool isChanged, Grid grid,
      bool isCanAddedToConfig, bool isValueSequence)
    {
      AddProperty(propertyName, propertyValue, isChanged, grid, isCanAddedToConfig);
    }

    #endregion

    #region private

    private static void AddItemToGrid(UIElement item, int col, int row, Grid grid)
    {
      item.SetValue(Grid.ColumnProperty, col);
      item.SetValue(Grid.RowProperty, row);

      grid.Children.Add(item);
    }

    #endregion
  }
}