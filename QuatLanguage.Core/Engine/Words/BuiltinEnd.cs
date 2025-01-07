using QuatLanguage.Core.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinEnd : QuatWord
{
    public BuiltinEnd() : base(BuiltinWords.End)
    {
    }

    public BuiltinEnd(IToken token) : base(token)
    {
    }

}
