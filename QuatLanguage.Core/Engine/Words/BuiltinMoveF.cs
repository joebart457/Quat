﻿using QuatLanguage.Core.Constants;
using System.Runtime.InteropServices;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinMoveF : QuatWord
{
    public BuiltinMoveF() : base(BuiltinWords.MoveF)
    {
    }

    public BuiltinMoveF(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        var value = context.PopVStack();
        if (nint.Size == 4)
        {
            var valueAsNFloat = BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
            context.PushFStack(valueAsNFloat);

        }
        else
        {
            var valueAsNFloat = BitConverter.ToDouble(BitConverter.GetBytes(value), 0);
            context.PushFStack((NFloat)valueAsNFloat);
        }
    }

}
