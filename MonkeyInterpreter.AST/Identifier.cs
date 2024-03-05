using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public struct Identifier
{
	public Token Token { get; set; }
	public string Value { get; set; }
	
	public void StatementNode()
	{
		
	}
	
	public string TokenLiteral()
	{
		return Token.Literal;
	}
}
