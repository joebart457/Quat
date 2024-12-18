using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinGo : Word
{
    public BuiltinGo() : base(BuiltinWords.Go)
    {
    }

    public BuiltinGo(IToken token) : base(token)
    {
    }

}
