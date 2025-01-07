using QuatLanguage.Core.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinDebug : QuatWord
{
    public BuiltinDebug() : base(BuiltinWords.Debug)
    {
    }

    public BuiltinDebug(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        context.DoDebugBreak(this);
    }

}
