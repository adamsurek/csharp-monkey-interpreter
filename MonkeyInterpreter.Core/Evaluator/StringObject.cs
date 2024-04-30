namespace MonkeyInterpreter.Core.Evaluator;

public class StringObject : IObject, IHashable
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
	
	public HashKey HashKey()
	{
		var s1 = Value.Substring(0, Value.Length / 2);
		var s2 = Value.Substring(Value.Length / 2);
		var hash = (long)s1.GetHashCode() << 32 | (uint)s2.GetHashCode();
		return new HashKey(ObjectType, (int)hash);
                
		// return new HashKey(ObjectType, Value.GetHashCode());
	}
}