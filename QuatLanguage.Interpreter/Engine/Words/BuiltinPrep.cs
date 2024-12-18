using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinPrep : Word
{
    public BuiltinPrep() : base(BuiltinWords.Prep)
    {
    }

    public BuiltinPrep(IToken token) : base(token)
    {
    }

}
