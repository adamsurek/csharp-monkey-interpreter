using MonkeyInterpreter.Core.Parser;

namespace MonkeyInterpreter.Core.AbstractSyntaxTree;

public class BooleanLiteral : IExpression
{
	private readonly Token _token;
	public readonly bool Value;

	public BooleanLiteral(Token token, bool value)
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
		return _token.Literal;
	}
}