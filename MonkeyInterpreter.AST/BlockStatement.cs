using System.Text;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class BlockStatement : IExpression
{
	public Token Token;
	public List<IStatement> Statements;

	public BlockStatement(Token token, List<IStatement> statements)
	{
		Token = token;
		Statements = statements;
	}
	public string TokenLiteral()
	{
		return Token.Literal;
	}

	public string String()
	{
		StringBuilder stringBuilder = new();

		foreach (var statement in Statements)
		{
			stringBuilder.Append(statement.String());
		}

		return stringBuilder.ToString();
	}
}