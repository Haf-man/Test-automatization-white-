using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation;
using AutomationTree;

namespace TreeGenerator
{
  public static class SnapshotGenerator
  {
    #region public

    public enum PropertyTypes
    {
      String,
      Bool,
      Int,
      Object
    };

    /// <summary>
    /// Generate UT tree by given process ID
    /// </summary>
    public static ControlNode GenerateTree(int processId)
    {
      var rootElement = FindElementByProcessId(processId);

      if (rootElement == null)
      {
        throw new ArgumentException("Can't find process with such processId");
      }

      return BuildTree(rootElement);
    }

    /// <summary>
    /// Generate UT tree by given AutomationElement
    /// </summary>
    public static ControlNode GenerateTree(AutomationElement rootElement)
    {
      return BuildTree(rootElement);
    }

    /// <summary>
    /// Generate UT tree by given AutomationElement name
    /// </summary>
    public static ControlNode GenerateTree(string elementName)
    {
      var rootElement = FindlElementByStringProperty(AutomationElement.NameProperty, elementName);
      if (rootElement == null)
      {
        throw new ArgumentException("Can't find element with such name");
      }

      return BuildTree(rootElement);
    }

    /// <summary>
    /// Generate UT tree by given AutomationId in tree with root parentAutomationId
    /// </summary>
    /// <param name="parentAutomationId">AutomationId that provides the main tree root</param>
    /// <param name="childAutomationId">AutomationId that must bi in the main tree</param>
    public static ControlNode GenerateTreeByAutomationId(string parentAutomationId, string childAutomationId = null)
    {
      AutomationElement rootElement = FindlElementByStringProperty(AutomationElement.AutomationIdProperty,
        parentAutomationId);
      if (rootElement == null)
        throw new Exception(String.Format("Can't find element with such automationId '{0}'", parentAutomationId));
      if (childAutomationId == null)
        return BuildTree(rootElement);

      rootElement = FindElementByAutomationId(rootElement, childAutomationId);
      if (rootElement == null)
        throw new Exception(String.Format("Can't find element with such automationId  '{0}'", childAutomationId));

      return BuildTree(rootElement);
    }

    /// <summary>
    /// Returns all usable properties as list of strings
    /// </summary>
    public static List<string> AllInterestingProperties
    {
      get
      {
        if (_allProperties == null)
        {
          List<string> propertiesList =
            InterestingControlProperties.Select(
              interestingControlProperty => interestingControlProperty.ProgrammaticName).ToList();
          propertiesList.AddRange(
            SpecalInterestingControlProperties.Select(
              interestingControlProperty => interestingControlProperty.ProgrammaticName));
          propertiesList.AddRange(
            AtributesControlProperties.Select(atributesControlProperteis => atributesControlProperteis.ProgrammaticName));

          propertiesList.Sort();
          _allProperties = propertiesList;
        }
        return _allProperties;
      }
    }

    /// <summary>
    /// Returns the character that separates values in propertie
    /// </summary>
    public static string SeparatingString
    {
      get { return _separatingString; }
    }

    #endregion

    #region private

    #region buildTree

    /// <summary>
    /// Contains ControlTypes that we don't want to see in the config
    /// </summary>
    private static readonly HashSet<string> ExcludedControlTypes = new HashSet<string>
    {
      "ControlType.Separator",
      "ControlType.Thumb",
      "ControlType.TitleBar"
    };

    private static readonly HashSet<string> UnexpandableTypes = new HashSet<string>
    {
      "ControlType.TreeItem"
    };

    /// <summary>
    /// Contains properties that we want to see in the config
    /// </summary>
    private static readonly List<AutomationProperty> InterestingControlProperties = new List<AutomationProperty>
    {
      AutomationElement.IsEnabledProperty,
      AutomationElement.ClassNameProperty,
      AutomationElement.IsOffscreenProperty,
      ValuePatternIdentifiers.ValueProperty,
      SelectionItemPatternIdentifiers.IsSelectedProperty
    };

    /// <summary>
    /// Contains properties that requires special processing 
    /// </summary>
    private static readonly List<AutomationProperty> SpecalInterestingControlProperties = new List
      <AutomationProperty>
    {
      TableItemPatternIdentifiers.ColumnHeaderItemsProperty,
      TableItemPatternIdentifiers.RowHeaderItemsProperty,
      SelectionPatternIdentifiers.SelectionProperty,
    };

    /// <summary>
    /// Contains properties that we can use in config but adding them to snapshot as main atributes
    /// </summary>
    private static readonly List<AutomationProperty> AtributesControlProperties = new List<AutomationProperty>()
    {
      AutomationElement.NameProperty
    };

    private static List<string> _allProperties;
    private static readonly string _separatingString = ", ";

    private static ControlNode BuildTree(AutomationElement rootElement)
    {
      string controlType = GetElementControlType(rootElement);

      var rootNode = new ControlNode(GetElementName(rootElement),
        GetElementAutomationId(rootElement),
        controlType,
        new List<string>());

      AddProperties(rootElement, rootNode);
      AddPatterns(rootElement, rootNode);

      if (!UnexpandableTypes.Contains(controlType))
        UsePatterns(rootElement);

      FindChilds(rootElement, rootNode);

      return rootNode;
    }

    private static void FindChilds(AutomationElement automationElement, ControlNode node)
    {
      foreach (AutomationElement childElement in GetElementChilds(automationElement))
      {
        string controlType = GetElementControlType(childElement);

        if (ExcludedControlTypes.Contains(controlType))
          continue;

        var childNode = new ControlNode(node, GetElementName(childElement), GetElementAutomationId(childElement),
          controlType);

        AddProperties(childElement, childNode);
        AddPatterns(childElement, childNode);
        node.AddChild(childNode);

        if (!UnexpandableTypes.Contains(controlType))
          UsePatterns(childElement);

        FindChilds(childElement, childNode);
      }
    }

    private static void AddPatterns(AutomationElement automationElement, ControlNode node)
    {
      foreach (var automationPattern in automationElement.GetSupportedPatterns())
      {
        PatternNode patternNode = new PatternNode(automationPattern.ProgrammaticName);

        node.AddPatern(patternNode);
      }
    }

    /// <summary>
    /// Adds the properties from Interesting and Special list of properties that are supported by given element
    /// </summary>
    private static void AddProperties(AutomationElement automationElement, ControlNode node)
    {
      var supportedProperties = automationElement.GetSupportedProperties();

      foreach (var interestingControlProperty in InterestingControlProperties)
      {
        if (supportedProperties.Contains(interestingControlProperty))
        {
          string value =
            (automationElement.GetCurrentPropertyValue(interestingControlProperty) ?? "").ToString();
          node.AddProperty(new PropertiesNode(interestingControlProperty.ProgrammaticName, value));
        }
      }
      foreach (var property in SpecalInterestingControlProperties)
      {
        if (supportedProperties.Contains(property))
        {
          if (Equals(property, TableItemPatternIdentifiers.ColumnHeaderItemsProperty) ||
              Equals(property, TableItemPatternIdentifiers.RowHeaderItemsProperty))
          {
            AutomationElement[] headerElements =
              automationElement.GetCurrentPropertyValue(property) as AutomationElement[];

            if (headerElements == null)
              return;

            node.AddProperty(new PropertiesNode(property.ProgrammaticName,
              GetElementsNamesAsString(headerElements), true));
          }
          else if (Equals(property, SelectionPatternIdentifiers.SelectionProperty))
          {
            AutomationElement[] selectedElements =
              automationElement.GetCurrentPropertyValue(property) as AutomationElement[];

            if (selectedElements == null)
              return;

            node.AddProperty(new PropertiesNode(property.ProgrammaticName,
              GetElementsNamesAsString(selectedElements), true));
          }
        }
      }
    }

    /// <summary>
    /// Returns names of elements as sequence (e.g.: |name1||name2|)
    /// </summary>
    private static string GetElementsNamesAsString(AutomationElement[] automationElements)
    {
      string nameSequence = String.Join(SeparatingString, automationElements.Select(GetElementName));

      return nameSequence;
    }

    /// <summary>
    /// Attempting to use expand & scroll on this element
    /// </summary>
    private static void UsePatterns(AutomationElement automationElement)
    {
      UseExpandeCollapsePattern(automationElement);
      UseScrollPattern(automationElement);
    }

    /// <summary>
    /// Attempting to use ExpandCollapse pattern on this element
    /// </summary>
    private static void UseExpandeCollapsePattern(AutomationElement automationElement)
    {
      try
      {
        ExpandCollapsePattern expandCollapsePattern =
          automationElement.GetCurrentPattern(ExpandCollapsePatternIdentifiers.Pattern) as
            ExpandCollapsePattern;
        if (expandCollapsePattern != null)
          expandCollapsePattern.Expand();
      }
      catch (InvalidOperationException)
      {
      }
    }

    /// <summary>
    /// Attempting to use scroll pattern on this element
    /// </summary>
    private static void UseScrollPattern(AutomationElement automationElement)
    {
      try
      {
        ScrollPattern scrollPattern =
          automationElement.GetCurrentPattern(ScrollPatternIdentifiers.Pattern) as ScrollPattern;
        if (scrollPattern != null)
        {
          double percent;
          do
          {
            scrollPattern.ScrollVertical(ScrollAmount.SmallIncrement);
            percent =
              (double)
                automationElement.GetCurrentPropertyValue(ScrollPattern.VerticalScrollPercentProperty);
          } while (percent < 100);
        }
      }
      catch (InvalidOperationException)
      {
      }
    }

    #endregion

    private static AutomationElement FindElementByProcessId(int processId)
    {
      AutomationElement root = AutomationElement.RootElement;

      var element = root.FindFirst(TreeScope.Descendants,
        new PropertyCondition(AutomationElement.ProcessIdProperty, processId));

      return element;
    }

    private static AutomationElement FindlElementByStringProperty(AutomationProperty property, string value)
    {
      AutomationElement root = AutomationElement.RootElement;

      var element = root.FindFirst(TreeScope.Descendants,
        new PropertyCondition(property, value));

      return element;
    }

    private static AutomationElement FindElementByAutomationId(AutomationElement element, string automationId)
    {
      AutomationElement childElement = element.FindFirst(TreeScope.Descendants,
        new PropertyCondition(AutomationElement.AutomationIdProperty, automationId));
      return childElement;
    }

    private static AutomationElementCollection GetElementChilds(AutomationElement element)
    {
      return element.FindAll(TreeScope.Children, Condition.TrueCondition);
    }

    private static string GetElementControlType(AutomationElement element)
    {
      return element.Current.ControlType.ProgrammaticName;
    }

    private static string GetElementName(this AutomationElement element)
    {
      return element.GetCurrentPropertyValue(AutomationElement.NameProperty) as string ?? "";
    }

    private static string GetElementAutomationId(AutomationElement element)
    {
      return element.GetCurrentPropertyValue(AutomationElement.AutomationIdProperty) as string ?? "";
    }

    #endregion
  }
}
