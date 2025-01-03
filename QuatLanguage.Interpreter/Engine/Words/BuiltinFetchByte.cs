using QuatLanguage.Interpreter.Constants;
using System.Runtime.InteropServices;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinFetchByte : Word
{
    public BuiltinFetchByte() : base(BuiltinWords.FetchByte)
    {
    }

    public BuiltinFetchByte(IToken token) : base(token)
    {
    }


    public override void Evaluate(QuatContext context)
    {
        var address = context.PopVStack();
        var byteValue = context.MemoryManager.ReadByte(address);
        context.PushVStack(byteValue);
    }
}
