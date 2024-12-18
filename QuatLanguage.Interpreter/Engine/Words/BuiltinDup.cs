using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinDup : Word
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
