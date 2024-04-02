using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class ExpressionStatement : IStatement
{
	private readonly Token _token;
	
	public IExpression? Expression;

	public ExpressionStatement(Token token)
	{
		_token = token;
	}

	public string TokenLiteral()
	{
		return _token.Literal;
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