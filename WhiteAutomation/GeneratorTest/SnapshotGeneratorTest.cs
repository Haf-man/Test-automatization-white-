using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeGenerator;

namespace GeneratorTest
{
    [TestClass]
    public class SnapshotGeneratorTest
    {
        [TestMethod]
        public void TestGenerateTreeByAutomationId()
        {
            AutomationTree.ControlNode tree =
                TreeGenerator.SnapshotGenerator.GenerateTreeByAutomationId("VisualStudioMainWindow");
        }

        [TestMethod]
        public void TestGenerateTreeByProcessId()
        {
            AutomationElement automationElement = AutomationElement.RootElement;
            automationElement = automationElement.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "TestAppMainWindow"));

            AutomationTree.ControlNode tree = TreeGenerator.SnapshotGenerator.GenerateTree(
                (int)automationElement.GetCurrentPropertyValue(AutomationElement.ProcessIdProperty));
        }

        [TestMethod]
        public void TestGenerateTreeByName()
        {
            AutomationTree.ControlNode tree =
                TreeGenerator.SnapshotGenerator.GenerateTree("WhiteAutomation - Microsoft Visual Studio");
        }

        [TestMethod]
        public void TestGenerateTreeWithUITree()
        {
            AutomationTree.ControlNode tree =
                TreeGenerator.SnapshotGenerator.GenerateTreeByAutomationId("AutomationNodesTree");
        }
    }
}
