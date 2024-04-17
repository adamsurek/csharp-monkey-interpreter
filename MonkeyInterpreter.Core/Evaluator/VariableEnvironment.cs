namespace MonkeyInterpreter.Core.Evaluator;

public class VariableEnvironment
{
	private readonly Dictionary<string, IObject> _store;
	private VariableEnvironment? _outer;

	public VariableEnvironment()
	{
		_store = new Dictionary<string, IObject>();
	}

	public IObject? Get(string key)
	{
		IObject? @object = _store.GetValueOrDefault(key);

		if (@object is null && _outer is not null)
		{
			@object = _outer.Get(key);
		}

		return @object;
	}

	public void Set(string key, IObject value)
	{
		_store.Add(key, value);
	}

	public VariableEnvironment EncloseEnvironment(VariableEnvironment outerEnvironment)
	{
		VariableEnvironment environment = new();
		environment._outer = outerEnvironment;
		return environment;
	}
}