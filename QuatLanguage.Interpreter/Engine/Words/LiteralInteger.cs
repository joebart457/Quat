using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class LiteralInteger : Word
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
