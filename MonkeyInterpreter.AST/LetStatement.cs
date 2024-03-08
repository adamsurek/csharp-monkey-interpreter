using System.Text;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class LetStatement : IStatement
{
	public Token Token { get; set; }
	public Identifier Name { get; set; }
	public IExpression Value { get; set; }

	public LetStatement(Token token)
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
		stringBuilder.Append(TokenLiteral() + " ");
		stringBuilder.Append(Name.String());
		stringBuilder.Append(" = ");

		if (Value is not null)
		{
			stringBuilder.Append(Value.String());
		}

		stringBuilder.Append(';');
		return stringBuilder.ToString();
	}
}
