namespace MonkeyInterpreter.Core.Evaluator;

public class BooleanObject : IObject, IHashable
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
	
	public HashKey HashKey()
	{
		return new HashKey(ObjectType, Value ? 1 : 0);
	}
}
