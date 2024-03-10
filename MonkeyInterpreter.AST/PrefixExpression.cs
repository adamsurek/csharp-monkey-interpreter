using System.Text;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class PrefixExpression : IExpression
{
	public Token Token;
	public string Operator;
	public IExpression Right;

	public string TokenLiteral()
	{
		return Token.Literal;
	}

	public string String()
	{
		StringBuilder stringBuilder = new();

		stringBuilder.Append('(');
		stringBuilder.Append(Operator);
		stringBuilder.Append(Right.String());
		stringBuilder.Append(')');

		return stringBuilder.ToString();
	}
}