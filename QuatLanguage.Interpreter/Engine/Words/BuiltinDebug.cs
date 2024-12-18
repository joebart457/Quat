using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinDebug : Word
{
    public BuiltinDebug() : base(BuiltinWords.Debug)
    {
    }

    public BuiltinDebug(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        context.DoDebugBreak(this);
    }

}
