using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public struct LetStatement : IStatement
{
	public Token Token { get; set; }
	public Identifier Name { get; set; }
	public IExpression Value { get; }

	public LetStatement StatementNode()
	{
		return this;
	}
	
	public string TokenLiteral()
	{
		return Token.Literal;
	}
}
