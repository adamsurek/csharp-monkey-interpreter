using System.Text;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

// TODO: Create proper constructor

public class InfixExpression : IExpression
{
	public Token Token;
	public IExpression Left;
	public IExpression Right;
	public string Operator;

	public string TokenLiteral()
	{
		return Token.Literal;
	}

	public string String()
	{
		StringBuilder stringBuilder = new();

		stringBuilder.Append('(');
		stringBuilder.Append(Left.String());
		stringBuilder.Append(Operator);
		stringBuilder.Append(Right.String());
		stringBuilder.Append(')');

		return stringBuilder.ToString();
	}
}