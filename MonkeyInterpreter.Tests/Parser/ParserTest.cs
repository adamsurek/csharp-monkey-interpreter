﻿using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using MonkeyInterpreter.Core.AbstractSyntaxTree;
using Xunit.Abstractions;

namespace MonkeyInterpreter.Tests.Parser;

public class Program
{
	public Core.Parser.Lexer Lexer;
	public Core.Parser.Parser Parser;
	public AbstractSyntaxTree Ast;
	
	public Program(string expression)
	{
		Lexer = new Core.Parser.Lexer(expression);
		Parser = new Core.Parser.Parser(Lexer);
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
	[InlineData("fn(x, y) { x + y; }", 1)]
	[InlineData("add(1, 2 * 3, 4 + 5);", 1)]
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
	[InlineData("fn(x, y) { x + y; }")]
	[InlineData("add(1, 2 * 3, 4 + 5);")]
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
	[InlineData("\"Hello world\";", typeof(StringLiteral))]
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
	[InlineData("fn(x, y) { x + y; }", typeof(FunctionLiteral))]
	[InlineData("add(1, 2 * 3, 4 + 5);", typeof(CallExpression))]
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

public class StringLiteralTests
{
	[Theory]
	[InlineData("\"Hello world\";", "Hello world")]
	public void StringLiteral_ReturnsExpectedValue(string expression, string expectedValue)
	{
		Program program = new(expression);
		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();

		Assert.All(expressionStatements,
			expressionStatement => Assert.Equal(expectedValue, ((StringLiteral)expressionStatement.Expression).Value));
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
	[Theory]
	[InlineData("if (x < y) { x }")]
	public void IfExpression_ConsequenceStatementsAreExpressionStatements(string expression)
	{
		Program program = new(expression);

		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();
		List<IfExpression> ifExpressions = expressionStatements
			.Select(statement => (IfExpression)statement.Expression).ToList();
		List<BlockStatement> consequences = ifExpressions
			.Select(statement => statement.Consequence!).ToList();
		List<List<IStatement>> consequenceStatements = consequences
			.Select(statement => statement.Statements).ToList();
		
		
		Assert.All(consequenceStatements,
			consequenceStatement => Assert.All(consequenceStatement,
				statement => Assert.IsType<ExpressionStatement>(statement) ));
	}
	
	[Theory]
	[InlineData("if (x < y) { x }", "x")]
	public void IfExpression_ReturnsExpectedConsequenceExpression(string expression, string expectedConsequenceExpression)
	{
		Program program = new(expression);

		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();
		List<IfExpression> ifExpressions = expressionStatements
			.Select(statement => (IfExpression)statement.Expression).ToList();
		List<BlockStatement> consequences = ifExpressions
			.Select(statement => statement.Consequence!).ToList();
		List<List<IStatement>> consequenceStatements = consequences
			.Select(statement => statement.Statements).ToList();
		
		
		Assert.All(consequenceStatements,
			consequenceStatement => Assert.All(consequenceStatement,
				statement => Assert.Equal(expectedConsequenceExpression, statement.TokenLiteral())));
	}
	
	[Theory]
	[InlineData("if (x < y) { x }")]
	public void IfExpression_ReturnsNullAlternativeExpression(string expression)
	{
		Program program = new(expression);

		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();
		List<IfExpression> ifExpressions = expressionStatements
			.Select(statement => (IfExpression)statement.Expression).ToList();
		List<BlockStatement?> alternatives = ifExpressions
			.Select(statement => statement.Alternative).ToList();

		Assert.All(alternatives,
			Assert.Null);
	}

	[Theory]
	[InlineData("if (x < y) { x } else { y }", "y")]
	public void IfExpression_ReturnsExpectedAlternativeExpression(string expression, string expectedAlternativeStatement)
	{
		Program program = new(expression);

		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();
		List<IfExpression> ifExpressions = expressionStatements
			.Select(statement => (IfExpression)statement.Expression).ToList();
		List<BlockStatement> alternatives = ifExpressions
			.Select(statement => statement.Alternative!).ToList();
		List<List<IStatement>> alternativeStatements = alternatives
			.Select(statement => statement.Statements).ToList();

		Assert.All(alternativeStatements,
			consequenceStatement => Assert.All(consequenceStatement,
				statement => Assert.Equal(expectedAlternativeStatement, statement.TokenLiteral())));
	}
}

public class FunctionLiteralTests
{
	[Theory]
	[InlineData("fn(x, y) { x + y; }", 2)]
	public void FunctionLiteral_ReturnsCorrectParamCount(string expression, int expectedParamCount)
	{	
		Program program = new(expression);

		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();
		List<FunctionLiteral> functionLiterals = expressionStatements
			.Select(statement => (FunctionLiteral)statement.Expression).ToList();
		
		Assert.All(functionLiterals,
			functionLiteral => Assert.Equal(expectedParamCount, functionLiteral.Parameters!.Count));
	}
	
	[Theory]
	[InlineData("fn(x, y) { x + y; }")]
	public void FunctionLiteral_BodyStatementIsExpressionStatement(string expression)
	{	
		Program program = new(expression);

		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();
		List<FunctionLiteral> functionLiterals = expressionStatements
			.Select(statement => (FunctionLiteral)statement.Expression).ToList();
		List<List<IStatement>> bodyStatements = functionLiterals
			.Select(statement => statement.Body!.Statements).ToList();

		Assert.All(bodyStatements,
			bodyStatement => Assert.All(bodyStatement,
				statement => Assert.IsType<ExpressionStatement>(statement)));
	}

	[Theory]
	[InlineData("fn(x, y) { x + y; }", "(x+y)")]
	public void FunctionLiteral_ReturnsExpectedBodyExpression(string expression, string expectedParsedStatement)
	{
		Program program = new(expression);

		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();
		List<FunctionLiteral> functionLiterals = expressionStatements
			.Select(statement => (FunctionLiteral)statement.Expression).ToList();
		List<List<IStatement>> bodyStatements = functionLiterals
			.Select(statement => statement.Body!.Statements).ToList();

		Assert.All(bodyStatements,
			bodyStatement => Assert.All(bodyStatement,
				statement => Assert.Equal(expectedParsedStatement, statement.String())));
	}
	
	[Theory]
	[InlineData("fn(x, y) { x + y; }", new[] {"x", "y"})]
	public void FunctionLiteral_ReturnsExpectedParameters(string expression, string[] expectedParameters)
	{
		Program program = new(expression);

		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();
		List<FunctionLiteral> functionLiterals = expressionStatements
			.Select(statement => (FunctionLiteral)statement.Expression).ToList();
		List<List<Identifier>?> identifierParameters = functionLiterals
			.Select(literals => literals.Parameters).ToList();

		Assert.All(identifierParameters,
			identifier => Assert.Equal(expectedParameters, identifier
				.Select(param => param.TokenLiteral())));
	}
}

public class CallExpressionTests
{
	[Theory]
	[InlineData("add(1, 2 * 3, 4 + 5);", "add")]
	public void CallExpression_ReturnsExpectedFunctionIdentifier(string expression, string expectedIdentifier)
	{	
		Program program = new(expression);

		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();
		List<CallExpression> callExpressions = expressionStatements
			.Select(statement => (CallExpression)statement.Expression).ToList();
		
		Assert.All(callExpressions,
			callExpression => Assert.Equal(expectedIdentifier, callExpression.Function.String()));
	}
	
	[Theory]
	[InlineData("add(1, 2 * 3, 4 + 5);", 3)]
	public void CallExpression_ReturnsCorrectParamCount(string expression, int expectedParamCount)
	{	
		Program program = new(expression);

		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();
		List<CallExpression> callExpressions = expressionStatements
			.Select(statement => (CallExpression)statement.Expression).ToList();
	
		Assert.All(callExpressions, 
			callExpression => Assert.Equal(expectedParamCount, callExpression.Arguments!.Count));
	}

	[Theory]
	[InlineData("add(1, 2 * 3, 4 + 5);", new[] {"1", "(2*3)", "(4+5)"})]
	public void CallExpression_ReturnsExpectedParameters(string expression, string[] expectedParameters)
	{
		Program program = new(expression);

		List<ExpressionStatement> expressionStatements = program.Ast.Statements
			.Select(statement => (ExpressionStatement)statement).ToList();
		List<CallExpression> callExpressions = expressionStatements
			.Select(statement => (CallExpression)statement.Expression).ToList();

		Assert.All(callExpressions,
			callExpression => Assert.Equal(expectedParameters, callExpression.Arguments!
				.Select(argument => argument.String())));
	}
}