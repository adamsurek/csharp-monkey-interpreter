using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public struct Identifier
{
	public Token Token { get; }
	public string Value { get; }
}
