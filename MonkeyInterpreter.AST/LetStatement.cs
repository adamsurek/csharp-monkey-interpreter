using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public struct LetStatement
{
	public Token _Token;
	public Identifier Name;
	public IExpression Value;
}
