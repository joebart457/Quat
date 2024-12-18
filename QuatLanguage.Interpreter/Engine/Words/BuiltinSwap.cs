using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinSwap : Word
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
