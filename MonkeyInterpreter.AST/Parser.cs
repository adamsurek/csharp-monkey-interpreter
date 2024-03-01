using MonkeyInterpreter.AST;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class Parser
{
	public Lexer _Lexer;
	public Token? CurrentToken;
	public Token? PeekToken;

	public Parser(Lexer lexer)
	{
		_Lexer = lexer;
	}

	public void NextToken()
	{
		CurrentToken = PeekToken;
		PeekToken = _Lexer.NextToken();
	}

	public Tree.Program ParseProgram()
	{
		return new Tree.Program();
	}
}