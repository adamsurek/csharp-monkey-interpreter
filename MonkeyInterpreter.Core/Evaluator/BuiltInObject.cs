namespace MonkeyInterpreter.Core.Evaluator;

public class BuiltInObject : IObject
{
	private const ObjectTypeEnum ObjectType = ObjectTypeEnum.BuiltIn;
	
	public delegate IObject BuiltInFunction(params IObject[] args);
	public BuiltInFunction Function;

	public BuiltInObject(BuiltInFunction function)
	{
		Function = function;
	}

	public ObjectTypeEnum Type()
	{
		return ObjectType;
	}

	public string Inspect()
	{
		return "Built-in function";
	}
}