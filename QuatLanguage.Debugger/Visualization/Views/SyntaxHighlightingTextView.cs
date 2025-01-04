using Terminal.Gui;
using TokenizerCore.Models.Constants;
using TokenizerCore.Interfaces;
using QuatLanguage.Interpreter.Constants;
using QuatLanguage.Interpreter.Parser;
using Attribute = Terminal.Gui.Attribute;
using System.Text;
using QuatLanguage.Interpreter.Engine;
using QuatLanguage.Interpreter.Engine.Words;

namespace QuatLanguage.Debugger.Visualization.Views;

public class SingleWordSuggestionGenerator : ISuggestionGenerator
{
    /// <summary>The full set of all strings that can be suggested.</summary>
    /// <returns></returns>
    public virtual List<string> AllSuggestions => GetSuggestions();

    private List<string> _builtinSuggestions = new()
    {
        BuiltinWords.Dup.ToUpperInvariant(),
        BuiltinWords.Swap.ToUpperInvariant(),
        BuiltinWords.Pop.ToUpperInvariant(),
        BuiltinWords.Over.ToUpperInvariant(),
        BuiltinWords.Rot.ToUpperInvariant(),
        BuiltinWords.Store.ToUpperInvariant(),
        BuiltinWords.Fetch.ToUpperInvariant(),
        BuiltinWords.FMove.ToUpperInvariant(),
        BuiltinWords.MoveF.ToUpperInvariant(),
        BuiltinWords.Variable.ToUpperInvariant(),
        BuiltinWords.If.ToUpperInvariant(),
        BuiltinWords.Else.ToUpperInvariant(),
        BuiltinWords.End.ToUpperInvariant(),
        BuiltinWords.Return.ToUpperInvariant(),
        BuiltinWords.Add.ToUpperInvariant(),
        BuiltinWords.Neg.ToUpperInvariant(),
        BuiltinWords.Cmp.ToUpperInvariant(),
        BuiltinWords.FAdd.ToUpperInvariant(),
        BuiltinWords.FNeg.ToUpperInvariant(),
        BuiltinWords.FCmp.ToUpperInvariant(),
        //BuiltinWords.Define.ToUpperInvariant(),
        //BuiltinWords.EndDefinition.ToUpperInvariant(),
        //BuiltinWords.FetchByte.ToUpperInvariant(),
        //BuiltinWords.StoreByte.ToUpperInvariant(),
        //BuiltinWords.PrintChar.ToUpperInvariant(),
        BuiltinWords.Prep.ToUpperInvariant(),
        BuiltinWords.Go.ToUpperInvariant(),
        BuiltinWords.Stay.ToUpperInvariant(),
        BuiltinWords.ReadKey.ToUpperInvariant(),
        BuiltinWords.Debug.ToUpperInvariant(),
        BuiltinWords.Res.ToUpperInvariant(),
    };

    private List<string> GetSuggestions()
    {
        var allSuggestions = _builtinSuggestions.Select(x => x).ToList();

        allSuggestions.AddRange(_grammars.Keys);
        return allSuggestions;
    }

    private Dictionary<string, Grammar> _grammars = new();

    public void SetGrammars(Dictionary<string, Grammar> grammars)
    {
        _grammars = grammars;
    }


    /// <inheritdoc/>
    public IEnumerable<Suggestion> GenerateSuggestions(AutocompleteContext context)
    {
        // if there is nothing to pick from
        if (AllSuggestions.Count == 0)
        {
            return Enumerable.Empty<Suggestion>();
        }

        List<Rune> line = context.CurrentLine.Select(c => c.Rune).ToList();
        string currentWord = IdxToWord(line, context.CursorPosition, out int startIdx);
        context.CursorPosition = startIdx < 1 ? 1 : Math.Min(startIdx + 1, line.Count);

        if (string.IsNullOrWhiteSpace(currentWord))
        {
            return Enumerable.Empty<Suggestion>();
        }

        return AllSuggestions.Where(
                                     o =>
                                         o.StartsWith(currentWord, StringComparison.CurrentCultureIgnoreCase)
                                         && !o.Equals(currentWord, StringComparison.CurrentCultureIgnoreCase)
                                    )
                             .Select(o => new Suggestion(currentWord.Length, o))
                             .ToList()
                             .AsReadOnly();
    }

    /// <summary>
    ///     Return true if the given symbol should be considered part of a word and can be contained in matches. Base
    ///     behavior is to use <see cref="char.IsLetterOrDigit(char)"/>
    /// </summary>
    /// <param name="rune">The rune.</param>
    /// <returns></returns>
    public virtual bool IsWordChar(Rune rune) { return char.IsLetterOrDigit((char)rune.Value); }

    /// <summary>
    ///     <para>
    ///         Given a <paramref name="line"/> of characters, returns the word which ends at <paramref name="idx"/> or null.
    ///         Also returns null if the <paramref name="idx"/> is positioned in the middle of a word.
    ///     </para>
    ///     <para>
    ///         Use this method to determine whether autocomplete should be shown when the cursor is at a given point in a
    ///         line and to get the word from which suggestions should be generated. Use the <paramref name="columnOffset"/> to
    ///         indicate if search the word at left (negative), at right (positive) or at the current column (zero) which is
    ///         the default.
    ///     </para>
    /// </summary>
    /// <param name="line"></param>
    /// <param name="idx"></param>
    /// <param name="startIdx">The start index of the word.</param>
    /// <param name="columnOffset"></param>
    /// <returns></returns>
    protected virtual string IdxToWord(List<Rune> line, int idx, out int startIdx, int columnOffset = 0)
    {
        var sb = new StringBuilder();
        startIdx = idx;

        // get the ending word index
        while (startIdx < line.Count)
        {
            if (IsWordChar(line[startIdx]))
            {
                startIdx++;
            }
            else
            {
                break;
            }
        }

        // It isn't a word char then there is no way to autocomplete that word
        if (startIdx == idx && columnOffset != 0)
        {
            return null;
        }

        // we are at the end of a word. Work out what has been typed so far
        while (startIdx-- > 0)
        {
            if (IsWordChar(line[startIdx]))
            {
                sb.Insert(0, (char)line[startIdx].Value);
            }
            else
            {
                break;
            }
        }

        startIdx = Math.Max(startIdx, 0);

        return sb.ToString();
    }
}

public class SyntaxHighlightingTextView: TextView
{

    public IToken? HighlightToken { get; set; }
    private SingleWordSuggestionGenerator _suggestionGenerator;
    public SyntaxHighlightingTextView(QuatParser parser)
    {
        _quatParser = parser;
        DrawNormalColor += HighlightSyntax;
        Added += (s, e) => LoadColors();
        LoadColors();
        _suggestionGenerator = new SingleWordSuggestionGenerator();
        Autocomplete.SuggestionGenerator = _suggestionGenerator;

        if (ContextMenu != null)
        {
            var children = ContextMenu.MenuItems.Children.ToList();
            children.Insert(0, new()
            {
                Title = "Go to definition",
                Action = () =>
                {
                    if (_parsingTask != null)
                    {
                        var x = ContextMenu.Position.X - 2;
                        var y = ContextMenu.Position.Y - 2;
                        var token = tokens.Find(token =>
                        {
                            int adjustedStart = token.Location.Column - token.Lexeme.Length;
                            if (token.Type == BuiltinTokenTypes.String)
                            {
                                adjustedStart -= 2;
                            }
                            return y == token.Location.Line && x <= token.Location.Column && x >= adjustedStart;
                        });
                        if (token != null && _grammars.TryGetValue(token.Lexeme, out var grammar) && grammar.Token != null)
                        {
                            ScrollTo(grammar.Token.Location.Line - (GetContentSize().Height / 2));
                            CursorPosition = new(token.Location.Column, token.Location.Line);
                        }
                    }
                }
            });
            ContextMenu.MenuItems.Children = children.ToArray();
        }
        
    }

    private List<IToken> tokens = new();

    private Task<List<IToken>>? _tokenizeTask;
    private Dictionary<string, Attribute> TokenColorMap = new();

    private bool contentsHaveChanged = false;

    private QuatParser _quatParser;


    public override void OnContentsChanged()
    {
        contentsHaveChanged = true;
        //if (_tokenizeTask == null || _tokenizeTask.IsFaulted)
        //{
        //    _tokenizeTask = Task.Run(() => Tokenizers.Default.Tokenize(Text).ToList());
        //}
        //else if (_tokenizeTask.IsCompletedSuccessfully)
        //{
        //    tokens = _tokenizeTask.Result;
        //    _tokenizeTask = Task.Run(() => Tokenizers.Default.Tokenize(Text).ToList());
        //}
        base.OnContentsChanged();
    }

    private Task<Dictionary<string, Grammar>>? _parsingTask;
    private Dictionary<string, Grammar> _grammars = new();
    private void HighlightSyntax(object? sender, RuneCellEventArgs e)
    {
        var text = Text;
        if (contentsHaveChanged)
        {
            //if (_tokenizeTask == null || _tokenizeTask.IsFaulted)
            //{
            //    _tokenizeTask = Task.Run(() => Tokenizers.Default.Tokenize(text).ToList());
            //}
            //else if (_tokenizeTask.IsCompletedSuccessfully)
            //{
            //    tokens = _tokenizeTask.Result;
            //    _tokenizeTask = Task.Run(() => Tokenizers.Default.Tokenize(text).ToList());
            //}
            //while (!_tokenizeTask.IsCompleted) { Thread.Sleep(100); }
            //if (_tokenizeTask.IsCompletedSuccessfully)
            //{
            //    tokens = _tokenizeTask.Result;
            //    _tokenizeTask = Task.Run(() => Tokenizers.Default.Tokenize(text).ToList());
            //}

            if (_parsingTask == null || _parsingTask.IsFaulted)
            {
                _parsingTask = Task.Run(() => _quatParser.ParseText(text, out _));
            }
            else if (_parsingTask.IsCompletedSuccessfully)
            {
                _grammars = _parsingTask.Result;
                tokens = _quatParser.GetTokens().ToList();
                _suggestionGenerator.SetGrammars(_grammars);
                _parsingTask = Task.Run(() => _quatParser.ParseText(text, out _));
            }
            while (!_parsingTask.IsCompleted) { Thread.Sleep(100); }
            if (_parsingTask.IsCompletedSuccessfully)
            {
                _grammars = _parsingTask.Result;
                tokens = _quatParser.GetTokens().ToList();
                _suggestionGenerator.SetGrammars(_grammars);
                _parsingTask = Task.Run(() => _quatParser.ParseText(text, out _));
            }



            contentsHaveChanged = false;
        }
        var token = tokens.Find(token =>
        {
            int adjustedStart = token.Location.Column - token.Lexeme.Length;
            if (token.Type == BuiltinTokenTypes.String)
            {
                adjustedStart -= 2;
            }
            return e.UnwrappedPosition.Row == token.Location.Line && e.Col < token.Location.Column && e.Col >= adjustedStart;
        });


        var cell = e.Line[e.Col];

        if (token != null && HighlightToken != null && token.Location.Column == HighlightToken.Location.Column && token.Location.Line == HighlightToken.Location.Line)
        {
            cell.ColorScheme = new ColorScheme(new Attribute(GetNormalColor().Foreground, Color.BrightYellow));
        }
        else if (token != null && _grammars.ContainsKey(token.Lexeme))
        {
            cell.ColorScheme = new ColorScheme(new Attribute(Color.DarkGray, GetNormalColor().Background));
        }
        else if (token != null && TokenColorMap.TryGetValue(token.Type, out var colorAttribute))
        {

            cell.ColorScheme = new ColorScheme(colorAttribute);

        }
        else
        {
            cell.ColorScheme = new ColorScheme(new Attribute(GetNormalColor()));
        }
        e.Line[e.Col] = cell;

    }

    public void LoadColors()
    {
        TokenColorMap = new()
        {
            { BuiltinWords.Variable, new Attribute(Color.Yellow, base.GetNormalColor().Background) },
            { BuiltinWords.Store, new Attribute(Color.Yellow, base.GetNormalColor().Background) },
            { BuiltinWords.FCmp, new Attribute(GetNormalColor().Foreground, GetNormalColor().Background) },
            { BuiltinWords.FAdd, new Attribute(GetNormalColor().Foreground, GetNormalColor().Background) },
            { BuiltinWords.FNeg, new Attribute(GetNormalColor().Foreground, GetNormalColor().Background) },
            { BuiltinWords.FMove, new Attribute(GetNormalColor().Foreground, GetNormalColor().Background) },
            { BuiltinWords.Add, new Attribute(GetNormalColor().Foreground, GetNormalColor().Background) },
            { BuiltinWords.Neg, new Attribute(GetNormalColor().Foreground, GetNormalColor().Background) },
            { BuiltinWords.Cmp, new Attribute(GetNormalColor().Foreground, GetNormalColor().Background) },
            { BuiltinWords.If, new Attribute(Color.Yellow, GetNormalColor().Background) },
            { BuiltinWords.Else, new Attribute(Color.Yellow, GetNormalColor().Background) },
            { BuiltinWords.End, new Attribute(Color.Yellow, GetNormalColor().Background) },
            { BuiltinWords.Go, new Attribute(Color.Magenta, GetNormalColor().Background) },
            { BuiltinWords.Prep, new Attribute(Color.Magenta, GetNormalColor().Background) },
            { BuiltinWords.Stay, new Attribute(Color.Magenta, GetNormalColor().Background) },
            { BuiltinWords.FetchByte, new Attribute(Color.Blue, GetNormalColor().Background) },
            { BuiltinWords.StoreByte, new Attribute(Color.Blue, GetNormalColor().Background) },
            { BuiltinWords.PrintChar, new Attribute(Color.Blue, GetNormalColor().Background) },
            { BuiltinWords.Debug, new Attribute(Color.BrightRed, GetNormalColor().Background) },
            { BuiltinTokenTypes.String, new Attribute(Color.Cyan, GetNormalColor().Background) },
        };
    }
}