using System.Text;
using MonkeyInterpreter.Core.Parser;

namespace MonkeyInterpreter.Core.AbstractSyntaxTree;

public class PrefixExpression : IExpression
{
	private readonly Token _token;
	
	public readonly string Operator;
	public readonly IExpression Right;

	public PrefixExpression(Token token, string @operator, IExpression right)
	{
		_token = token;
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
		stringBuilder.Append(Operator);
		stringBuilder.Append(Right.String());
		stringBuilder.Append(')');

		return stringBuilder.ToString();
	}
}