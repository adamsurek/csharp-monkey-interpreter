﻿using System.Collections;
using Xunit.Abstractions;
using MonkeyInterpreter.AST;

namespace MonkeyInterpreter.Tests.Parser;

// TODO: Consolidate tests - lots of repetition

public class AstCollection
{
	public Core.Lexer Lexer;
	public AST.Parser Parser;
	public AbstractSyntaxTree Ast;
	
	public AstCollection(string expression)
	{
		Lexer = new Core.Lexer(expression);
		Parser = new AST.Parser(Lexer);
		Ast = Parser.ParseProgram();
	}
}

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

public class BooleanLiteralTests
{
	[Theory]
	[InlineData("true;", 1)]
	public void BooleanLiteral_ReturnsExpectedStatementCount(string expression, int expectedStatementCount)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();
	
		Assert.Equal(expectedStatementCount, ast.Statements.Count);
	}

	[Theory]
	[InlineData("true;")]
	public void BooleanLiteral_StatementIsExpressionStatement(string expression)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();
	
		Assert.IsType<ExpressionStatement>(ast.Statements[0]);
	}

	[Theory]
	[InlineData("true;")]
	public void BooleanLiteral_ExpressionStatementExpressionIsIdentifier(string expression)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		
		Assert.IsType<BooleanLiteral>(expressionStatement.Expression);
	}

	[Theory]
	[InlineData("true;", true)]
	public void BooleanLiteral_ReturnsExpectedValue(string expression, bool expectedValue)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		BooleanLiteral identifier = (BooleanLiteral)expressionStatement.Expression;

		Assert.Equal(expectedValue, identifier.Value);
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

public class OperatorPrecedenceTests
{
	[Theory]
	[InlineData("-a*b","((-a)*b)")]
	[InlineData("!-a","(!(-a))")]
	[InlineData("a+b+c","((a+b)+c)")]
	[InlineData("a+b-c","((a+b)-c)")]
	[InlineData("a*b*c","((a*b)*c)")]
	[InlineData("a*b/c","((a*b)/c)")]
	[InlineData("a+b/c","(a+(b/c))")]
	[InlineData("a+b*c+d/e-f","(((a+(b*c))+(d/e))-f)")]
	[InlineData("3+4;-5*5","(3+4)((-5)*5)")]
	[InlineData("5>4==3<4","((5>4)==(3<4))")]
	[InlineData("5<4!=3>4","((5<4)!=(3>4))")]
	[InlineData("3+4*5==3*1+4*5","((3+(4*5))==((3*1)+(4*5)))")]
	[InlineData("1+(2+3)+4","((1+(2+3))+4)")]
	[InlineData("(5+5)*2","((5+5)*2)")]
	[InlineData("!(true==true)","(!(true==true))")]
	[InlineData("a+add(b*c)+d","((a+add((b*c)))+d)")]
	[InlineData("add(a,b,1,2*3,4+5,add(6,7*8))","add(a, b, 1, (2*3), (4+5), add(6, (7*8)))")]
	[InlineData("add(a+b+c*d/f+g)","add((((a+b)+((c*d)/f))+g))")]
	public void OperatorPrecedence_OperatorsParsedWithCorrectPrecedence(string expression,
		string expectedPrecedence)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();
			
		Assert.Equal(expectedPrecedence, ast.String());
	}
}


public class IfExpressionTests
{
	private ITestOutputHelper _output;
	
	public IfExpressionTests(ITestOutputHelper output)
	{
		_output = output;
	}
	
	[Theory]
	[InlineData("if (x < y) { x }", 1)]
	public void IfExpression_ReturnsExpectedStatementCount(string expression, int expectedStatementCount)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();
		
		Assert.Equal(expectedStatementCount, ast.Statements.Count);
	}
	
	[Theory]
	[InlineData("if (x < y) { x }")]
	[InlineData("if (x < y) { x } else { y }")]
	public void IfExpression_StatementIsExpressionStatement(string expression)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		Assert.IsType<ExpressionStatement>(ast.Statements[0]);
	}
	
	[Theory]
	[InlineData("if (x < y) { x }")]
	public void IfExpression_ExpressionStatementIsIfExpression(string expression)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
	
		Assert.IsType<IfExpression>(expressionStatement.Expression);
	}
	
	[Theory]
	[InlineData("if (x < y) { x }", 1)]
	public void IfExpression_ReturnsExpectedConsequenceStatementCount(string expression, int expectedStatementCount)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		IfExpression ifExpression = (IfExpression)expressionStatement.Expression;

		Assert.Equal(expectedStatementCount, ifExpression.Consequence!.Statements!.Count);
	}
	
	[Theory]
	[InlineData("if (x < y) { x }")]
	public void IfExpression_ConsequenceStatementIsExpressionStatement(string expression)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		IfExpression ifExpression = (IfExpression)expressionStatement.Expression;

		Assert.IsType<ExpressionStatement>(ifExpression.Consequence!.Statements![0]);
	}
	
	[Theory]
	[InlineData("if (x < y) { x }", "x")]
	public void IfExpression_ReturnsExpectedConsequenceExpression(string expression, string expectedConsequenceExpression)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		IfExpression ifExpression = (IfExpression)expressionStatement.Expression;
		ExpressionStatement consequenceExpressionStatement = (ExpressionStatement)ifExpression.Consequence!.Statements![0];

		Assert.Equal(expectedConsequenceExpression, consequenceExpressionStatement.Expression.TokenLiteral());
	}
	
	[Theory]
	[InlineData("if (x < y) { x }")]
	public void IfExpression_ReturnsNullAlternativeExpression(string expression)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		IfExpression ifExpression = (IfExpression)expressionStatement.Expression;
		BlockStatement? alternative = ifExpression.Alternative;

		Assert.Null(alternative);
	}

	[Theory]
	[InlineData("if (x < y) { x } else { y }", "y")]
	public void IfExpression_ReturnsExpectedAlternativeExpression(string expression, string expectedAlernativeStatement)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		IfExpression ifExpression = (IfExpression)expressionStatement.Expression;
		ExpressionStatement alternativeStatement = (ExpressionStatement)ifExpression.Alternative!.Statements[0];
		
		Assert.Equal(expectedAlernativeStatement, alternativeStatement.TokenLiteral());
	}
}

public class FunctionLiteralTests
{
	[Theory]
	[InlineData("fn(x, y) { x + y; }", 1)]
	public void FunctionLiteral_ReturnsExpectedStatementCount(string expression, int expectedStatementCount)
	{	
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();
		
		Assert.Equal(expectedStatementCount, ast.Statements.Count);
	}
	
	[Theory]
	[InlineData("fn(x, y) { x + y; }")]
	public void FunctionLiteral_StatementIsExpressionStatement(string expression)
	{	
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();
		
		Assert.IsType<ExpressionStatement>(ast.Statements[0]);
	}
	
	[Theory]
	[InlineData("fn(x, y) { x + y; }")]
	public void FunctionLiteral_ExpressionIsFunctionLiteral(string expression)
	{	
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		
		Assert.IsType<FunctionLiteral>(expressionStatement.Expression);
	}
	
	[Theory]
	[InlineData("fn(x, y) { x + y; }", 2)]
	public void FunctionLiteral_ReturnsCorrectParamCount(string expression, int expectedParamCount)
	{	
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		FunctionLiteral functionLiteral = (FunctionLiteral)expressionStatement.Expression;
		
		Assert.Equal(expectedParamCount, functionLiteral.Parameters.Count);
	}
	
	[Theory]
	[InlineData("fn(x, y) { x + y; }", 1)]
	public void FunctionLiteral_BodyStatementIsExpressionStatement(string expression, int expectedStatementCount)
	{	
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		FunctionLiteral functionLiteral = (FunctionLiteral)expressionStatement.Expression;
		ExpressionStatement bodyStatement = (ExpressionStatement)functionLiteral.Body.Statements[0];
		
		Assert.IsType<ExpressionStatement>(functionLiteral.Body.Statements[0]);
		
		
	}

	[Theory]
	[InlineData("fn(x, y) { x + y; }", "(x+y)")]
	public void FunctionLiteral_ReturnsExpectedBodyExpression(string expression, string expectedParsedStatement)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		FunctionLiteral functionLiteral = (FunctionLiteral)expressionStatement.Expression;
		ExpressionStatement bodyStatement = (ExpressionStatement)functionLiteral.Body!.Statements[0];
	
		Assert.Equal(expectedParsedStatement, bodyStatement.String());
	}
	
	[Theory]
	[InlineData("fn(x, y) { x + y; }", new[] {"x", "y"})]
	public void FunctionLiteral_ReturnsExpectedParameters(string expression, string[] expectedParsedStatement)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		FunctionLiteral functionLiteral = (FunctionLiteral)expressionStatement.Expression;

		List<string> parameters = new();
		foreach (Identifier identifier in functionLiteral.Parameters)
		{
			parameters.Add(identifier.Value);
		}
		
		Assert.Equal(expectedParsedStatement, parameters.ToArray());
	}
}

public class CallExpressionTests
{
	[Theory]
	[InlineData("add(1, 2 * 3, 4 + 5);", 1)]
	public void CallExpression_ReturnsExpectedStatementCount(string expression, int expectedStatementCount)
	{	
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();
		
		Assert.Equal(expectedStatementCount, ast.Statements.Count);
	}
	
	[Theory]
	[InlineData("add(1, 2 * 3, 4 + 5);")]
	public void CallExpression_StatementIsExpressionStatement(string expression)
	{	
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();
		
		Assert.IsType<ExpressionStatement>(ast.Statements[0]);
	}
	
	[Theory]
	[InlineData("add(1, 2 * 3, 4 + 5);")]
	public void CallExpression_ExpressionIsCallExpression(string expression)
	{	
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		
		Assert.IsType<CallExpression>(expressionStatement.Expression);
	}
	
	[Theory]
	[InlineData("add(1, 2 * 3, 4 + 5);", "add")]
	public void CallExpression_ReturnsExpectedFunctionIdentifier(string expression, string expectedIdentifier)
	{	
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		CallExpression callExpression = (CallExpression)expressionStatement.Expression;
		
		Assert.Equal(expectedIdentifier, callExpression.Function.String());
	}
	
	[Theory]
	[InlineData("add(1, 2 * 3, 4 + 5);", 3)]
	public void CallExpression_ReturnsCorrectParamCount(string expression, int expectedParamCount)
	{	
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		CallExpression callExpression = (CallExpression)expressionStatement.Expression;
		
		Assert.Equal(expectedParamCount, callExpression.Arguments.Count);
	}

	[Theory]
	[InlineData("add(1, 2 * 3, 4 + 5);", new[] {"1", "(2*3)", "(4+5)"})]
	public void CallExpression_ReturnsExpectedParameters(string expression, string[] expectedParsedStatement)
	{
		Core.Lexer lexer = new(expression);
		AST.Parser parser = new(lexer);
		AbstractSyntaxTree ast = parser.ParseProgram();

		ExpressionStatement expressionStatement = (ExpressionStatement)ast.Statements[0];
		CallExpression callExpression = (CallExpression)expressionStatement.Expression;

		List<string> parameters = new();
		foreach (IExpression argument in callExpression.Arguments)
		{
			parameters.Add(argument.String());
		}
		
		Assert.Equal(expectedParsedStatement, parameters.ToArray());
	}
}