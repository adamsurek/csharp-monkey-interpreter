using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class ExpressionStatement : IStatement
{
	public Token Token;
	public IExpression Expression;

	public ExpressionStatement(Token token)
	{
		Token = token;
	}

	public string TokenLiteral()
	{
		return Token.Literal;
	}

	public string String()
	{
		if (Expression is not null)
		{
			return Expression.String();
		}

		return "";
	}
}