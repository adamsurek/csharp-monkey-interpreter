using System.Data;
using MonkeyInterpreter.AST;
using MonkeyInterpreter.Core;
using Xunit.Abstractions;

namespace MonkeyInterpreter.Tests.Evaluator;

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
	[InlineData("5", typeof(Integer))]
	[InlineData("10", typeof(Integer))]
	public void EvalResult_IsExpectedObjectType(string expression, Type expectedType)
	{
		Program program = new(expression);
		IObject evaluatedObject = AST.Evaluator.Evaluate(program.Ast)!;

		Assert.IsType(expectedType, evaluatedObject);
	}
}

public class IntegerEvaluationTests
{
	[Theory]
	[InlineData("5", 5)]
	[InlineData("10", 10)]
	public void EvalResult_HasExpectedValue(string expression, int expectedValue)
	{
		Program program = new(expression);
		Integer evaluatedObject = (Integer)AST.Evaluator.Evaluate(program.Ast)!;

		Assert.Equal(expectedValue, evaluatedObject.Value);
	}
}