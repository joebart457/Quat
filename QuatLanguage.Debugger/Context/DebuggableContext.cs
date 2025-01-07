using QuatLanguage.Debugger.Visualization;
using QuatLanguage.Core.Engine;
using QuatLanguage.Core.Engine.Words;
using QuatLanguage.Core.Memory;
using System.Runtime.InteropServices;
using Terminal.Gui;

namespace QuatLanguage.Debugger.Context;

public class DebuggableContext : QuatContext
{
    public DebuggableContext(IMemoryManager memoryManager, Dictionary<string, Grammar> grammars, string? sourceFilePath = null) 
        : base(memoryManager, grammars, sourceFilePath)
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
        else throw new InvalidOperationException($"definition for '{word}' not found!");
    }

    private bool IsSourceFileLoaded = false;
    public override void DoDebugBreak(QuatWord word)
    {

        if (word.Token != null && SourceFilePath != null)
        {
            if (!IsSourceFileLoaded)
            {
                QuatEditorWindow.Instance.LoadDebugView(SourceFilePath);
                IsSourceFileLoaded = true;
            }
            QuatEditorWindow.Instance.ScrollTo(word.Token);
            Application.Run(QuatEditorWindow.Instance);
        }

    }

    public void EvaluateGrammar(Grammar grammar)
    {
        var Words = grammar.Words;
        var context = this;
        for (int i = 0; i < Words.Count; i++)
        {
            if (QuatEditorWindow.Instance?.DebugBreakOnNext == true)
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
            else if (Words[i] is BuiltinPrintChar builtinPrintChar && QuatEditorWindow.Instance != null)
            {
                var address = context.PopVStack();
                var byteValue = context.MemoryManager.ReadByte(address);
                QuatEditorWindow.Instance.WriteConsoleMessage(new string((char)byteValue, 1));
            }
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