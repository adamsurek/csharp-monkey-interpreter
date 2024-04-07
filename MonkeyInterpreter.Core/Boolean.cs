namespace MonkeyInterpreter.Core;

public class Boolean : IObject
{
	private const string ObjectType = "BOOLEAN";
	public readonly bool Value;

	public Boolean(bool value)
	{
		Value = value;
	}

	public string Inspect()
	{
		return Value.ToString();
	}

	public string Type()
	{
		return ObjectType;
	}
}
