using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class Tree
{
	public interface INode
	{
		string TokenLiteral();
	}
	
	public interface IStatement : INode
	{
		// INode Node { get; }
		LetStatement StatementNode();
	}
	
	public interface IExpression : INode
	{
		// INode Node { get; }
		Identifier ExpressionNode();
	}

	public struct Identifier
	{
		public Token _Token;
		public string Value;
	}

	public struct LetStatement
	{
		public Token _Token;
		public Identifier Name;
		public IExpression Value;
	}
	
	public struct Program : IStatement, IExpression
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

	
	
}