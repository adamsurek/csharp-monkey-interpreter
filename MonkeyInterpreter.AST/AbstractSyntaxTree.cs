namespace MonkeyInterpreter.AST;

public class AbstractSyntaxTree
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
}