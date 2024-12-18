using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinElse : Word
{
    public BuiltinElse() : base(BuiltinWords.Else)
    {
    }

    public BuiltinElse(IToken token) : base(token)
    {
    }
}
