using QuatLanguage.Core.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinAdd : QuatWord
{
    public BuiltinAdd() : base(BuiltinWords.Add)
    {
    }

    public BuiltinAdd(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        var b = context.PopVStack();
        var a = context.PopVStack();
        context.PushVStack(a + b);
    }

}
