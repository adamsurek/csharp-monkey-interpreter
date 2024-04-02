using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class Identifier : IExpression
{
	private readonly Token _token;
	public readonly string Value;
	
	public Identifier(Token token, string value)
	{
		_token = token;
		Value = value;
	}
	
	public string TokenLiteral()
	{
		return _token.Literal;
	}

	public string String()
	{
		return Value;
	}
}
