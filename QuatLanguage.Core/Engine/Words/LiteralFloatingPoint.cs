using System.Runtime.InteropServices;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class LiteralFloatingPoint : QuatWord
{
    public NFloat Value { get; set; }

    public LiteralFloatingPoint(NFloat value) : base("")
    {
        Value = value;
    }

    public LiteralFloatingPoint(IToken token, NFloat value) : base(token)
    {
        Value = value;
    }

    public override void Evaluate(QuatContext context)
    {
        context.PushFStack(Value);
    }
}
