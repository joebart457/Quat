using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinVariable : Word
{
    public BuiltinVariable() : base(BuiltinWords.Variable)
    {
    }

    public BuiltinVariable(IToken token) : base(token)
    {

    }

    public override void Evaluate(QuatContext context)
    {
        var ptr = context.ReserveMemory();
        context.PushVStack(ptr);
    }

}
