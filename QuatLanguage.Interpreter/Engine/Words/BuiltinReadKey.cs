using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class BuiltinReadKey : Word
{
    public BuiltinReadKey() : base(BuiltinWords.ReadKey)
    {
    }

    public BuiltinReadKey(IToken token) : base(token)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        var key = Console.ReadKey(true);
        context.PushVStack(key.KeyChar);
    }

}
