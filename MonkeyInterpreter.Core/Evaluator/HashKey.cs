using MonkeyInterpreter.Core.AbstractSyntaxTree;

namespace MonkeyInterpreter.Core.Evaluator;

public class HashKey
{
	public ObjectTypeEnum Type;
	public int Value;

	public HashKey(BooleanObject booleanObject)
	{
		Type = booleanObject.Type();
		Value = booleanObject.Value ? 1 : 0;
	}

	public HashKey(IntegerObject integerObject)
	{
		Type = integerObject.Type();
		Value = integerObject.Value;
	}
	
	public HashKey(StringObject stringObject)
	{
		Type = stringObject.Type();
		Value = stringObject.Value.GetHashCode();
	}
}