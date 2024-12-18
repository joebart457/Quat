using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinOver : Word
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
