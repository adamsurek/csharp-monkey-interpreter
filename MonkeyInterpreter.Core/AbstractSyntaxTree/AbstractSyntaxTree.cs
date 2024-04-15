using System.Text;

namespace MonkeyInterpreter.Core.AbstractSyntaxTree;

public class AbstractSyntaxTree : INode
{
	public List<IStatement> Statements;
	public List<IExpression> Expressions;
	
	public string TokenLiteral()
	{
		if (Statements.Count > 0)
		{
			return Statements[0].TokenLiteral();
		}
			
		return "";
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