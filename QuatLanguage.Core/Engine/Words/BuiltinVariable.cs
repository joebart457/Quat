using QuatLanguage.Core.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinVariable : QuatWord
{
    public BuiltinVariable() : base(BuiltinWords.Variable)
    {
    }

    public BuiltinVariable(IToken token) : base(token)
    {

    }

    public override void Evaluate(QuatContext context)
    {
        var ptr = context.ReserveMemory();
        context.PushVStack(ptr);
    }

}
