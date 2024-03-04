using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public struct LetStatement
{
	public Token Token { get; }
	public Identifier Name { get; }
	public IExpression Value { get; }
}
