﻿using ParserLite.Exceptions;
using ParserLite;
using QuatLanguage.Interpreter.Engine;
using QuatLanguage.Interpreter.Parser;
using TokenizerCore.Model;
using QuatLanguage.Interpreter.Memory;
using QuatLanguage.Interpreter.Engine.Words;

namespace QuatLanguage.Debugger.Context;

public class QuatContextFactory
{
    protected List<Func<TokenParser, Word?>> _parsingRules = new();
    protected List<TokenizerRule> _tokenizerRules = new();
    public static QuatContextFactory CreateNew() => new QuatContextFactory();
    protected bool _useDetachedMemoryModel = false;
    protected bool _useGlobalMemoryModel = false;
    protected IMemoryManager? _memoryManager;
    public virtual QuatContextFactory AddBasicWord<T>(string? nameOverride = null) where T : Word, new()
    {
        var name = nameOverride ?? typeof(T).Name;
        _tokenizerRules.Add(new(name, name));
        _parsingRules.Add((tokenParser) =>
        {
            if (tokenParser.AdvanceIfMatch(name))
            {
                var parsedWord = new T();
                parsedWord.Token = tokenParser.Previous();
                return parsedWord;
            }
            return null;
        });
        return this;
    }

    public virtual QuatContextFactory AddRule(Func<TokenParser, Word?> parsingRule)
    {
        _parsingRules.Add(parsingRule); 
        return this;
    }

    public virtual QuatContextFactory UseDetachedMemoryModel()
    {
        _useDetachedMemoryModel = true;
        _useGlobalMemoryModel = false;
        return this;
    }

    public virtual QuatContextFactory UseGlobalMemoryModel()
    {
        _useDetachedMemoryModel = false;
        _useGlobalMemoryModel = true;
        return this;
    }

    public virtual QuatContextFactory UseMemoryModel(IMemoryManager memoryManager)
    {
        _memoryManager = memoryManager;
        return this;
    }

    public virtual QuatParser EmitParser(bool overwriteExistingParser = false)
    {

        return new QuatParser(_parsingRules, overwriteExistingParser, Tokenizers.CreateFromDefault(_tokenizerRules));
    }

    protected virtual QuatParser EmitParser(bool overwriteExistingParser, IMemoryManager memoryManager)
    {

        return new QuatParser(_parsingRules, overwriteExistingParser, Tokenizers.CreateFromDefault(_tokenizerRules), memoryManager);
    }


    public virtual QuatContext CreateContext(string filePath, out List<ParsingException> errors)
    {
        var memoryManager = DecideMemoryManager();
        var parser = EmitParser(false, memoryManager);
        var grammars = parser.ParseFile(filePath, out errors);
        return new QuatContext(grammars, filePath, memoryManager);
    }


    protected virtual IMemoryManager DecideMemoryManager()
    {
        if (!_useGlobalMemoryModel && !_useDetachedMemoryModel && _memoryManager == null)
            throw new InvalidOperationException("Memory model has not been specified. Call UseDetachedMemoryModel() or UseGlobalMemoryModel() to set the default memory model. Otherwise provide your own by calling UseMemoryModel(IMemoryModel)");
        if (_memoryManager != null) return _memoryManager;
        if (_useDetachedMemoryModel) return GlobalMemoryManager.Instance;
        return GlobalMemoryManager.CreateDetachedInstance();
    }

}