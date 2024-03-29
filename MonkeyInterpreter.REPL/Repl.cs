﻿using MonkeyInterpreter.AST;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.REPL;

public static class Repl
{
	private const string Prompt = ">> ";

	public static void Start(TextReader reader, TextWriter writer)
	{
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

			writer.WriteLine(ast.String());
			writer.WriteLine();
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