using System.Text;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class IfExpression : IExpression
{
	private readonly Token _token;
	
	public readonly IExpression Condition;
	public readonly BlockStatement Consequence;
	public readonly BlockStatement? Alternative;

	public IfExpression(Token token, IExpression condition, BlockStatement consequence, BlockStatement? alternative = null)
	{
		_token = token;
		Condition = condition;
		Consequence = consequence;
		Alternative = alternative;
	}

	public string TokenLiteral()
	{
		return _token.Literal;
	}

	public string String()
	{
		StringBuilder stringBuilder = new();

		stringBuilder.Append("if");
		stringBuilder.Append(Condition.String());
		stringBuilder.Append(" ");
		stringBuilder.Append(Consequence.String());

		if (Alternative is not null)
		{
			stringBuilder.Append("else");
			stringBuilder.Append(Alternative.String());
		}

		return stringBuilder.ToString();
	}
}