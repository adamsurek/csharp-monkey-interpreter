namespace MonkeyInterpreter.Core;

public class Boolean : IObject
{
	private const string ObjectType = "BOOLEAN";
	private readonly bool _value;

	public Boolean(bool value)
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
