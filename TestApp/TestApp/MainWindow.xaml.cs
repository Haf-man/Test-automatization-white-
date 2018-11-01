using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Automation;
namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int AmountRows = 3;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BattonRandom_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<DataModel> randomTable = new ObservableCollection<DataModel>();
            
            Random randGenerater = new Random();

            for (int i = 0; i < AmountRows; ++i)
            {
                randomTable.Add(new DataModel
                {
                    IntData = randGenerater.Next(),
                    LogicData = Convert.ToBoolean(randGenerater.Next() % 2),
                    TextData = randGenerater.Next().ToString(),
                    TimeData = new DateTime(Math.Abs(randGenerater.Next()))
                });
                
               
            }
            
            DataGrid.ItemsSource = randomTable;
              
            
        }

       
        private void ButtonStaticFill_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<DataModel> staticTable = new ObservableCollection<DataModel>();
            for (int i = 0; i < AmountRows/3; i++)
            {
                staticTable.Add(new DataModel
                {
                    IntData = DateTime.Today.Day,
                    LogicData = Convert.ToBoolean(DateTime.Now.Minute%2),
                    TextData = DateTime.Today.Month.ToString(),
                    TimeData = DateTime.Now
                });
            }
            DataGrid_Static.ItemsSource = staticTable;
             
        }

    }
}
