using System.Text;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class BlockStatement : IExpression
{
	private readonly Token _token;
	
	public readonly List<IStatement> Statements;

	public BlockStatement(Token token, List<IStatement> statements)
	{
		_token = token;
		Statements = statements;
	}
	public string TokenLiteral()
	{
		return _token.Literal;
	}

	public string String()
	{
		StringBuilder stringBuilder = new();

		foreach (IStatement statement in Statements)
		{
			stringBuilder.Append(statement.String());
		}

		return stringBuilder.ToString();
	}
}