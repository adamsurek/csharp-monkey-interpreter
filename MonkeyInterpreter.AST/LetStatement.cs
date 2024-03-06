using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class LetStatement : IStatement
{
	public Token Token { get; set; }
	public Identifier Name { get; set; }
	public IExpression Value { get; }

	public LetStatement(Token token)
	{
		Token = token;
	}

	public string TokenLiteral()
	{
		return Token.Literal;
	}
}
