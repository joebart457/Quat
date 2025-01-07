using QuatLanguage.Core.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinStay : QuatWord
{
    public BuiltinStay() : base(BuiltinWords.Stay)
    {
    }

    public BuiltinStay(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        context.PopAddressStack();
    }

}
