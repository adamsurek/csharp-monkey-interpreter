using System.Text;
using MonkeyInterpreter.Core.Parser;

namespace MonkeyInterpreter.Core.AbstractSyntaxTree;

public class LetStatement : IStatement
{
	private readonly Token _token;
	
	public readonly IExpression Value;
	public readonly Identifier Name;
	
	public LetStatement(Token token, Identifier name, IExpression value)
	{
		_token = token;
		Name = name;
		Value = value;
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
		stringBuilder.Append(Value.String());
		stringBuilder.Append(';');
		return stringBuilder.ToString();
	}
}
