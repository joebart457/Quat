namespace QuatLanguage.Interpreter.Memory
{
    public interface IMemoryManager
    {
        nint AllocateAndTrack(nint bytes);
        nint AllocateNativeIntegerSize();
        nint AllocateString(string str);
        nint AllocateStruct<T>(T tStruct) where T : struct;
        void ForceFree(nint ptr);
        void FreeAll();
        bool FreeMemory(nint ptr);
        nint ReadIntPtr(nint ptr);
        string? ReadAsString(nint ptr);
        T ReadAsStruct<T>(nint ptr) where T : struct;
        byte ReadByte(nint ptr);
        nint TrackMemory(nint ptr);
        void WriteIntPtr(nint ptr, nint valueToWrite);
        void WriteByte(nint ptr, byte valueToWrite);
    }
}