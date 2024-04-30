using MonkeyInterpreter.Core.Evaluator;

namespace MonkeyInterpreter.Tests.Evaluator;

public class HashKeyTests
{
	[Theory]
	[InlineData("Test", "Test")]
	[InlineData(1, 1)]
	[InlineData(true, true)]
	[InlineData(false, false)]
	public void HashKey_HashKeysAreEquivalent(object firstExpression, object secondExpression)
	{
		HashKey? firstHashKey = null;
		HashKey? secondHashKey = null;

		switch (firstExpression)
		{
			case string stringExpression:
				StringObject stringObject = new(stringExpression);
				firstHashKey = stringObject.HashKey();
				break;

			case int integerExpression:
				IntegerObject integerObject = new(integerExpression);
				firstHashKey = integerObject.HashKey();
				break;

			case bool booleanExpression:
				BooleanObject booleanObject = new(booleanExpression);
				firstHashKey = booleanObject.HashKey();
				break;
			
			default:
				Assert.Fail($"Unhandled expression type: {firstExpression.GetType()}");
				break;
		}

		switch (secondExpression)
		{
			case string stringExpression:
				StringObject stringObject = new(stringExpression);
				secondHashKey = stringObject.HashKey();
				break;

			case int integerExpression:
				IntegerObject integerObject = new(integerExpression);
				secondHashKey = integerObject.HashKey();
				break;

			case bool booleanExpression:
				BooleanObject booleanObject = new(booleanExpression);
				secondHashKey = booleanObject.HashKey();
				break;
			
			default:
				Assert.Fail($"Unhandled expression type: {secondExpression.GetType()}");
				break;
		}
		
		Assert.Equivalent(firstHashKey, secondHashKey, true);
	}
}
