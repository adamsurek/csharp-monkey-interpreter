using System.Text;
using MonkeyInterpreter.Core.Parser;

namespace MonkeyInterpreter.Core.AbstractSyntaxTree;

public class HashLiteral : IExpression
{
	public readonly Token Token;
	public Dictionary<IExpression, IExpression?> Pairs;

	public HashLiteral(Token token, Dictionary<IExpression, IExpression?> pairs)
	{
		Token = token;
		Pairs = pairs;
	}

	public string TokenLiteral()
	{
		return Token.Literal;
	}

	public string String()
	{
		StringBuilder stringBuilder = new();
		List<string> pairs = new();

		foreach (var pair in Pairs)
		{
			pairs.Add($"{pair.Key.String()}:{pair.Value.String()}");
		}

		stringBuilder.Append("{");
		stringBuilder.Append(string.Join(", ", pairs));
		stringBuilder.Append("}");

		return stringBuilder.ToString();
	}
}