using TokenizerCore.Interfaces;

namespace QuatLanguage.Core.Engine.Words;

public class Grammar : QuatWord
{
    public List<QuatWord> Words { get; set; }

    public Grammar(string name, List<QuatWord> words) : base(name)
    {
        Words = words;
    }

    public Grammar(IToken name, List<QuatWord> words) : base(name)
    {
        Words = words;
    }

    public override void Evaluate(QuatContext context)
    {
        for (int i = 0; i < Words.Count; i++)
        {
            if (Words[i] is BuiltinGo go)
            {
                i = context.PopAddressStack();
                i--;
                continue;
            }
            else if (Words[i] is BuiltinPrep builtinPrep)
            {
                context.PushAddressStack(i);
            }
            else if (Words[i] is BuiltinReturn builtinReturn)
            {
                break;
            }
            else if (Words[i] is BuiltinIf builtinIf)
            {
                var condition = context.PopVStack();
                if (condition == 0)
                {
                    var nestedIfs = 0;
                    // skip to else or end
                    while (i < Words.Count)
                    {
                        i++;
                        if (Words[i] is BuiltinIf nestedIf) nestedIfs++;
                        else if (Words[i] is BuiltinElse && nestedIfs <= 0) break;
                        else if (Words[i] is BuiltinEnd)
                        {
                            if (nestedIfs == 0) break;
                            nestedIfs--;
                        }
                    }
                }
            }
            else if (Words[i] is BuiltinElse builtinElse)
            {
                // if we encounter an else naturually, skip to the end
                var nestedIfs = 0;
                // skip to else or end
                while (i < Words.Count)
                {
                    i++;
                    if (Words[i] is BuiltinIf nestedIf) nestedIfs++;
                    else if (Words[i] is BuiltinEnd)
                    {
                        if (nestedIfs == 0) break;
                        nestedIfs--;
                    }
                }
            }
            else if (Words[i] is BuiltinEnd builtinEnd) { }
            else Words[i].Evaluate(context);

        }
    }
}
