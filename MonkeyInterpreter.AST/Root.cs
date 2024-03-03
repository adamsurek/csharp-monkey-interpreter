namespace MonkeyInterpreter.AST;

public class Root : IStatement, IExpression
{
	public IStatement[] Statements;
	public IExpression[] Expressions;
	
	public LetStatement StatementNode()
	{
		return new LetStatement();
	}
		
	public Identifier ExpressionNode()
	{
		return new Identifier();
	}
		
	public string TokenLiteral()
	{
		if (Statements.Length > 0)
		{
			return Statements[0].TokenLiteral();
		}
			
		return "";
	}

	public string TokenLiteral(LetStatement statement)
	{
		return statement._Token.Literal;
	}
		
	public string TokenLiteral(Identifier identifier)
	{
		return identifier._Token.Literal;
	}
}