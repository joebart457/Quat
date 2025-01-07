using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;


public class QuatWord
{
    public virtual string Name { get; set; }
    public IToken? Token { get; set; }

    public QuatWord(string name)
    {
        Name = name;
    }

    public QuatWord(IToken token)
    {
        Name = token.Lexeme;
        Token = token;
    }

    public virtual void Evaluate(QuatContext context)
    {

    }
}
