using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinReturn : Word
{
    public BuiltinReturn() : base(BuiltinWords.Return)
    {
    }

    public BuiltinReturn(IToken token) : base(token)
    {
    }

}
