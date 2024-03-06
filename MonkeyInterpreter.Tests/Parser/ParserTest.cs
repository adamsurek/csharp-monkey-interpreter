using System.Collections;
using Xunit.Abstractions;
using MonkeyInterpreter.AST;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.Tests.Parser;

public class LetStatementTestDataGenerator : IEnumerable<object[]>
{
	private readonly List<object[]> _data =
	[
		[
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
		]
	];

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
	[ClassData(typeof(LetStatementTestDataGenerator))]
	public void LetStatements_ReturnsExpectedIdentifier(string input, List<string> identifiers)
	{
		Core.Lexer lexer = new(input);
		AST.Parser parser = new(lexer);

		AbstractSyntaxTree abstractSyntaxTree = parser.ParseProgram();
		CheckParserErrors(parser);

		for (int i = 0; i < identifiers.Count; i++)
		{
			_output.WriteLine(
				$"Index: {i} - Statement: {abstractSyntaxTree.Statements[i]} - Identifier: {identifiers[i]}");
			Assert.True(
				LetStatement_SingleStatement_IsCompleteStatement(abstractSyntaxTree.Statements[i], identifiers[i]));
		}
	}

	[Theory]
	[InlineData(
		"""
		return 5;
		return 10;
		return 123456;
		""", 3
	)]
	public void ReturnStatements_ReturnsExpectedIdentifier(string input, int statementCount)
	{
		Core.Lexer lexer = new(input);
		AST.Parser parser = new(lexer);

		AbstractSyntaxTree abstractSyntaxTree = parser.ParseProgram();
		CheckParserErrors(parser);

		Assert.Equal(statementCount, abstractSyntaxTree.Statements.Count);	

		for (int i = 0; i < abstractSyntaxTree.Statements.Count; i++)
		{
			_output.WriteLine(
				$"Index: {i} - Statement: {abstractSyntaxTree.Statements[i]}");
			Assert.IsType<ReturnStatement>(abstractSyntaxTree.Statements[i]);
			Assert.Equal("return", abstractSyntaxTree.Statements[i].TokenLiteral());
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
		
		if (statement is LetStatement letStmt)
		{
			letStatement = letStmt;
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