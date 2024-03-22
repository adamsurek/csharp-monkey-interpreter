using System.Text;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class IfExpression : IExpression
{
	public Token Token;
	public IExpression? Condition;
	public BlockStatement? Consequence;
	public BlockStatement? Alternative;

	public IfExpression(Token token)
	{
		Token = token;
	}

	public string TokenLiteral()
	{
		return Token.Literal;
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