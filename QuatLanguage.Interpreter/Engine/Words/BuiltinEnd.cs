using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinEnd : Word
{
    public BuiltinEnd() : base(BuiltinWords.End)
    {
    }

    public BuiltinEnd(IToken token) : base(token)
    {
    }

}
