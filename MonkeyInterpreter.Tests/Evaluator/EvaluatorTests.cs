using MonkeyInterpreter.Core.AbstractSyntaxTree;
using MonkeyInterpreter.Core.Evaluator;

namespace MonkeyInterpreter.Tests.Evaluator;

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
	[InlineData("fn(x) { x + 2; }; ", typeof(FunctionObject))]
	public void EvalResult_IsExpectedObjectType(string expression, Type expectedType)
	{
		Program program = new(expression);
		VariableEnvironment env = new();
		IObject evaluatedObject = Core.Evaluator.Evaluator.Evaluate(program.Ast, env);

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
	[InlineData("let a = 5; a;", 5)]
	[InlineData("let a = 5 * 5; a;", 25)]
	[InlineData("let a = 5; let b = a; b;", 5)]
	[InlineData("let a = 5; let b = a; let c = a + b + 5; c;", 15)]
	[InlineData("let identity = fn(x) { x; }; identity(5);", 5)]
    [InlineData("let identity = fn(x) { return x; }; identity(5);", 5)]
    [InlineData("let double = fn(x) { x * 2; }; double(5);", 10)]
    [InlineData("let add = fn(x, y) { x + y; }; add(5, 5);", 10)]
    [InlineData("let add = fn(x, y) { x + y; }; add(5 + 5, add(5, 5));", 20)]
    [InlineData("fn(x) { x; }(5)", 5)]
    [InlineData("let add = fn(x) { fn(y) { x + y }; }; let wrapper = add(2); wrapper(2);", 4)]
	public void IntegerObject_HasExpectedValue(string expression, int expectedValue)
	{
		Program program = new(expression);
		VariableEnvironment env = new();
		IntegerObject evaluatedObject = (IntegerObject)Core.Evaluator.Evaluator.Evaluate(program.Ast, env);

		Assert.Equal(expectedValue, evaluatedObject.Value);
	}
}

public class StringLiteralTests
{
	[Theory]
	[InlineData("\"Hello world\"", "Hello world")]
	[InlineData("let a = \"foobar\"; a;", "foobar")]
	[InlineData("let identity = fn(x) { \"foobar\"; }; identity(5);", "foobar")]
	[InlineData("\"Hello\" + \" \" + \"world\"", "Hello world")]
	public void StringObject_HasExpectedValue(string expression, string expectedValue)
	{
		Program program = new(expression);
		VariableEnvironment env = new();
		StringObject evaluatedObject = (StringObject)Core.Evaluator.Evaluator.Evaluate(program.Ast, env);

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
		VariableEnvironment env = new();
		BooleanObject evaluatedObject = (BooleanObject)Core.Evaluator.Evaluator.Evaluate(program.Ast, env);

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
	[InlineData("if (1 < 2) { \"hello\" } else { \"world\" }", "hello")]
	[InlineData("if (1 > 2) { \"hello\" } else { \"world\" }", "world")]
	public void ConditionalExpression_EvaluatesCorrectExpression(string expression, object? expectedValue)
	{
		Program program = new(expression);
		VariableEnvironment env = new();
		object? actualValue = Core.Evaluator.Evaluator.Evaluate(program.Ast, env) switch
		{
			IntegerObject integerObject => integerObject.Value,
			StringObject stringObject => stringObject.Value,
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
	[InlineData("if (10 > 1) { if (10 > 1) {return 6 * 2;} return 1; }", 12)]
	[InlineData("return \"hello world\";", "hello world")]
	public void ReturnStatements_HasExpectedValue(string expression, object expectedValue)
	{
		Program program = new(expression);
		VariableEnvironment env = new();
		object? actualValue = Core.Evaluator.Evaluator.Evaluate(program.Ast, env) switch
		{
			IntegerObject integerObject => integerObject.Value,
			StringObject stringObject => stringObject.Value,
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
	[InlineData("foobar", "Identifier not found: foobar")]
	[InlineData("\"Hello\" - \"world\"", "Unknown operator: String - String")]
	[InlineData("len(1)", "Argument to 'len' not supported. Expected String but got Integer")]
	[InlineData("len(\"one\", \"two\")", "Wrong number of arguments. Got 2, expected 1")]
	public void ErrorHandling_OutputsCorrectError(string expression, string expectedError)
	{
		Program program = new(expression);
		VariableEnvironment env = new();
		ErrorObject errorObject = (ErrorObject)Core.Evaluator.Evaluator.Evaluate(program.Ast, env);

		Assert.Equal(expectedError, errorObject.Message);
	}
}

public class FunctionObjectTests
{
	[Theory]
	[InlineData("fn(x) { x + 2; };", 1)]
	[InlineData("fn(x, y) { x + y; };", 2)]
	[InlineData("fn() { 1 + 2; };", 0)]
	public void FunctionObject_HasExpectedParameterCount(string expression, int expectedParameterCounter)
	{
		Program program = new(expression);
		VariableEnvironment env = new();
		FunctionObject functionObject = (FunctionObject)Core.Evaluator.Evaluator.Evaluate(program.Ast, env);
		
		Assert.Equal(expectedParameterCounter, functionObject.Parameters.Count);
	}
	
	[Theory]
	[InlineData("fn(x) { x + 2; };", new[] { "x" })]
	[InlineData("fn(x, y) { x + y; };", new[] {"x", "y"})]
	[InlineData("fn() { 1 + 2; };", new string[] {})]
	public void FunctionObject_HasExpectedParameterNames(string expression, string[]? expectedParameterNames)
	{
		Program program = new(expression);
		VariableEnvironment env = new();
		FunctionObject functionObject = (FunctionObject)Core.Evaluator.Evaluator.Evaluate(program.Ast, env);

		List<string> identifiers = functionObject.Parameters.Select(identifier => identifier.TokenLiteral()).ToList();
		
		Assert.Equal(expectedParameterNames, identifiers);
	}
	
	[Theory]
	[InlineData("fn(x) { x + 2; };", "(x+2)")]
	[InlineData("fn(x, y) { x + y; };", "(x+y)")]
	[InlineData("fn() { 1 + 2; };", "(1+2)")]
	public void FunctionObject_HasExpectedBody(string expression, string expectedBody)
	{
		Program program = new(expression);
		VariableEnvironment env = new();
		FunctionObject functionObject = (FunctionObject)Core.Evaluator.Evaluator.Evaluate(program.Ast, env);

		Assert.Equal(expectedBody, functionObject.Body.String());
	}
}

public class BuiltInFunctionTests
{
	[Theory]
	[InlineData("len(\"\")", 0)]
	[InlineData("len(\"hello world\")", 11)]
	[InlineData("len(\"test\")", 4)]
	public void BuiltInFunction_ReturnsExpectedValue(string expression, object expectedValue)
	{
		Program program = new(expression);
		VariableEnvironment env = new();
		object? actualValue = Core.Evaluator.Evaluator.Evaluate(program.Ast, env) switch
		{
			IntegerObject integerObject => integerObject.Value,
			StringObject stringObject => stringObject.Value,
			_ => null
		};
		
		Assert.Equal(expectedValue, actualValue);
	}
}

public class ArrayLiteralTests
{
	[Theory]
	[InlineData("[0]", 1)]
	[InlineData("[1, 2 * 2, 3 + 3]", 3)]
	public void ArrayObject_HasExpectedElementCount(string expression, int expectedElementCount)
	{
		Program program = new(expression);
		VariableEnvironment env = new();
		ArrayObject arrayLiteral = (ArrayObject)Core.Evaluator.Evaluator.Evaluate(program.Ast, env);
		
		Assert.Equal(expectedElementCount, arrayLiteral.Elements.Count);

	}
	
	[Theory]
	[InlineData("[0]" , new object[] { 0 })]
	[InlineData("[1, 2 * 2, 3 + 3]", new object[] { 1, 4, 6})]
	public void ArrayObject_ElementsEvaluateAsExpected(string expression, object[] expectedElementValues)
	{
		Program program = new(expression);
		VariableEnvironment env = new();
		ArrayObject arrayLiteral = (ArrayObject)Core.Evaluator.Evaluator.Evaluate(program.Ast, env);

		List<object> actualValues = new();

		for (int i = 0; i < arrayLiteral.Elements.Count; i++)
		{
			switch (arrayLiteral.Elements[i])
			{
				case IntegerObject integerObject:
					actualValues.Add(integerObject.Value);
					break;
				
				case StringObject stringObject:
					actualValues.Add(stringObject.Value);
					break;
				
				case BooleanObject booleanObject:
					actualValues.Add(booleanObject.Value);
					break;
				
				default:
					Assert.Fail("Invalid Element object type");
					break;
			}
		}
		
		Assert.Equal(expectedElementValues, actualValues);

	}
}