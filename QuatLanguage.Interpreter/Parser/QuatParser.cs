using ParserLite;
using ParserLite.Exceptions;
using QuatLanguage.Interpreter.Constants;
using QuatLanguage.Interpreter.Engine;
using QuatLanguage.Interpreter.Engine.Words;
using QuatLanguage.Interpreter.Memory;
using System.Runtime.InteropServices;
using TokenizerCore;
using TokenizerCore.Models.Constants;

namespace QuatLanguage.Interpreter.Parser;


public class QuatParser : TokenParser
{

    private List<Func<IMemoryManager, TokenParser, Word?>> _wordParsingRules = new List<Func<IMemoryManager, TokenParser, Word?>>()
    {
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.If)) return new BuiltinIf(tokenParser.Previous());                  else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.Else)) return new BuiltinElse(tokenParser.Previous());              else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.End)) return new BuiltinEnd(tokenParser.Previous());                else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.Return)) return new BuiltinReturn(tokenParser.Previous());          else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.Add)) return new BuiltinAdd(tokenParser.Previous());                else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.Neg)) return new BuiltinNeg(tokenParser.Previous());                else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.Cmp)) return new BuiltinCmp(tokenParser.Previous());                else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.FAdd)) return new BuiltinFAdd(tokenParser.Previous());              else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.FNeg)) return new BuiltinFNeg(tokenParser.Previous());              else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.FCmp)) return new BuiltinFCmp(tokenParser.Previous());              else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.Dup)) return new BuiltinDup(tokenParser.Previous());                else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.Swap)) return new BuiltinStore(tokenParser.Previous());             else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.Pop)) return new BuiltinPop(tokenParser.Previous());                else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.Fetch)) return new BuiltinFetch(tokenParser.Previous());            else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.Store)) return new BuiltinStore(tokenParser.Previous());            else return null; },
        
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.FetchByte)) return new BuiltinFetchByte(tokenParser.Previous());    else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.PrintChar)) return new BuiltinPrintChar(tokenParser.Previous());    else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.Go)) return new BuiltinGo(tokenParser.Previous());                  else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.Prep)) return new BuiltinPrep(tokenParser.Previous());              else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.StoreByte)) return new BuiltinStoreByte(tokenParser.Previous());    else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.FMove)) return new BuiltinFMove(tokenParser.Previous());            else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.MoveF)) return new BuiltinMoveF(tokenParser.Previous());            else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.Stay)) return new BuiltinStay(tokenParser.Previous());              else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.Over)) return new BuiltinOver(tokenParser.Previous());              else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.Rot)) return new BuiltinRot(tokenParser.Previous());                else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.Debug)) return new BuiltinDebug(tokenParser.Previous());            else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinTokenTypes.Integer)) return new LiteralInteger(tokenParser.Previous(), nint.Parse(tokenParser.Previous().Lexeme));         else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinTokenTypes.Double)) return new LiteralFloatingPoint(tokenParser.Previous(), NFloat.Parse(tokenParser.Previous().Lexeme));  else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinTokenTypes.Word)) return new Identifier(tokenParser.Previous());                            else return null; },
        (memoryManager, tokenParser) => {
            if (tokenParser.AdvanceIfMatch(BuiltinTokenTypes.String))
            {
                var str = tokenParser.Previous().Lexeme;
                var ptr = memoryManager.AllocateString(str);
                 return new LiteralInteger(tokenParser.Previous(), ptr);
            }
            else return null;
        },
        (memoryManager, tokenParser) => { 
            if (tokenParser.AdvanceIfMatch(BuiltinWords.Variable))
            {
                return new LiteralInteger(memoryManager.AllocateNativeIntegerSize());
            }      
            else return null; 
        },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.ReadKey)) return new BuiltinReadKey(tokenParser.Previous());    else return null; },
        (memoryManager, tokenParser) => { if (tokenParser.AdvanceIfMatch(BuiltinWords.Res)) return new BuiltinRes(tokenParser.Previous());            else return null; },
    };


    private readonly Tokenizer _tokenizer = Tokenizers.Default;
    private readonly IMemoryManager _memoryManager;
    public QuatParser(IMemoryManager memoryManager, List<Func<IMemoryManager, TokenParser, Word?>> wordParsingRules, bool overrideExisting = false, Tokenizer? tokenizer = null)
    {
        if (overrideExisting)
            _wordParsingRules = wordParsingRules;
        else _wordParsingRules.AddRange(wordParsingRules);
        if (!overrideExisting) _wordParsingRules.Reverse(); // reverse rules so user rules occur before builtin rules
        _tokenizer = tokenizer ?? Tokenizers.Default;
        _memoryManager = memoryManager;
    }

    public QuatParser()
    {
        _memoryManager = GlobalMemoryManager.Instance;
    }

    public Dictionary<string, Grammar> ParseFile(string path, out List<ParsingException> errors)
    {
        return ParseText(File.ReadAllText(path), out errors);
    }

    public Dictionary<string, Grammar> ParseText(string text, out List<ParsingException> errors)
    {
        errors = new List<ParsingException>();
        var grammars = new Dictionary<string, Grammar>();
        var tokens = _tokenizer.Tokenize(text);
        Initialize(tokens.Where(t => t.Type != BuiltinTokenTypes.EndOfFile).ToList());
        while (!AtEnd())
        {
            try
            {
                var grammar = ParseGrammar();
                if (grammars.ContainsKey(grammar.Name))
                    throw new ParsingException(Previous(), $"redefinition of word {grammar.Name}");
                grammars[grammar.Name] = grammar;
            }
            catch (ParsingException pe)
            {
                errors.Add(pe);
                SeekToNextParsableUnit();
            }
        }
        return grammars;
    }

    public Grammar ParseGrammar()
    {
        var name = Consume(BuiltinTokenTypes.Word, "expect definition");
        Consume(BuiltinWords.Define, "expect <name> :- definition ;");
        var words = new List<Word>();
        if (!AdvanceIfMatch(BuiltinWords.EndDefinition))
        {
            do
            {
                words.Add(ParseWord());
            } while (!AtEnd() && !Match(BuiltinWords.EndDefinition));
            Consume(BuiltinWords.EndDefinition, "expect definition to end with ;");
        }
        return new Grammar(name, words);
    }

    public Word ParseWord()
    {
        foreach(var wordParsingRule in _wordParsingRules)
        {
            var parsedResult = wordParsingRule(_memoryManager, this);
            if (parsedResult != null) return parsedResult;
        }

        throw new ParsingException(Current(), $"unexpected token {Current()}");
    }

    private void SeekToNextParsableUnit()
    {
        while (!AtEnd())
        {
            if (AdvanceIfMatch(BuiltinWords.EndDefinition)) break;
            Advance();
        }
    }
}
