using QuatLanguage.Core.Constants;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class BuiltinReadKey : QuatWord
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
