using QuatLanguage.Core.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinGo : QuatWord
{
    public BuiltinGo() : base(BuiltinWords.Go)
    {
    }

    public BuiltinGo(IToken token) : base(token)
    {
    }

}
