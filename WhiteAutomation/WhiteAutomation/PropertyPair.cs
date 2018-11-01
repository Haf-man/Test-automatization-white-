namespace UiTreeGenerator
{
  public class PropertyPair<T1, T2>
  {
    public PropertyPair()
    {
    }

    public PropertyPair(T1 property, T2 propertyType)
    {
      this.Property = property;
      this.PropertyType = propertyType;
    }

    public T1 Property { get; set; }

    public T2 PropertyType { get; set; }
  };
}