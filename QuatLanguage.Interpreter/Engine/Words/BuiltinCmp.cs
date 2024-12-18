using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinCmp : Word
{
    public BuiltinCmp() : base(BuiltinWords.Cmp)
    {
    }

    public BuiltinCmp(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        var b = context.PopVStack();
        var a = context.PopVStack();
        if (a < b) context.PushVStack(-1);
        else if (a == b) context.PushVStack(0);
        else context.PushVStack(1);
    }

}
