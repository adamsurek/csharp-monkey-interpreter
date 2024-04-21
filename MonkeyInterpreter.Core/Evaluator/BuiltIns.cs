using MonkeyInterpreter.Core.AbstractSyntaxTree;

namespace MonkeyInterpreter.Core.Evaluator;

public static class BuiltIns
{
	public static Dictionary<string, BuiltInObject> BuiltInFunctions = new()
	{
		{ "len", new BuiltInObject(Len) },
		{ "first", new BuiltInObject(First) },
		{ "last", new BuiltInObject(Last) },
		{ "rest", new BuiltInObject(Rest) },
		{ "push", new BuiltInObject(Push) },
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
			
			case ArrayObject arrayObject:
				return new IntegerObject(arrayObject.Elements.Count);
			
			default:
				return Evaluator.GenerateError("Argument to 'len' not supported. Expected String but got {0}",
					args[0].Type());
		}
	}

	private static IObject First(params IObject[] args)
	{
		if (args.Length != 1)
		{
			return Evaluator.GenerateError("Wrong number of arguments. Got {0}, expected 1",
				args.Length);
		}

		if (args[0].Type() != ObjectTypeEnum.Array)
		{
			return Evaluator.GenerateError("Argument to 'first' must be Array, got {0}",
				args[0].Type());
		}

		ArrayObject array = (ArrayObject)args[0];
		if (array.Elements.Count > 0)
		{
			return array.Elements[0];
		}

		return new NullObject();
	}
	
	private static IObject Last(params IObject[] args)
	{
		if (args.Length != 1)
		{
			return Evaluator.GenerateError("Wrong number of arguments. Got {0}, expected 1",
				args.Length);
		}

		if (args[0].Type() != ObjectTypeEnum.Array)
		{
			return Evaluator.GenerateError("Argument to 'last' must be Array, got {0}",
				args[0].Type());
		}

		ArrayObject array = (ArrayObject)args[0];
		if (array.Elements.Count > 0)
		{
			return array.Elements[^1];
		}

		return new NullObject();
	}
	
	private static IObject Rest(params IObject[] args)
	{
		if (args.Length != 1)
		{
			return Evaluator.GenerateError("Wrong number of arguments. Got {0}, expected 1",
				args.Length);
		}

		if (args[0].Type() != ObjectTypeEnum.Array)
		{
			return Evaluator.GenerateError("Argument to 'rest' must be Array, got {0}",
				args[0].Type());
		}

		ArrayObject array = (ArrayObject)args[0];
		if (array.Elements.Count > 0)
		{
			List<IObject> elements = array.Elements.GetRange(1, array.Elements.Count - 1);
			return new ArrayObject(elements);
		}

		return new NullObject();
	}
	
	private static IObject Push(params IObject[] args)
	{
		if (args.Length != 2)
		{
			return Evaluator.GenerateError("Wrong number of arguments. Got {0}, expected 1",
				args.Length);
		}

		if (args[0].Type() != ObjectTypeEnum.Array)
		{
			return Evaluator.GenerateError("Argument to 'push' must be Array, got {0}",
				args[0].Type());
		}

		ArrayObject array = (ArrayObject)args[0];
		
		List<IObject> elements = array.Elements.GetRange(0, array.Elements.Count);
		elements.Add(args[1]);
		
		return new ArrayObject(elements);

	}
}