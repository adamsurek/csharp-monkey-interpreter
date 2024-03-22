using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class BooleanLiteral : IExpression
{
	public Token Token;
	public bool Value;

	public BooleanLiteral(Token token, bool value)
	{
		Token = token;
		Value = value;
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