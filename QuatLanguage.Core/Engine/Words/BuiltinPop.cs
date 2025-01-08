using QuatLanguage.Core.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinPop : QuatWord
{
    public BuiltinPop() : base(BuiltinWords.Pop)
    {
    }

    public BuiltinPop(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        _ = context.PopVStack();
    }
}
