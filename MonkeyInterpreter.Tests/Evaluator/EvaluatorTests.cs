using System.Data;
using MonkeyInterpreter.AST;
using MonkeyInterpreter.Core;
using Xunit.Abstractions;
using Boolean = MonkeyInterpreter.Core.Boolean;

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
	[InlineData("true", typeof(Boolean))]
	[InlineData("false", typeof(Boolean))]
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
	public void IntegerObject_HasExpectedValue(string expression, int expectedValue)
	{
		Program program = new(expression);
		Integer evaluatedObject = (Integer)AST.Evaluator.Evaluate(program.Ast)!;

		Assert.Equal(expectedValue, evaluatedObject.Value);
	}
}

public class BooleanEvaluationTests
{
	[Theory]
	[InlineData("true", true)]
	[InlineData("false", false)]
	[InlineData("!true", false)]
	[InlineData("!false", true)]
	[InlineData("!!true", true)]
	[InlineData("!!false", false)]
	[InlineData("!5", false)]
	[InlineData("!!5", true)]
	public void BooleanObject_HasExpectedValue(string expression, bool expectedValue)
	{
		Program program = new(expression);
		Boolean evaluatedObject = (Boolean)AST.Evaluator.Evaluate(program.Ast)!;

		Assert.Equal(expectedValue, evaluatedObject.Value);
	}
	
	// [Theory]
	// [InlineData("!true", false)]
	// [InlineData("!false", true)]
	// [InlineData("!!true", true)]
	// [InlineData("!!false", false)]
	// [InlineData("!5", false)]
	// [InlineData("!!5", true)]
	// public void BooleanObject_BangOperator_ReturnsExpectedValue(string expression, bool expectedValue)
	// {
	// 	Program program = new(expression);
	// 	Boolean evaluatedObject = (Boolean)AST.Evaluator.Evaluate(program.Ast)!;
	//
	// 	Assert.Equal(expectedValue, evaluatedObject.Value);
	// }
	
	
}