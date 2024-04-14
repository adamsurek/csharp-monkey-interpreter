namespace MonkeyInterpreter.Core;

public static class Environment
{
	private static readonly Dictionary<string, IObject> Store = new();

	public static IObject? Get(string key)
	{
		return Store.GetValueOrDefault(key);
	}

	public static void Set(string key, IObject value)
	{
		Store.Add(key, value);
	}
}