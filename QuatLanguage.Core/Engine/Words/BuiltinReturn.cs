using QuatLanguage.Core.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinReturn : QuatWord
{
    public BuiltinReturn() : base(BuiltinWords.Return)
    {
    }

    public BuiltinReturn(IToken token) : base(token)
    {
    }

}
