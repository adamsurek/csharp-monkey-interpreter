namespace MonkeyInterpreter.Core;

public class ReturnValueObject : IObject
{
	private const ObjectTypeEnum ObjectType = ObjectTypeEnum.ReturnValue;
	public readonly IObject Value;

	public ReturnValueObject(IObject value)
	{
		Value = value;
	}

	public ObjectTypeEnum Type()
	{
		return ObjectType;
	}

	public string Inspect()
	{
		return Value.Inspect();
	}
}