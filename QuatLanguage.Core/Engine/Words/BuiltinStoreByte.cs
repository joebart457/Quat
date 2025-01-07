using QuatLanguage.Core.Constants;
using System.Runtime.InteropServices;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinStoreByte : QuatWord
{
    public BuiltinStoreByte() : base(BuiltinWords.FetchByte)
    {
    }

    public BuiltinStoreByte(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        var address = context.PopVStack();
        var value = context.PopVStack();
        var byteValue = BitConverter.GetBytes(value)[0];
        context.MemoryManager.WriteByte(address, byteValue);
    }
}
