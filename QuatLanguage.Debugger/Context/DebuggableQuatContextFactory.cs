using ParserLite.Exceptions;
using ParserLite;
using QuatLanguage.Interpreter.Engine;
using QuatLanguage.Interpreter.Parser;
using TokenizerCore.Model;
using QuatLanguage.Interpreter.Memory;
using QuatLanguage.Interpreter.Engine.Words;
using QuatLanguage.Interpreter.Factories;

namespace QuatLanguage.Debugger.Context;

public class DebuggableQuatContextFactory: QuatContextFactory
{

    public static new QuatContextFactory CreateNew() => new DebuggableQuatContextFactory();
    public override QuatContext CreateContext(string filePath, out List<ParsingException> errors)
    {
        var memoryManager = DecideMemoryManager();
        var parser = EmitParser(false, memoryManager);
        var grammars = parser.ParseFile(filePath, out errors);
        return new DebuggableContext(memoryManager, grammars, filePath);
    }

}