namespace MonkeyInterpreter.Core.Evaluator;

public struct HashKey
{
	private ObjectTypeEnum _type;
	private int _value;

	public HashKey(ObjectTypeEnum type, int value)
	{
		_type = type;
		_value = value;
	}
}