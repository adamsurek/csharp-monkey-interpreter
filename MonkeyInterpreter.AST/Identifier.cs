using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public struct Identifier : IExpression
{
	public Token Token { get; set; }
	public string Value { get; set; }
	
	public string TokenLiteral()
	{
		return Token.Literal;
	}

	public string String()
	{
		return Value;
	}
}
