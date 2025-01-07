using QuatLanguage.Core.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinFAdd : QuatWord
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
