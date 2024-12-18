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
        var byteValue = Marshal.ReadByte(address);
        if (nint.Size == 4)
        {
            var paddedByte = BitConverter.ToInt32([byteValue, 0, 0, 0], 0);
            context.PushVStack(paddedByte);

        }
        else
        {
            long paddedByte = byteValue;
            context.PushVStack((nint)paddedByte);
        }
    }
}
