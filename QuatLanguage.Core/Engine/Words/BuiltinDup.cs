using QuatLanguage.Core.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinDup : QuatWord
{
    public BuiltinDup() : base(BuiltinWords.Dup)
    {
    }

    public BuiltinDup(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        var a = context.PopVStack();
        context.PushVStack(a);
        context.PushVStack(a);
    }

}
