using System.Text;
using MonkeyInterpreter.Core.Parser;

namespace MonkeyInterpreter.Core.AbstractSyntaxTree;

public class InfixExpression : IExpression
{
	private readonly Token _token;
	
	public readonly IExpression Left;
	public readonly IExpression Right;
	public readonly string Operator;

	public InfixExpression(Token token, IExpression left, string @operator, IExpression right)
	{
		_token = token;
		Left = left;
		Operator = @operator;
		Right = right;
	}

	public string TokenLiteral()
	{
		return _token.Literal;
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