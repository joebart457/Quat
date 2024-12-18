using QuatLanguage.Interpreter.Constants;
using System.Runtime.InteropServices;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinFetch : Word
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
        var value = Marshal.ReadIntPtr(address);
        context.PushVStack(value);
    }
}
