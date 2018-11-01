using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStack.White;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.WindowItems;

namespace CalculatorTests
{
  [TestClass]
  public class CalculatorTest
  {
    [TestMethod]
    public void TestSummation()
    {
      Application application = Application.Launch(@"C:\Windows\system32\calc.exe");
      try
      {
        Window window = application.GetWindow("Калькулятор");

        var inputField = window.Get(SearchCriteria.ByAutomationId("150"));
        inputField.Enter("125");

        var sumButton = window.Get(SearchCriteria.ByAutomationId("93"));
        sumButton.Click();

        inputField.Enter("131");

        var evalButton = window.Get(SearchCriteria.ByAutomationId("121"));
        evalButton.Click();

        ControlPoint.ControlPoint.PerformAction(@".\..\..\..\Snapshots\checkSummation",
          ControlPoint.ControlPoint.ActionTypes.CheckTree,
          window);
      }
      finally
      {
        application.Close();
      }
    }

    [TestMethod]
    public void TestMultiplication()
    {
      Application application = Application.Launch(@"C:\Windows\system32\calc.exe");
      Window window = application.GetWindow("Калькулятор");

      var inputField = window.Get(SearchCriteria.ByAutomationId("150"));
      inputField.Enter("1024");

      var multButton = window.Get(SearchCriteria.ByAutomationId("92"));
      multButton.Click();
      inputField.Enter("1024");

      var evalButton = window.Get(SearchCriteria.ByAutomationId("121"));
      evalButton.Click();

      inputField.Enter("100");
      multButton.Click();
      evalButton.Click();

      try
      {
        ControlPoint.ControlPoint.PerformAction(@".\..\..\..\Snapshots\checkMultiplication",
          ControlPoint.ControlPoint.ActionTypes.WriteTree,
          window);
      }
      finally
      {
        application.Close();
      }
    }

    [TestMethod]
    public void TestInputFeild()
    {
      Application application = Application.Launch(@"C:\Windows\system32\calc.exe");
      Window window = application.GetWindow("Калькулятор");

      //var titleBar = window.Get(SearchCriteria.ByAutomationId("TitleBar"));
      ControlPoint.ControlPoint.PerformAction("Calc", ControlPoint.ControlPoint.ActionTypes.WriteTree, window);
      var inputField = window.Get(SearchCriteria.ByAutomationId("150"));
      inputField.Enter("125");

      var sumButton = window.Get(SearchCriteria.ByAutomationId("93"));
      sumButton.Click();

      inputField.Enter("131");
      ControlPoint.ControlPoint.PerformAction("checkFieldChange", ControlPoint.ControlPoint.ActionTypes.WriteTree,
        inputField);

      var evalButton = window.Get(SearchCriteria.ByAutomationId("121"));
      evalButton.Click();

      ControlPoint.ControlPoint.PerformAction("checkFieldChange", ControlPoint.ControlPoint.ActionTypes.CheckTree,
        inputField);

      application.Close();
    }
  }
}
