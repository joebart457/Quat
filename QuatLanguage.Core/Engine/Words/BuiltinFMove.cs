using QuatLanguage.Core.Constants;
using System.Runtime.InteropServices;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinFMove : QuatWord
{
    public BuiltinFMove() : base(BuiltinWords.FMove)
    {
    }

    public BuiltinFMove(IToken token) : base(token)
    {
    }


    public override void Evaluate(QuatContext context)
    {
        var value = context.PopFStack();
        if (nint.Size == 4)
        {
            var valueAsNint = BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
            context.PushVStack(valueAsNint);

        }
        else
        {
            var valueAsNint = BitConverter.ToInt64(BitConverter.GetBytes(value), 0);
            context.PushVStack((nint)valueAsNint);
        }
    }

}
