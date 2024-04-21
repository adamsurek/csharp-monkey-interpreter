using System.Text;
using MonkeyInterpreter.Core.Parser;

namespace MonkeyInterpreter.Core.AbstractSyntaxTree;

public class IndexExpression : IExpression
{
	public readonly Token Token;
	public IExpression Left;
	public IExpression Index;

	public IndexExpression(Token token, IExpression left, IExpression index)
	{
		Token = token;
		Left = left;
		Index = index;
	}

	public string TokenLiteral()
	{
		return Token.Literal;
	}

	public string String()
	{
		StringBuilder stringBuilder = new();

		stringBuilder.Append("(");
		stringBuilder.Append(Left.String());
		stringBuilder.Append("[");
		stringBuilder.Append(Index.String());
		stringBuilder.Append("])");

		return stringBuilder.ToString();
	}
}