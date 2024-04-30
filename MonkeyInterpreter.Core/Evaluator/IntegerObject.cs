
namespace MonkeyInterpreter.Core.Evaluator;

public class IntegerObject : IObject, IHashable
{
	private const ObjectTypeEnum ObjectType = ObjectTypeEnum.Integer;
	public readonly int Value;

	public IntegerObject(int value)
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
		return new HashKey(ObjectType, Value);
	}
}
