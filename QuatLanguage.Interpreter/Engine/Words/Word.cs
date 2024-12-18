using TokenizerCore.Interfaces;

namespace QuatLanguage.Interpreter.Engine.Words;


public class Word
{
    public virtual string Name { get; set; }
    public IToken? Token { get; set; }

    public Word(string name)
    {
        Name = name;
    }

    public Word(IToken token)
    {
        Name = token.Lexeme;
        Token = token;
    }

    public virtual void Evaluate(QuatContext context)
    {

    }
}
