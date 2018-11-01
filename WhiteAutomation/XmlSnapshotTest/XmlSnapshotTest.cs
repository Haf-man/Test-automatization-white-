 
using Microsoft.VisualStudio.TestTools.UnitTesting;
 

namespace XmlSnapshotTest
{
    [TestClass]
    public class XmlSnapshotTest
    {
        [TestMethod]
        public void TestGenerateXmlByProcessId()
        {
            //AutomationTree.ControlNode tree =
             //   TreeGenerator.SnapshotGenerator.GenerateTreeByAutomationId("VisualStudioMainWindow");
            AutomationTree.ControlNode tree = TreeGenerator.SnapshotGenerator.GenerateTree(2996);
            XmlSnapshot.XmlSnapshot.WriteToXmlFile(tree, @"D:\file.xml");
           
        }
        [TestMethod]
        public void TestGenerateTreeFromXml()
        {
            AutomationTree.ControlNode tree = TreeGenerator.SnapshotGenerator.GenerateTree(3488);
            XmlSnapshot.XmlSnapshot.WriteToXmlFile(tree, @"D:\file.xml");
            AutomationTree.ControlNode tree1 = XmlSnapshot.XmlSnapshot.ReadFromXmlFile(@"D:\file.xml");
            XmlSnapshot.XmlSnapshot.WriteToXmlFile(tree1, @"D:\file1.xml");
        }
    }
}
