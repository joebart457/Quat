using QuatLanguage.Interpreter.Factories;

var context = QuatContextFactory.CreateNew()
    .UseGlobalMemoryModel()
    .CreateContext("spec2.txt", out var errors);


context.LookupAndRun("Main");