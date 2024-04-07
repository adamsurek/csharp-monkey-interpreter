
namespace MonkeyInterpreter.Core;

public class IntegerObject : IObject
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
}
