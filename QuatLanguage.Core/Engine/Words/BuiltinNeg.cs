using QuatLanguage.Core.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinNeg : QuatWord
{
    public BuiltinNeg() : base(BuiltinWords.Neg)
    {
    }

    public BuiltinNeg(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        var a = context.PopVStack();
        context.PushVStack(-a);
    }

}
