using QuatLanguage.Core.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinPrep : QuatWord
{
    public BuiltinPrep() : base(BuiltinWords.Prep)
    {
    }

    public BuiltinPrep(IToken token) : base(token)
    {
    }

}
