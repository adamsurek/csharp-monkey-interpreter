namespace MonkeyInterpreter.Core.Evaluator;

public class HashPair
{
	public IObject Key;
	public IObject Value;

	public HashPair(IObject key, IObject value)
	{
		Key = key;
		Value = value;
	}
}
