using QuatLanguage.Interpreter.Factories;

var context = QuatContextFactory.CreateNew()
    .UseGlobalMemoryModel()
    .CreateContext("", out var errors);


context.LookupAndRun("Main");