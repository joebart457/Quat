using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinFNeg : Word
{
    public BuiltinFNeg() : base(BuiltinWords.Neg)
    {
    }

    public BuiltinFNeg(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        var a = context.PopFStack();
        context.PushFStack(-a);
    }

}
