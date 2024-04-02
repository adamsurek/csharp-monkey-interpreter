using System.Text;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class ReturnStatement : IStatement
{
	private readonly Token _token;
	
	public readonly IExpression ReturnValue;

	public ReturnStatement(Token token, IExpression returnValue)
	{
		_token = token;
		ReturnValue = returnValue;
	}

	public string TokenLiteral()
	{
		return _token.Literal;
	}

	public string String()
	{
		StringBuilder stringBuilder = new();
		stringBuilder.Append(TokenLiteral() + " ");
		stringBuilder.Append(ReturnValue.String());
		stringBuilder.Append(';');
		return stringBuilder.ToString();
	}
}