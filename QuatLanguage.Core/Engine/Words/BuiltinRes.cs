using QuatLanguage.Core.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinRes : QuatWord
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