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

			Console.WriteLine($"'{line}'");
			
			if (line is null)
			{
				return;
			}
			
			Lexer lexer = new Lexer(line!);

			while (lexer.Character.ToString() != Token.Eof)
			{
				Token? token = lexer.NextToken();
				if (token is not null)
				{
					writer.WriteLine($"Literal: {token.Literal}, Type: {token.Type}");
				}
			}
		}
	}
}