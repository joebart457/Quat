using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class LiteralInteger : QuatWord
{
    public nint Value { get; set; }

    public LiteralInteger(nint value) : base("")
    {
        Value = value;
    }

    public LiteralInteger(IToken token, nint value) : base(token)
    {
        Value = value;
    }

    public override void Evaluate(QuatContext context)
    {
        context.PushVStack(Value);
    }
}
