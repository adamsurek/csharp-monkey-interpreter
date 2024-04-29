using System.Text;

namespace MonkeyInterpreter.Core.Evaluator;

public class HashObject : IObject
{
	private const ObjectTypeEnum ObjectType = ObjectTypeEnum.Hash;
	
	public Dictionary<HashKey, HashPair> Pairs;

	public HashObject(Dictionary<HashKey, HashPair> pairs)
	{
		Pairs = pairs;
	}

	public ObjectTypeEnum Type()
	{
		return ObjectType;
	}

	public string Inspect()
	{
		StringBuilder stringBuilder = new();
		List<string> pairs = new();

		foreach (HashPair pair in Pairs.Values)
		{
			pairs.Add($"{pair.Key.Inspect()}: {pair.Value.Inspect()}");
		}
		
		stringBuilder.Append("{");
		stringBuilder.Append(string.Join(", ", pairs));
		stringBuilder.Append("}");
		
		return stringBuilder.ToString();
	}
}
