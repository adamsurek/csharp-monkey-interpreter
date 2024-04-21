using System.Text;

namespace MonkeyInterpreter.Core.Evaluator;

public class ArrayObject : IObject
{
	private const ObjectTypeEnum ObjectType = ObjectTypeEnum.Array;
	
	public List<IObject> Elements;

	public ArrayObject(List<IObject> elements)
	{
		Elements = elements;
	}

	public ObjectTypeEnum Type()
	{
		return ObjectType;
	}

	public string Inspect()
	{
		StringBuilder stringBuilder = new StringBuilder();

		List<string> elements = new();
		foreach (IObject element in Elements)
		{
			elements.Add(element.Inspect());
		}

		stringBuilder.Append("[");
		stringBuilder.Append(string.Join(", ", elements));
		stringBuilder.Append("]");

		return stringBuilder.ToString();
	}
}