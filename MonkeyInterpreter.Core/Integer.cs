namespace MonkeyInterpreter.Core;

public class Integer : IObject
{
	private const string ObjectType = "INTEGER";
	private readonly int _value;

	public Integer(int value)
	{
		_value = value;
	}

	public string Inspect()
	{
		return _value.ToString();
	}

	public string Type()
	{
		return ObjectType;
	}
}
