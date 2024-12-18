using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinFCmp : Word
{
    public BuiltinFCmp() : base(BuiltinWords.FCmp)
    {
    }

    public BuiltinFCmp(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        var b = context.PopFStack();
        var a = context.PopFStack();
        if (a < b) context.PushVStack(-1);
        else if (a == b) context.PushVStack(0);
        else context.PushVStack(1);
    }

}
