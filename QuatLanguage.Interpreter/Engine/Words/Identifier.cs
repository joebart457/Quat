using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;

public class Identifier : Word
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
