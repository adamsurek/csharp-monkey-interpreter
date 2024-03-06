using System.Collections;
using Xunit.Abstractions;
using MonkeyInterpreter.AST;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.Tests.Parser;

public class ParserTestDataGenerator : IEnumerable<object[]>
{
	private readonly List<object[]> _data = new()
	{
		new object[]
		{
			"""
			let x = 5;
			let y = 10;
			let foobar = 123456;
			""",
			new List<string>
			{
				"x",
				"y",
				"foobar"
			}
		}
	};

	public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class ParserTest
{
	private readonly ITestOutputHelper _output;
    
	public ParserTest(ITestOutputHelper output)
	{
		_output = output;
	}
	
	[Theory]
	[ClassData(typeof(ParserTestDataGenerator))]
	public void LetStatements_ReturnsExpectedIdentifier(string input, List<string> identifiers)
	{
		Core.Lexer lexer = new(input);
		AST.Parser parser = new(lexer);

		AbstractSyntaxTree abstractSyntaxTree = parser.ParseProgram();
		CheckParserErrors(parser);

		for (int i = 0; i < identifiers.Count; i++)
		{
			_output.WriteLine($"Index: {i} - Statement: {abstractSyntaxTree.Statements[i]} - Identifier: {identifiers[i]}");
			Assert.True(LetStatement_SingleStatement_IsCompleteStatement(abstractSyntaxTree.Statements[i], identifiers[i]));
		}
		
	}

	private void CheckParserErrors(AST.Parser parser)
	{
		List<string> errors = parser.Errors();
		Assert.Empty(errors);
		
	}

	private bool LetStatement_SingleStatement_IsCompleteStatement(IStatement statement, string name)
	{
		LetStatement letStatement;
		
		if (statement is LetStatement)
		{
			letStatement = (LetStatement)statement;
		}
		else
		{
			_output.WriteLine("Statement is not a 'let' statement");
			return false;
		}
		
		if (statement.TokenLiteral() != "let")
		{
			_output.WriteLine("Missing 'let' keyword");
			return false;
		}

		if (letStatement.Name.Value != name)
		{
			_output.WriteLine($"Statement value '{letStatement.Name.Value}' doesn't equal {name}");
			return false;
		}

		if (letStatement.Name.TokenLiteral() != name)
		{
			_output.WriteLine($"Token literal '{statement.TokenLiteral()}' doesn't equal {name}");
			return false;
		}
		
		return true;
	}
}