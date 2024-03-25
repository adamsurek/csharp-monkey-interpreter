﻿using System.Collections;
using Xunit.Abstractions;
using MonkeyInterpreter.AST;

namespace MonkeyInterpreter.Tests.Parser;

// TODO: Consolidate tests - lots of repetition

public class Program
{
	public Core.Lexer Lexer;
	public AST.Parser Parser;
	public AbstractSyntaxTree Ast;
	
	public Program(string expression)
	{
		Lexer = new Core.Lexer(expression);
		Parser = new AST.Parser(Lexer);
		Ast = Parser.ParseProgram();
	}
}

public class GenericTests
{
	[Theory]
	[InlineData("")]
	public void AbstractSyntaxTree_Errors_IsEmpty(string expression)
	{
		Program program = new(expression);
		
		Assert.Empty(program.Parser.Errors());
	}
	
	[Theory]
	[InlineData("let x y z")]
	public void AbstractSyntaxTree_Errors_IsNotEmpty(string expression)
	{
		Program program = new(expression);
		
		Assert.NotEmpty(program.Parser.Errors());
	}
	
	[Theory]
	[InlineData("", 0)]
	[InlineData("foobar;", 1)]
	[InlineData("5;", 1)]
	[InlineData("!5;", 1)]
	[InlineData("-15;", 1)]
	[InlineData("5 + 5;", 1)]
	[InlineData("5 - 5;", 1)]
	[InlineData("5 * 5;", 1)]
	[InlineData("5 / 5;", 1)]
	[InlineData("5 > 5;", 1)]
	[InlineData("5 < 5;", 1)]
	[InlineData("5 == 5;", 1)]
	[InlineData("5 != 5;", 1)]
	[InlineData("if (x < y) { x }", 1)]
	public void AbstractSyntaxTree_Statements_CountIsExpected(string expression, int expectedStatementCount)
	{
		Program program = new(expression);
		
		Assert.Equal(expectedStatementCount, program.Ast.Statements.Count);
	}
	
	[Theory]
	[InlineData("let x;")]
	[InlineData("foobar;")]
	[InlineData("5;")]
	[InlineData("!5;")]
	[InlineData("-15;")]
	public void AbstractSyntaxTree_Statements_AreExpressionStatements(string expression)
	{
		Program program = new(expression);
		
		Assert.All(program.Ast.Statements, statement => Assert.IsType<ExpressionStatement>(statement));
	}

	[Theory]
	[InlineData("let x = 5;", typeof(LetStatement))]
	[InlineData("return 5;", typeof(ReturnStatement))]
	public void AbstractSyntaxTree_Statements_AreExpectedTypes(string expression, Type expectedType)
	{
		Program program = new(expression);
		
		Assert.All(program.Ast.Statements, 
			statement => Assert.IsType(expectedType, statement));
	}
	
	[Theory]
	[InlineData("5;", typeof(IntegerLiteral))]
	[InlineData("true;", typeof(BooleanLiteral))]
	[InlineData("foobar;", typeof(Identifier))]
	[InlineData("!5;", typeof(PrefixExpression))]
	[InlineData("-15;", typeof(PrefixExpression))]
	[InlineData("5 + 5;", typeof(InfixExpression))]
	[InlineData("5 - 5;", typeof(InfixExpression))]
	[InlineData("5 * 5;", typeof(InfixExpression))]
	[InlineData("5 / 5;", typeof(InfixExpression))]
	[InlineData("5 > 5;", typeof(InfixExpression))]
	[InlineData("5 < 5;", typeof(InfixExpression))]
	[InlineData("5 == 5;", typeof(InfixExpression))]
	[InlineData("5 != 5;", typeof(InfixExpression))]
	[InlineData("if (x < y) { x }", typeof(IfExpression))]
	[InlineData("if (x < y) { x } else { y }", typeof(IfExpression))]
	public void AbstractSyntaxTree_Expressions_AreExpectedTypes(string expression, Type expectedType)
	{
		Program program = new(expression);
		
		Assert.All(program.Ast.Statements, 
			statement => Assert.IsType(expectedType, ((ExpressionStatement)statement).Expression));
	}
}

public class LetStatementTests
{
	private readonly ITestOutputHelper _output;
	public LetStatementTests(ITestOutputHelper output)
	{
		_output = output;
	}
	
	[Theory]
	[InlineData("let x = 5;", "x")]
	[InlineData("let y = true;", "y")]
	[InlineData("let foobar = '\0';", "foobar")]
	public void LetStatement_SingleStatement_IsCompleteLetStatement(string expression, string name)
	{
		Program program = new(expression);
		List<IStatement> statements = program.Ast.Statements;
		
		Assert.Multiple(
			() => Assert.All(statements, statement => Assert.IsType<LetStatement>(statement)),
			() => Assert.All(statements, statement => Assert.Equal("let", statement.TokenLiteral())),
			() => Assert.All(statements, statement => Assert.Equal(name, ((LetStatement)statement).Name.Value)),
			() => Assert.All(statements, statement => Assert.Equal(name, ((LetStatement)statement).Name.TokenLiteral()))
			);
	}
	
	[Theory]
	[InlineData("let x = 5;\nlet y = a;\nlet foobar = true;", "x", "y", "foobar")]
	public void LetStatement_MultipleStatements_AreCompleteLetStatements(string expression, params string[] names)
	{
		Program program = new(expression);
		List<IStatement> statements = program.Ast.Statements;
		List<Identifier> identifiers = statements.Select(statement => ((LetStatement)statement).Name).ToList();
		
		Assert.Multiple(
			() => Assert.All(statements, statement => Assert.IsType<LetStatement>(statement)),
			() => Assert.All(statements, statement => Assert.Equal("let", ((LetStatement)statement).TokenLiteral())),
			() => Assert.Equal(identifiers.Select(ident => ident.Value), names),
			() => Assert.Equal(identifiers.Select(ident => ident.TokenLiteral()), names)
			);
	}
}

public class ReturnStatementTest
{
	[Theory]
	[InlineData("return 5;", "5")]
	[InlineData("return true;", "true")]
	[InlineData("return x;", "x")]
	public void LetStatement_SingleStatement_IsCompleteLetStatement(string expression, object expectedReturnValue)
	{
		Program program = new(expression);
		List<IStatement> statements = program.Ast.Statements;
		
		Assert.Multiple(
			() => Assert.All(statements, statement => Assert.IsType<ReturnStatement>(statement)),
			() => Assert.All(statements, statement => Assert.Equal("return", statement.TokenLiteral())),
			() => Assert.All(statements, statement => Assert.Equal(expectedReturnValue, ((ReturnStatement)statement).ReturnValue.TokenLiteral()))
			);
	}
}

public class IdentifierExpressionTests
{
	[Theory]
	[InlineData("foobar;", "foobar")]
	public void IdentifierExpression_Statements_ReturnExpectedValues(string expression, string expectedValue)
	{
		Program program = new(expression);
		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();

		Assert.All(expressionStatements,
			expressionStatement => Assert.Equal(expectedValue, ((Identifier)expressionStatement.Expression).Value));
	}
}

public class IntegerLiteralExpressionTests
{
	[Theory]
	[InlineData("5;", 5)]
	public void IntegerLiteralExpression_ReturnsExpectedValue(string expression, int expectedValue)
	{
		Program program = new(expression);
		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();

		Assert.All(expressionStatements,
			expressionStatement => Assert.Equal(expectedValue, ((IntegerLiteral)expressionStatement.Expression).Value));
	}
}

public class BooleanLiteralTests
{
	[Theory]
	[InlineData("true;", true)]
	public void BooleanLiteral_ReturnsExpectedValue(string expression, bool expectedValue)
	{
		Program program = new(expression);
		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();

		Assert.All(expressionStatements,
			expressionStatement => Assert.Equal(expectedValue, ((BooleanLiteral)expressionStatement.Expression).Value));
	}
	
}

public class PrefixOperatorTests
{
	[Theory]
	[InlineData("!5;", "!")]
	[InlineData("-15;", "-")]
	public void PrefixOperator_Expression_OperatorIsCorrect(string expression, string expectedOperator)
	{
		Program program = new(expression);
		
		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();
		
		Assert.All(expressionStatements,
			expressionStatement => Assert.Equal(expectedOperator,
				((PrefixExpression)expressionStatement.Expression).Operator));
	}

	[Theory]
	[InlineData("!5;")]
	[InlineData("-15;")]
	public void PrefixOperator_PrefixExpressionRightIsIntegerLiteral(string expression)
	{
		Program program = new(expression);
		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();

		Assert.All(expressionStatements,
			expressionStatement =>
				Assert.IsType<IntegerLiteral>(((PrefixExpression)expressionStatement.Expression).Right));
	}
	
	[Theory]
	[InlineData("!5;", 5)]
	[InlineData("-15;", 15)]
	public void PrefixOperator_IsCorrectIntegerLiteral(string expression, int expectedLiteral)
	{
		Program program = new(expression);
		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();
		List<PrefixExpression> prefixExpressions =
			expressionStatements.Select(statement => ((PrefixExpression)statement.Expression)).ToList();

		Assert.All(prefixExpressions,
			prefixExpression => Assert.Equal(expectedLiteral, ((IntegerLiteral)prefixExpression.Right).Value));
	}
	
}

public class InfixExpressionTests
{
	[Theory]
	[InlineData("5 + 5;", "+")]
	[InlineData("5 - 5;", "-")]
	[InlineData("5 * 5;", "*")]
	[InlineData("5 / 5;", "/")]
	[InlineData("5 > 5;", ">")]
	[InlineData("5 < 5;", "<")]
	[InlineData("5 == 5;", "==")]
	[InlineData("5 != 5;", "!=")]
	public void InfixExpression_Expression_OperatorIsCorrect(string expression, string expectedOperator)
	{
		Program program = new(expression);
		
		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();
		
		Assert.All(expressionStatements,
			expressionStatement => Assert.Equal(expectedOperator,
				((InfixExpression)expressionStatement.Expression).Operator));
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
	public void InfixExpression_LeftOperandIsCorrect(string expression, int expectedLiteral)
	{
		Program program = new(expression);

		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();
		List<InfixExpression> infixExpressions =
			expressionStatements.Select(statement => ((InfixExpression)statement.Expression)).ToList();
		
		Assert.All(infixExpressions,
			infixExpression => Assert.Equal(expectedLiteral, ((IntegerLiteral)infixExpression.Left).Value));
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
	public void InfixExpression_RightOperandIsCorrect(string expression, int expectedLiteral)
	{
		Program program = new(expression);

		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();
		List<InfixExpression> infixExpressions =
			expressionStatements.Select(statement => ((InfixExpression)statement.Expression)).ToList();
		
		Assert.All(infixExpressions,
			infixExpression => Assert.Equal(expectedLiteral, ((IntegerLiteral)infixExpression.Right).Value));
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
	public void OperatorPrecedence_OperatorsParsedWithCorrectPrecedence(string expression, string expectedPrecedence)
	{
		Program program = new(expression);
			
		Assert.Equal(expectedPrecedence, program.Ast.String());
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
	[InlineData("if (x < y) { x }")]
	public void IfExpression_ConsequenceStatementIsExpressionStatement(string expression)
	{
		Program program = new(expression);

		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();
		List<IfExpression> ifExpressions = expressionStatements
			.Select(statement => (IfExpression)statement.Expression).ToList();
		List<IStatement> consequenceStatements = ifExpressions
			.Select(statement => (IStatement)statement.Consequence!.Statements).ToList();
		
		Assert.All(consequenceStatements,
			consequenceStatement => Assert.IsType<ExpressionStatement>(consequenceStatement));
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
	[InlineData("fn(x, y) { x + y; }")]
	public void FunctionLiteral_BodyStatementIsExpressionStatement(string expression)
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