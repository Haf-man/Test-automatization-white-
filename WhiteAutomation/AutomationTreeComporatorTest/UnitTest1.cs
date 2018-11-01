using System.IO;
using System.Windows.Automation;
using ConfigurationReader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStack.White;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TableItems;
using TestStack.White.UIItems.WindowItems;

namespace AutomationTreeComporatorTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
          string[] configFiles = {"NameIncluded", "NameExcluded", "LineIncluded", "ExcludeControlName", "ExcludeControlChilds" };
           
            AutomationTree.ControlNode tree1 = XmlSnapshot.XmlSnapshot.ReadFromXmlFile(@"D:\file.xml");
            AutomationTree.ControlNode tree2 = XmlSnapshot.XmlSnapshot.ReadFromXmlFile(@"D:\file1.xml");
            //AutomationTree.ControlNode tree2 = TreeGenerator.SnapshotGenerator.GenerateTree(7624);
            //Configuration configuration = ConfigurationReaderWriter.ReadConfiguration(@"D:\NameTwoProperties.config.xml");
            // Configuration configuration = new Configuration();
          foreach (var configFileName in configFiles)
          {
            Configuration configuration = ConfigurationReaderWriter.ReadConfiguration(@"D:\" + configFileName + ".config.xml");
            var result = AutomationTreeComparator.Comparator.CompareTrees(tree1, tree2, configuration);
            AutomationTreeComparator.Comparator.ResultsToFile(result, @"D:\" + configFileName + ".out.xml"); 
          }     
             
        }

        [TestMethod]
        public void PreTestMethod1()
        {
            //int id = 7392;
            string id = "TestAppMainWindow";
            //string id = "VisualStudioMainWindow";
           // int id = 6728;
            AutomationTree.ControlNode tree1 = TreeGenerator.SnapshotGenerator.GenerateTreeByAutomationId(id);
            XmlSnapshot.XmlSnapshot.WriteToXmlFile(tree1, @"D:\file.xml");
            AutomationTree.ControlNode tree2 = TreeGenerator.SnapshotGenerator.GenerateTreeByAutomationId(id);
            XmlSnapshot.XmlSnapshot.WriteToXmlFile(tree2, @"D:\file1.xml");
  
        }

        [TestMethod]
        public void TestWithWhite()
        {
           
            
            string applicationPaths = @".\..\..\..\..\Tools\";

            var applicationPath = Path.Combine(applicationPaths, "TestApp.exe");
            Application application = Application.Launch(applicationPath);
      try
      {
            Window window = application.GetWindow("MainWindow", InitializeOption.NoCache);
            Button button = window.Get<Button>(SearchCriteria.ByAutomationId("BattonRandom"));
            button.Click();
            window.WaitWhileBusy();

        Table grid = window.Get<Table>(SearchCriteria.ByAutomationId("DataGrid"));
        grid.Rows[2].Select();
        grid.Rows[5].Select();
        grid.Rows[7].Select();

        ControlPoint.ControlPoint.PerformAction(@".\..\..\..\Snapshots\testAppCheckMultipleSelect",
          ControlPoint.ControlPoint.ActionTypes.WriteTree,
                    window);
            }
            finally
            {
                application.Close();
            }
        }
    }
}
