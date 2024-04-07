using MonkeyInterpreter.AST;
using MonkeyInterpreter.Core;

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
	[InlineData("5", typeof(IntegerObject))]
	[InlineData("10", typeof(IntegerObject))]
	[InlineData("true", typeof(BooleanObject))]
	[InlineData("false", typeof(BooleanObject))]
	[InlineData("!true", typeof(BooleanObject))]
	[InlineData("!false", typeof(BooleanObject))]
	[InlineData("!!true", typeof(BooleanObject))]
	[InlineData("!!false", typeof(BooleanObject))]
	[InlineData("!5", typeof(BooleanObject))]
	[InlineData("!!5", typeof(BooleanObject))]
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
	[InlineData("-5", -5)]
	[InlineData("-10", -10)]
	[InlineData("5 - 5 - 5 - 5 - 10", -20)]
	[InlineData("5 + 5 + 5 + 5 - 10", 10)]
	[InlineData("2 * 2 * 2", 8)]
	[InlineData("5 + 5 * 10", 55)]
	[InlineData("-50 + 100 - 50", 0)]
	[InlineData("5 / 5 * 10", 10)]
	[InlineData("-2 * 3 / 6", -1)]
	[InlineData("(1 + 3) / 2", 2)]
	[InlineData("((10 + 10) - 20) / 2", 0)]
	public void IntegerObject_HasExpectedValue(string expression, int expectedValue)
	{
		Program program = new(expression);
		IntegerObject evaluatedObject = (IntegerObject)AST.Evaluator.Evaluate(program.Ast)!;

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
	[InlineData("1 < 2", true)]
	[InlineData("1 > 2", false)]
	[InlineData("1 < 1", false)]
	[InlineData("1 == 1", true)]
	[InlineData("1 != 1", false)]
	public void BooleanObject_HasExpectedValue(string expression, bool expectedValue)
	{
		Program program = new(expression);
		BooleanObject evaluatedObject = (BooleanObject)AST.Evaluator.Evaluate(program.Ast)!;

		Assert.Equal(expectedValue, evaluatedObject.Value);
	}
}