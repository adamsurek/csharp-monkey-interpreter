namespace MonkeyInterpreter.Core.Evaluator;

public class StringObject : IObject
{
	private const ObjectTypeEnum ObjectType = ObjectTypeEnum.String;
	public string Value;

	public StringObject(string value)
	{
		Value = value;
	}

	public ObjectTypeEnum Type()
	{
		return ObjectType;
	}

	public string Inspect()
	{
		return Value;
	}
}