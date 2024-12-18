using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinStay : Word
{
    public BuiltinStay() : base(BuiltinWords.Stay)
    {
    }

    public BuiltinStay(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        context.PopAddressStack();
    }

}
