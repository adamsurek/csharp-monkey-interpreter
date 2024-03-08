using System.Text;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class ReturnStatement : IStatement
{
	public Token Token { get; set; }
	public IExpression ReturnValue { get; set; }

	public ReturnStatement(Token token)
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

		if (ReturnValue is not null)
		{
			stringBuilder.Append(ReturnValue.String());
		}

		stringBuilder.Append(';');
		return stringBuilder.ToString();
	}
}