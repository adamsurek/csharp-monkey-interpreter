using System.Text;
using MonkeyInterpreter.Core.Parser;

namespace MonkeyInterpreter.Core.AbstractSyntaxTree;

public class ArrayLiteral : IExpression
{
	public readonly Token Token;
	public List<IExpression>? Elements;

	public ArrayLiteral(Token token, List<IExpression>? elements = null)
	{
		Token = token;
		Elements = elements;
	}

	public string TokenLiteral()
	{
		return Token.Literal;
	}

	public string String()
	{
		StringBuilder stringBuilder = new();
		List<string> elements = new();

		foreach (var element in Elements)
		{
			elements.Add(element.String());
		}

		stringBuilder.Append("[");
		stringBuilder.Append(string.Join(", ", elements));
		stringBuilder.Append("]");

		return stringBuilder.ToString();
	}
}