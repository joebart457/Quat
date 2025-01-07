using QuatLanguage.Core.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinOver : QuatWord
{
    public BuiltinOver() : base(BuiltinWords.Over)
    {
    }

    public BuiltinOver(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        var b = context.PopVStack();
        var a = context.PopVStack();
        context.PushVStack(a);
        context.PushVStack(b);
        context.PushVStack(a);
    }

}
