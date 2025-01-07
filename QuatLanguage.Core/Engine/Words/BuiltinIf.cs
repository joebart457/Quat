using QuatLanguage.Core.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinIf : QuatWord
{
    public BuiltinIf() : base(BuiltinWords.If)
    {
    }

    public BuiltinIf(IToken token) : base(token)
    {
    }

}
