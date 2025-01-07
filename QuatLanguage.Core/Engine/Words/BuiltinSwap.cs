using QuatLanguage.Core.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinSwap : QuatWord
{
    public BuiltinSwap() : base(BuiltinWords.Swap)
    {
    }

    public BuiltinSwap(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        var b = context.PopVStack();
        var a = context.PopVStack();
        context.PushVStack(b);
        context.PushVStack(a);
    }
}
