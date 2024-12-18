using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinRot : Word
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
