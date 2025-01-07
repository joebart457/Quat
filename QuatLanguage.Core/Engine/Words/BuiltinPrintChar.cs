using QuatLanguage.Core.Constants;
using System.Runtime.InteropServices;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinPrintChar : QuatWord
{
    public BuiltinPrintChar() : base(BuiltinWords.PrintChar)
    {
    }

    public BuiltinPrintChar(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        var address = context.PopVStack();
        var byteValue = context.MemoryManager.ReadByte(address);
        Console.Write((char)byteValue);
    }

}
