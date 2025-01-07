using QuatLanguage.Core.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinElse : QuatWord
{
    public BuiltinElse() : base(BuiltinWords.Else)
    {
    }

    public BuiltinElse(IToken token) : base(token)
    {
    }
}
