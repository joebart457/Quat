using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinFAdd : Word
{
    public BuiltinFAdd() : base(BuiltinWords.Add)
    {
    }

    public BuiltinFAdd(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        var b = context.PopFStack();
        var a = context.PopFStack();
        context.PushFStack(a + b);
    }

}
