using QuatLanguage.Debugger.Context;
using System.Reflection.Metadata.Ecma335;
using TokenizerCore.Models.Constants;


var context = QuatContextFactory.CreateNew()
                .AddBasicWord<ExDebug>()
                .AddRule((tokenParser) => tokenParser.AdvanceIfMatch(BuiltinTokenTypes.String)? new ExString(tokenParser.Previous(), tokenParser.Previous().Lexeme) : null)
                .CreateContext("C:\\Users\\Jimmy\\Desktop\\Repositories\\QuatLanguage\\spec2.txt", out var errors);
context.LookupAndRun("Main");
