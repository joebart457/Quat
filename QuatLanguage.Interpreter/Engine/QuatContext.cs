
using QuatLanguage.Interpreter.Engine.Words;
using QuatLanguage.Interpreter.Memory;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace QuatLanguage.Interpreter.Engine;

public class QuatContext
{
    public IMemoryManager MemoryManager { get; protected set; }
    public Dictionary<string, Grammar> Grammars { get; protected set; }
    protected Stack<nint> _valueStack = new();
    protected Stack<NFloat> _floatStack = new();
    protected Stack<int> _addressStack = new();

    // user defined data that can persist between Word executions
    // useful for passing information within the interpreter itself
    public virtual object? PersistentData { get; set; }
    public virtual T GetPersistentData<T>()
    {
        if (PersistentData is T tyVal) return tyVal;
        throw new InvalidCastException($"Cannot cast PersistentData to type {typeof(T).FullName}");
    }
    public virtual void SetPersistentData<T>(T data) => PersistentData = data;
    public string? SourceFilePath { get; protected set; }

    public QuatContext(IMemoryManager memoryManager, Dictionary<string, Grammar> grammars, string? sourceFilePath = null)
    {
        Grammars = grammars;    
        SourceFilePath = sourceFilePath;
        MemoryManager = memoryManager;
    }

    public virtual void LookupAndRun(string word)
    {
        if (Grammars.TryGetValue(word, out var grammar))
        {
            grammar.Evaluate(this);
        }
        else throw new InvalidOperationException($"defintion for '{word}' not found!");
    }

    public virtual void PushVStack(nint value)
    {
        _valueStack.Push(value);
    }

    public virtual nint PopVStack()
    {
        if (!_valueStack.Any()) throw new InvalidOperationException("unable to pop from empty stack!");
        return _valueStack.Pop();
    }

    public virtual void PushFStack(NFloat value)
    {
        _floatStack.Push(value);
    }

    public virtual NFloat PopFStack()
    {
        if (!_floatStack.Any()) throw new InvalidOperationException("unable to pop from empty floating point stack!");
        return _floatStack.Pop();
    }

    public virtual void PushAddressStack(int address)
    {
        _addressStack.Push(address);
    }

    public virtual int PopAddressStack()
    {
        if (!_addressStack.Any()) throw new InvalidOperationException("unable to pop from empty address stack!");
        return _addressStack.Pop();
    }

    public virtual byte FetchByte()
    {
        return MemoryManager.ReadByte(PopVStack());
    }

    public virtual nint ReserveMemory()
    {
        return MemoryManager.AllocateNativeIntegerSize();
    }

    public virtual nint ReserveMemory(nint bytes)
    {
        return MemoryManager.AllocateAndTrack(bytes);
    }

    public virtual void DoDebugBreak(Word word)
    {
        Debugger.Break();
    }


    #region PublicAPIConvienceMethods


    public virtual string? MarshalStringFromVStack()
    {
        var ptr = PopVStack();
        return MemoryManager.ReadAsString(ptr);
    }

    public virtual T MarshalStructFromVStack<T>() where T: struct
    {
        var ptr = PopVStack();
        return MemoryManager.ReadAsStruct<T>(ptr);
    }

    // Returns a pointer to the HeapAllocated string that was pushed to the stack
    public virtual nint PushStringToVStack(string str)
    {
        var ptr = MemoryManager.AllocateString(str);
        PushVStack(ptr);
        return ptr;
    }

    // Returns a pointer to the HeapAllocated struct that was pushed to the stack
    public virtual nint PushStructToVStack<T>(T tStruct) where T : struct
    {
        var ptr = MemoryManager.AllocateStruct(tStruct);
        PushVStack(ptr);
        return ptr;
    }

    #endregion

}