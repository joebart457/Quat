using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinPop : Word
{
    public BuiltinPop() : base(BuiltinWords.Pop)
    {
    }

    public BuiltinPop(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        _ = context.PopVStack();
    }

}
