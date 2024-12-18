using QuatLanguage.Debugger.Visualization;
using QuatLanguage.Interpreter.Engine;
using QuatLanguage.Interpreter.Engine.Words;
using Terminal.Gui;
using TokenizerCore.Interfaces;

namespace QuatLanguage.Debugger.Context;


public class ExDebug : Word
{
    public ExDebug(IToken token) : base(token) { }
    public ExDebug(): base(nameof(ExDebug)) { }

    public override void Evaluate(QuatContext context)
    {
        if (context is DebuggableContext debuggableContext)
        {
            debuggableContext.DoDebugBreak(this);
        }
        
    }

}

public class ExString: Word
{
    public string Value { get; set; }
    public ExString(IToken token, string str) : base(token)
    {
        Value = str;
    }

    public override void Evaluate(QuatContext context)
    {
        context.PushStringVStack(Value);

    }

}