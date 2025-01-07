using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class Identifier : QuatWord
{
    public Identifier(string name) : base(name)
    {
    }

    public Identifier(IToken name) : base(name)
    {
    }

    public override void Evaluate(QuatContext context)
    {
        context.LookupAndRun(Name);
    }


}
