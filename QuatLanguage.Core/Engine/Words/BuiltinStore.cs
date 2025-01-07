using QuatLanguage.Core.Constants;
using System.Runtime.InteropServices;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinStore : QuatWord
{
    public BuiltinStore() : base(BuiltinWords.Store)
    {
    }

    public BuiltinStore(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        var address = context.PopVStack();
        var value = context.PopVStack();
        context.MemoryManager.WriteIntPtr(address, value);
    }


}
