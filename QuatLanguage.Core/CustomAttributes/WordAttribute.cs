
namespace QuatLanguage.Core.CustomAttributes;

[AttributeUsage(AttributeTargets.Class)]
public class WordAttribute: Attribute
{
    public string Name { get; set; }

    public WordAttribute(string name)
    {
        Name = name;
    }
}