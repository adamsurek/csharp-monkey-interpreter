namespace MonkeyInterpreter.Core;

public class Integer : IObject
{
	private const string ObjectType = "INTEGER";
	public readonly int Value;

	public Integer(int value)
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
