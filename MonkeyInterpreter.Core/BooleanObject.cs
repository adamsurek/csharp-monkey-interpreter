namespace MonkeyInterpreter.Core;

public class BooleanObject : IObject
{
	private const ObjectTypeEnum ObjectType = ObjectTypeEnum.Boolean;
	public readonly bool Value;

	public BooleanObject(bool value)
	{
		Value = value;
	}

	public string Inspect()
	{
		return Value.ToString();
	}

	public ObjectTypeEnum Type()
	{
		return ObjectType;
	}
}
