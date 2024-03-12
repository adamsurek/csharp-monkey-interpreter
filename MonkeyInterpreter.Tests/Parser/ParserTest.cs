using System.Collections;
using Xunit.Abstractions;
using MonkeyInterpreter.AST;

namespace MonkeyInterpreter.Tests.Parser;

// TODO: Consolidate tests - lots of repetition

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

public class IdentifierExpressionTests
{
	[Theory]
	[InlineData("foobar;", 1)]
	public void IdentifierExpression_ReturnsExpectedStatementCount(string expression, int expectedStatementCount)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();
	
		Assert.Equal(expectedStatementCount, ast.Statements.Count);
	}

	[Theory]
	[InlineData("foobar;")]
	public void IdentifierExpression_StatementIsExpressionStatement(string expression)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();
	
		Assert.IsType<ExpressionStatement>(ast.Statements[0]);
	}

	[Theory]
	[InlineData("foobar;")]
	public void IdentifierExpression_ExpressionStatementExpressionIsIdentifier(string expression)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		
		Assert.IsType<Identifier>(expressionStatement.Expression);
	}

	[Theory]
	[InlineData("foobar;", "foobar")]
	public void IdentifierExpression_ReturnsExpectedValue(string expression, string expectedValue)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		Identifier identifier = (Identifier)expressionStatement.Expression;

		Assert.Equal(expectedValue, identifier.Value);
	}
}

public class IntegerLiteralExpressionTests
{
	[Theory]
	[InlineData("5;", 1)]
	public void IntegerLiteralExpression_ReturnsExpectedStatementCount(string expression, int expectedStatementCount)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		Assert.Equal(expectedStatementCount, ast.Statements.Count);
	}
	
	[Theory]
	[InlineData("5;")]
	public void IntegerLiteralExpression_StatementIsExpressionStatement(string expression)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		Assert.IsType<ExpressionStatement>(ast.Statements[0]);
	}
	
	[Theory]
	[InlineData("5;")]
	public void IntegerLiteralExpression_ExpressionStatementExpressionIsIntegerLiteral(string expression)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
	
		Assert.IsType<IntegerLiteral>(expressionStatement.Expression);
	}

	[Theory]
	[InlineData("5;", 5)]
	public void IntegerLiteralExpression_ReturnsExpectedValue(string expression, int expectedValue)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		IntegerLiteral integerLiteral = (IntegerLiteral)expressionStatement.Expression;

		Assert.Equal(expectedValue, integerLiteral.Value);
	}
}

public class PrefixOperatorTests
{
	[Theory]
	[InlineData("!5;")]
	[InlineData("-15;")]
	private void CheckParserErrors(string expression)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		
		Assert.Empty(parser.Errors());
	}
	
	[Theory]
	[InlineData("!5;", 1)]
	[InlineData("-15;", 1)]
	public void PrefixOperator_ReturnsExpectedStatementCount(string expression, int expectedStatementCount)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();
		
		Assert.Equal(expectedStatementCount, ast.Statements.Count);
	}
	
	[Theory]
	[InlineData("!5;")]
	[InlineData("-15;")]
	public void PrefixOperator_StatementIsExpressionStatement(string expression)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		Assert.IsType<ExpressionStatement>(ast.Statements[0]);
	}
	
	[Theory]
	[InlineData("!5;")]
	[InlineData("-15;")]
	public void PrefixOperator_ExpressionStatementIsPrefixExpression(string expression)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
	
		Assert.IsType<PrefixExpression>(expressionStatement.Expression);
	}
	
	[Theory]
	[InlineData("!5;", "!")]
	[InlineData("-15;", "-")]
	public void PrefixOperator_PrefixExpressionOperatorIsCorrect(string expression, string expectedOperator)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		PrefixExpression prefixExpression = (PrefixExpression)expressionStatement.Expression;

		Assert.Equal(expectedOperator, prefixExpression.Operator);
	}

	[Theory]
	[InlineData("!5;")]
	[InlineData("-15;")]
	public void PrefixOperator_PrefixExpressionRightIsIntegerLiteral(string expression)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		PrefixExpression prefixExpression = (PrefixExpression)expressionStatement.Expression;

		Assert.IsType<IntegerLiteral>(prefixExpression.Right);
	}
	
	[Theory]
	[InlineData("!5;", 5)]
	[InlineData("-15;", 15)]
	public void PrefixOperator_IsCorrectIntegerLiteral(string expression, int expectedLiteral)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		PrefixExpression prefixExpression = (PrefixExpression)expressionStatement.Expression;
		IntegerLiteral integerLiteral = (IntegerLiteral)prefixExpression.Right;

		Assert.Equal(expectedLiteral, integerLiteral.Value);
	}
	
}

public class InfixExpressionTests
{
	[Theory]
	[InlineData("5 + 5;", 1)]
	[InlineData("5 - 5;", 1)]
	[InlineData("5 * 5;", 1)]
	[InlineData("5 / 5;", 1)]
	[InlineData("5 > 5;", 1)]
	[InlineData("5 < 5;", 1)]
	[InlineData("5 == 5;", 1)]
	[InlineData("5 != 5;", 1)]
	public void InfixExpression_ReturnsExpectedStatementCount(string expression, int expectedStatementCount)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();
		
		Assert.Equal(expectedStatementCount, ast.Statements.Count);
	}
	
	[Theory]
	[InlineData("5 + 5;")]
	[InlineData("5 - 5;")]
	[InlineData("5 * 5;")]
	[InlineData("5 / 5;")]
	[InlineData("5 > 5;")]
	[InlineData("5 < 5;")]
	[InlineData("5 == 5;")]
	[InlineData("5 != 5;")]
	public void InfixExpression_ExpressionStatementIsInfixExpression(string expression)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
	
		Assert.IsType<InfixExpression>(expressionStatement.Expression);
	}
	
	[Theory]
	[InlineData("5 + 5;", "+")]
	[InlineData("5 - 5;", "-")]
	[InlineData("5 * 5;", "*")]
	[InlineData("5 / 5;", "/")]
	[InlineData("5 > 5;", ">")]
	[InlineData("5 < 5;", "<")]
	[InlineData("5 == 5;", "==")]
	[InlineData("5 != 5;", "!=")]
	public void InfixExpression_InfixExpressionOperatorIsCorrect(string expression, string expectedOperator)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		InfixExpression infixExpression = (InfixExpression)expressionStatement.Expression;

		Assert.Equal(expectedOperator, infixExpression.Operator);
	}

	[Theory]
	[InlineData("5 + 5;", 5)]
	[InlineData("5 - 5;", 5)]
	[InlineData("5 * 5;", 5)]
	[InlineData("5 / 5;", 5)]
	[InlineData("5 > 5;", 5)]
	[InlineData("5 < 5;", 5)]
	[InlineData("5 == 5;", 5)]
	[InlineData("5 != 5;", 5)]
	public void InfixExpression_LeftOperandIsCorrect(string expression, int expectedLeftOperand)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		InfixExpression infixExpression = (InfixExpression)expressionStatement.Expression;
		IntegerLiteral integerLiteral = (IntegerLiteral)infixExpression.Left;

		Assert.Equal(expectedLeftOperand, integerLiteral.Value);
	}
	
	[Theory]
	[InlineData("5 + 5;", 5)]
	[InlineData("5 - 5;", 5)]
	[InlineData("5 * 5;", 5)]
	[InlineData("5 / 5;", 5)]
	[InlineData("5 > 5;", 5)]
	[InlineData("5 < 5;", 5)]
	[InlineData("5 == 5;", 5)]
	[InlineData("5 != 5;", 5)]
	public void InfixExpression_RightOperandIsCorrect(string expression, int expectedRightOperand)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		InfixExpression infixExpression = (InfixExpression)expressionStatement.Expression;
		IntegerLiteral integerLiteral = (IntegerLiteral)infixExpression.Right;

		Assert.Equal(expectedRightOperand, integerLiteral.Value);
	}
}