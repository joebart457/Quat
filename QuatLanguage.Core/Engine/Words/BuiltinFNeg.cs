using QuatLanguage.Core.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinFNeg : QuatWord
{
    public BuiltinFNeg() : base(BuiltinWords.Neg)
    {
    }

    public BuiltinFNeg(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        var a = context.PopFStack();
        context.PushFStack(-a);
    }

}
