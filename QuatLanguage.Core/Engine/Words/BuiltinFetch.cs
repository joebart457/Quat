using QuatLanguage.Core.Constants;
using System.Runtime.InteropServices;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinFetch : QuatWord
{
    public BuiltinFetch() : base(BuiltinWords.Neg)
    {
    }

    public BuiltinFetch(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        var address = context.PopVStack();
        var value = context.MemoryManager.ReadIntPtr(address);
        context.PushVStack(value);
    }
}
