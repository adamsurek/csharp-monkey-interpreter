using MonkeyInterpreter.Core;
using MonkeyInterpreter.Core.AbstractSyntaxTree;
using MonkeyInterpreter.Core.Evaluator;
using MonkeyInterpreter.Core.Parser;

namespace MonkeyInterpreter.REPL;

public static class Repl
{
	private const string Prompt = ">> ";

	public static void Start(TextReader reader, TextWriter writer)
	{
		VariableEnvironment env = new();
		
		while (true)
		{
			writer.Write(Prompt);
			string? line = reader.ReadLine();

			if (line is null)
			{
				return;
			}
			
			Lexer lexer = new(line!);
			Parser parser = new(lexer);
			AbstractSyntaxTree ast = parser.ParseProgram();

			List<string> errors = parser.Errors();

			if (errors.Count() != 0)
			{
				PrintParserErrors(writer, errors);
				continue;
			}

			IObject evaluatedObject = Evaluator.Evaluate(ast, env);

			if (evaluatedObject.Type() != ObjectTypeEnum.Null)
			{
				writer.WriteLine(evaluatedObject.Inspect());
			}

		}
	}

	private static void PrintParserErrors(TextWriter writer, List<string> errors)
	{
		foreach (string error in errors)
		{
			writer.WriteLine($"\t{error}");
		}
	}
}