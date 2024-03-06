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
}