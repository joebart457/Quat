using QuatLanguage.Interpreter.Factories;

var context = QuatContextFactory.CreateNew()
    .UseGlobalMemoryModel()
    .CreateContext("C:\\Users\\Jimmy\\Desktop\\Repositories\\QuatLanguage\\spec2.txt", out var errors);


context.LookupAndRun("Main");