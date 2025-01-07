using QuatLanguage.Core.CustomAttributes;
using System.Reflection;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;


public class BuiltinLoadLibrary : QuatWord
{
    public BuiltinLoadLibrary(IToken token) : base(token)
    {
    }

    public BuiltinLoadLibrary(string name) : base(name)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        var libraryName = context.MarshalStringFromVStack();
        if (libraryName == null) throw new($"unable to load library from nullptr");
        var assembly = Assembly.LoadFrom(libraryName);
        var importedWords = assembly
            .GetExportedTypes()
            .Where(x => x.GetCustomAttribute<WordAttribute>() != null)
            .ToList();

        foreach(var importedWord in importedWords)
        {
            var attributeData = importedWord.GetCustomAttribute<WordAttribute>();
            var instance = (QuatWord?)Activator.CreateInstance(importedWord, attributeData!.Name);
            if (instance == null) continue;
            context.Grammars[importedWord.Name] = new Grammar(importedWord.Name, [ instance ]);
        }

    }
}
