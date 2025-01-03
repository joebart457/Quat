using QuatLanguage.Debugger.Context;
using QuatLanguage.Debugger.Visualization;
using Terminal.Gui;

//var context = DebuggableQuatContextFactory.CreateNew()
//        .UseDetachedMemoryModel()
//        .CreateContext("C:\\Users\\Jimmy\\Desktop\\Repositories\\QuatLanguage\\spec2.txt", out var errors);
//context.LookupAndRun("Main");
//context.Dispose();

Application.Init();
Application.Run<QuatEditorWindow>();

Application.Shutdown();