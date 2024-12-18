using QuatLanguage.Debugger.Visualization;
using QuatLanguage.Interpreter.Engine;
using QuatLanguage.Interpreter.Engine.Words;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;
using TokenizerCore.Model;

namespace QuatLanguage.Debugger.Context;

public class DebuggableContext : QuatContext
{
    public DebuggableContext(Dictionary<string, Grammar> grammars, string sourceFilePath) : base(grammars, sourceFilePath)
    {
    }

    public Stack<nint> ValueStack => _valueStack;
    public Stack<NFloat> FloatStack => _floatStack;
    public Stack<(int, Grammar)> AddressStack { get; set; } = new();
    public Stack<Grammar> CallStack { get; private set; } = new();
    public override void LookupAndRun(string word)
    {
        if (Grammars.TryGetValue(word, out var grammar))
        {
            CallStack.Push(grammar);
            EvaluateGrammar(grammar);
            CallStack.Pop();
        }
        else throw new InvalidOperationException($"defintion for '{word}' not found!");
    }

    private bool AppIsInitialized = false;
    public void DoDebugBreak(Word word)
    {
        if (!AppIsInitialized)
        {
            Application.Init();
            AppIsInitialized = true;
        }
        if (DebuggerWindow.Instance == null)
        {
            DebuggerWindow.Instance = new DebuggerWindow();
        }
        DebuggerWindow.Instance.SetDebuggableContext(this);
        DebuggerWindow.Instance.OpenFile(SourceFilePath);
        if (word.Token != null) DebuggerWindow.Instance.ScrollTo(word.Token);
        Application.Run(DebuggerWindow.Instance);
        if (AppIsInitialized && !DebuggerWindow.Instance.DebugBreakOnNext)
        {
            Application.Shutdown();
            AppIsInitialized = false;
        }
    }

    public void EvaluateGrammar(Grammar grammar)
    {
        var Words = grammar.Words;
        var context = this;
        for (int i = 0; i < Words.Count; i++)
        {
            if (DebuggerWindow.Instance?.DebugBreakOnNext == true)
            {
                DoDebugBreak(Words[i]);
            }
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

    public override void PushAddressStack(int address)
    {
        AddressStack.Push((address, CallStack.Peek()));
    }

    public override int PopAddressStack()
    {
        if (!AddressStack.Any()) throw new InvalidOperationException("unable to pop from empty address stack!");
        return AddressStack.Pop().Item1;
    }


}