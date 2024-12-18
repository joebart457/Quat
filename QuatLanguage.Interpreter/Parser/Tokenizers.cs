using QuatLanguage.Interpreter.Constants;
using TokenizerCore.Model;
using TokenizerCore.Models.Constants;
using TokenizerCore;

namespace QuatLanguage.Interpreter.Parser;


public static class Tokenizers
{
    public static List<TokenizerRule> _defaultRules => new List<TokenizerRule>()
        {
                    new TokenizerRule(BuiltinWords.If, "if"),
                    new TokenizerRule(BuiltinWords.Else, "else"),
                    new TokenizerRule(BuiltinWords.End, "end"),
                    new TokenizerRule(BuiltinWords.Return, "return"),
                    new TokenizerRule(BuiltinWords.Add, "add"),
                    new TokenizerRule(BuiltinWords.Neg, "neg"),
                    new TokenizerRule(BuiltinWords.Cmp, "cmp"),
                    new TokenizerRule(BuiltinWords.FAdd, "fadd"),
                    new TokenizerRule(BuiltinWords.FNeg, "fneg"),
                    new TokenizerRule(BuiltinWords.FCmp, "fcmp"),
                    new TokenizerRule(BuiltinWords.Dup, "dup"),
                    new TokenizerRule(BuiltinWords.Swap, "swap"),
                    new TokenizerRule(BuiltinWords.Pop, "pop"),
                    new TokenizerRule(BuiltinWords.Store, "store"),
                    new TokenizerRule(BuiltinWords.Fetch, "fetch"),
                    new TokenizerRule(BuiltinWords.Variable, "variable"),
                    new TokenizerRule(BuiltinWords.StoreByte, "$"),
                    new TokenizerRule(BuiltinWords.FetchByte, "%"),

                    new TokenizerRule(BuiltinWords.PrintChar, "?"),

                    new TokenizerRule(BuiltinWords.Go, "go"),
                    new TokenizerRule(BuiltinWords.Prep, "prep"),
                    new TokenizerRule(BuiltinWords.Stay, "stay"),
                    new TokenizerRule(BuiltinWords.Over, "over"),
                    new TokenizerRule(BuiltinWords.Rot, "rot"),
                    new TokenizerRule(BuiltinWords.FMove, "fmove"),
                    new TokenizerRule(BuiltinWords.MoveF, "movef"),
                    new TokenizerRule(BuiltinWords.ReadKey, "readkey"),

                    new TokenizerRule(BuiltinWords.Define, ":-"),
                    new TokenizerRule(BuiltinWords.EndDefinition, ";"),

                    new TokenizerRule(BuiltinTokenTypes.EndOfLineComment, "//"),
                    new TokenizerRule(BuiltinTokenTypes.String, "\"", enclosingLeft: "\"", enclosingRight: "\""),
                    new TokenizerRule(BuiltinTokenTypes.String, "'", enclosingLeft: "'", enclosingRight: "'"),
                    new TokenizerRule(BuiltinTokenTypes.Word, "`", enclosingLeft: "`", enclosingRight: "`"),
        };
    public static TokenizerSettings DefaultSettings => new TokenizerSettings
    {
        AllowNegatives = true,
        NegativeChar = '-',
        NewlinesAsTokens = false,
        WordStarters = "_@",
        WordIncluded = "_@?",
        IgnoreCase = true,
        TabSize = 1,
    };
    public static Tokenizer Default => new Tokenizer(_defaultRules, DefaultSettings);

    public static Tokenizer CreateFromDefault(List<TokenizerRule> rules)
    {
        var defaultRulesCopy = _defaultRules.Select(x => x).ToList();
        defaultRulesCopy.AddRange(rules);
        return new Tokenizer(defaultRulesCopy, DefaultSettings);
    }
}