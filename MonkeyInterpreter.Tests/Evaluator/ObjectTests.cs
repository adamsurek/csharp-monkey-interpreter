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
				firstHashKey = new HashKey(new StringObject(stringExpression));
				break;
			
			case int integerExpression:
				firstHashKey = new HashKey(new IntegerObject(integerExpression));
				break;
			
			case bool booleanExpression:
				firstHashKey = new HashKey(new BooleanObject(booleanExpression));
				break;
			
			default:
				Assert.Fail($"Unhandled expression type: {firstExpression.GetType()}");
				break;
		}

		switch (secondExpression)
		{
			case string stringExpression:
				secondHashKey = new HashKey(new StringObject(stringExpression));
				break;
			
			case int integerExpression:
				secondHashKey = new HashKey(new IntegerObject(integerExpression));
				break;
			
			case bool booleanExpression:
				secondHashKey = new HashKey(new BooleanObject(booleanExpression));
				break;
			
			default:
				Assert.Fail($"Unhandled expression type: {secondExpression.GetType()}");
				break;
		}
		
		Assert.Equivalent(firstHashKey, secondHashKey, true);
	}
}
