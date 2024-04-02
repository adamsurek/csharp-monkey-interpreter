using System.Text;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class LetStatement : IStatement
{
	private readonly Token _token;
	private readonly IExpression _value;
	
	public Identifier Name;
	
	public LetStatement(Token token, Identifier name, IExpression value)
	{
		_token = token;
		Name = name;
		_value = value;
	}

	public string TokenLiteral()
	{
		return _token.Literal;
	}

	public string String()
	{
		StringBuilder stringBuilder = new();
		stringBuilder.Append(TokenLiteral() + " ");
		stringBuilder.Append(Name.String());
		stringBuilder.Append(" = ");
		stringBuilder.Append(_value.String());
		stringBuilder.Append(';');
		return stringBuilder.ToString();
	}
}
