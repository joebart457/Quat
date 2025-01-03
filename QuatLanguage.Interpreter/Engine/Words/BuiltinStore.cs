using QuatLanguage.Interpreter.Constants;
using System.Runtime.InteropServices;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinStore : Word
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
