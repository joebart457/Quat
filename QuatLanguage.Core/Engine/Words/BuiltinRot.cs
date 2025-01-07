using QuatLanguage.Core.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinRot : QuatWord
{
    public BuiltinRot() : base(BuiltinWords.Rot)
    {
    }

    public BuiltinRot(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        var a = context.PopVStack();
        var b = context.PopVStack();
        var c = context.PopVStack();
        context.PushVStack(a);
        context.PushVStack(c);
        context.PushVStack(b);
    }

}
