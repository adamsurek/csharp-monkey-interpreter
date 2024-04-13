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
	[InlineData("((10 + 10) - 20) / 2", typeof(IntegerObject))]
	[InlineData("true", typeof(BooleanObject))]
	[InlineData("false", typeof(BooleanObject))]
	[InlineData("!true", typeof(BooleanObject))]
	[InlineData("!false", typeof(BooleanObject))]
	[InlineData("!!true", typeof(BooleanObject))]
	[InlineData("!!false", typeof(BooleanObject))]
	[InlineData("!5", typeof(BooleanObject))]
	[InlineData("!!5", typeof(BooleanObject))]
	[InlineData("return 10;", typeof(IntegerObject))]
	[InlineData("return 10; 9;", typeof(IntegerObject))]
	[InlineData("", typeof(NullObject))]
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
	[InlineData("true == true", true)]
	[InlineData("true != false", true)]
	[InlineData("true == false", false)]
	[InlineData("false == true", false)]
	[InlineData("false != true", true)]
	[InlineData("(1 < 2) == true", true)]
	[InlineData("(1 > 2) == false", true)]
	[InlineData("(1 > 2) == true", false)]
	[InlineData("(1 < 2) == false", false)]
	public void BooleanObject_HasExpectedValue(string expression, bool expectedValue)
	{
		Program program = new(expression);
		BooleanObject evaluatedObject = (BooleanObject)AST.Evaluator.Evaluate(program.Ast)!;

		Assert.Equal(expectedValue, evaluatedObject.Value);
	}
}

public class ConditionalExpressionTests
{
	[Theory]
	[InlineData("if (true) { 10 }", 10)]
	[InlineData("if (false) { 10 }", null)]
	[InlineData("if (1) { 10 }", 10)]
	[InlineData("if (1 < 2) { 10 }", 10)]
	[InlineData("if (1 > 2) { 10 }", null)]
	[InlineData("if (1 > 2) { 10 } else { 5 }", 5)]
	public void ConditionalExpression_EvaluatesCorrectExpression(string expression, object? expectedValue)
	{
		Program program = new(expression);
		object? actualValue = AST.Evaluator.Evaluate(program.Ast) switch
		{
			IntegerObject integerObject => integerObject.Value,
			_ => null
		};
		
		Assert.Equal(expectedValue, actualValue);
	}
}

public class ReturnStatementTests
{
	[Theory]
	[InlineData("return 10;", 10)]
	[InlineData("return 10; 9;", 10)]
	[InlineData("return 6 * 2; 9;", 12)]
	[InlineData("15; return 6 * 2; 9;", 12)]
	// [InlineData("if (10 > 1) { if (10 > 1) {return 6 * 2;} return 1; }", 12)]
	public void ReturnStatements_HasExpectedValue(string expression, object expectedValue)
	{
		Program program = new(expression);
		object? actualValue = AST.Evaluator.Evaluate(program.Ast) switch
		{
			IntegerObject integerObject => integerObject.Value,
			_ => null
		};
		
		Assert.Equal(expectedValue, actualValue);
	}
}

public class ErrorHandlingTests
{
	[Theory]
	[InlineData("5 + true","Type mismatch: Integer + Boolean")]
	[InlineData("5 + true; 5;","Type mismatch: Integer + Boolean")]
	[InlineData("-true","Unknown operator: -Boolean")]
	[InlineData("true + false","Unknown operator: Boolean + Boolean")]
	[InlineData("5; true + false; 5;","Unknown operator: Boolean + Boolean")]
	[InlineData("if (10 > 1) { true + false; }","Unknown operator: Boolean + Boolean")]
	[InlineData("if (10 > 1) { if (10 > 1) { return true + false; } return 1; }","Unknown operator: Boolean + Boolean")]
	public void ErrorHandling_OutputsCorrectError(string expression, string expectedError)
	{
		Program program = new(expression);
		ErrorObject errorObject = (ErrorObject)AST.Evaluator.Evaluate(program.Ast);

		Assert.Equal(expectedError, errorObject.Message);
	}
}