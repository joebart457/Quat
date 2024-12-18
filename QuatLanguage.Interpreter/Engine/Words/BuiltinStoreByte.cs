using QuatLanguage.Interpreter.Constants;
using System.Runtime.InteropServices;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinStoreByte : Word
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
        Marshal.WriteByte(address, byteValue);
    }
}
