using MonkeyInterpreter.Core.Parser;

namespace MonkeyInterpreter.Core.AbstractSyntaxTree;

public class StringLiteral : IExpression
{
	public readonly Token Token;
	public string Value;

	public StringLiteral(Token token, string value = "")
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
	
	