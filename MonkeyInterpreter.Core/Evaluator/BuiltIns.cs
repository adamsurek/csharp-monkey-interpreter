namespace MonkeyInterpreter.Core.Evaluator;

public static class BuiltIns
{
	public static Dictionary<string, BuiltInObject> BuiltInFunctions = new()
	{
		{ "len", new BuiltInObject(Len) }
	};

	private static IObject Len(params IObject[] args)
	{
		if (args.Length != 1)
		{
			return Evaluator.GenerateError("Wrong number of arguments. Got {0}, expected 1",
				args.Length);
		}

		switch (args[0])
		{
			case StringObject stringObject:
				return new IntegerObject(stringObject.Value.Length);
			
			default:
				return Evaluator.GenerateError("Argument to 'len' not supported. Expected String but got {0}",
					args[0].Type());
		}
	}
}