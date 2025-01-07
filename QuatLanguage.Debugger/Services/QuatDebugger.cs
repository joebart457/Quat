using ParserLite.Exceptions;
using QuatLanguage.Debugger.Context;
using QuatLanguage.Debugger.Visualization;
using QuatLanguage.Core.Memory;
using Terminal.Gui;

namespace QuatLanguage.Debugger.Services
{
    public static class QuatDebugger
    {
        /// <summary>
        /// Calling this function will overwrite any existing TUI or debugger window
        /// with a new window loaded with the specified file
        /// </summary>
        /// <param name="filePath"></param>
        public static void ShowEditorTUI(string? filePath = null)
        {
            Application.Init();
            if (filePath != null) QuatEditorWindow.Instance.Load(filePath);
            Application.Run(QuatEditorWindow.Instance);
            Application.Shutdown();
        }

        /// <summary>
        /// Opens the TUI in debug mode and begins debugging the specified file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="stepInto">if true, breaks on the first instruction encountered, otherwise breaks only on DEBUG instruction</param>
        
        public static void StartDebugging(string filePath, bool stepInto = false)
        {
            Application.Init();
            QuatEditorWindow.Instance.Load(filePath);
            QuatEditorWindow.Instance.RunDebug(stepInto);
            Application.Shutdown();
        }

        public static DebuggableContext GetDebuggableContext(string sourceFilePath, out List<ParsingException> errors)
        {
            return (DebuggableContext)DebuggableQuatContextFactory.CreateNew()
                .UseDetachedMemoryModel()
                .CreateContext(sourceFilePath, out errors);
        }

        public static DebuggableContext GetDebuggableContext(string sourceFilePath, IMemoryManager memoryManager, out List<ParsingException> errors)
        {
            return (DebuggableContext)DebuggableQuatContextFactory.CreateNew()
                .UseMemoryModel(memoryManager)
                .CreateContext(sourceFilePath, out errors);
        }

    }
}
