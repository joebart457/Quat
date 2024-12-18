using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinRes : Word
{
    public BuiltinRes() : base(BuiltinWords.Res)
    {
    }

    public BuiltinRes(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        var byteCount = context.PopVStack();
        context.ReserveMemory(byteCount);
    }

}