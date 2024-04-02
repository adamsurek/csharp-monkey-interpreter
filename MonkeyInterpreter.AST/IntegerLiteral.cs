using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class IntegerLiteral : IExpression
{
	public Token Token;
	public int Value;

	public IntegerLiteral(Token token)
	{
		Token = token;
	}

	public string TokenLiteral()
	{
		return Token.Literal;
	}

	public string String()
	{
		return Token.Literal;
	}
}