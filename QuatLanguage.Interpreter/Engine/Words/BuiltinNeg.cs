using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinNeg : Word
{
    public BuiltinNeg() : base(BuiltinWords.Neg)
    {
    }

    public BuiltinNeg(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        var a = context.PopVStack();
        context.PushVStack(-a);
    }

}
