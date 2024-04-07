namespace MonkeyInterpreter.Core;

public class Null : IObject
{
	private const string ObjectType = "NULL";

	public string Inspect()
	{
		return "null";
	}

	public string Type()
	{
		return ObjectType;
	}
}
