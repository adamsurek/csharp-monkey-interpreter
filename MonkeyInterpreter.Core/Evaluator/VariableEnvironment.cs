namespace MonkeyInterpreter.Core.Evaluator;

public class VariableEnvironment
{
	private readonly Dictionary<string, IObject> _store;

	public VariableEnvironment()
	{
		_store = new Dictionary<string, IObject>();
	}

	public IObject? Get(string key)
	{
		return _store.GetValueOrDefault(key);
	}

	public void Set(string key, IObject value)
	{
		_store.Add(key, value);
	}
}