using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinIf : Word
{
    public BuiltinIf() : base(BuiltinWords.If)
    {
    }

    public BuiltinIf(IToken token) : base(token)
    {
    }

}
