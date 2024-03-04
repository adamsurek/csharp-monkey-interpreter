using MonkeyInterpreter.AST;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class Parser
{
	private Token? _currentToken;
	private Token? _peekToken;
	private Lexer _lexer;

	public Parser(Lexer lexer)
	{
		_lexer = lexer;
	}

	public void NextToken()
	{
		_currentToken = _peekToken;
		_peekToken = _lexer.NextToken();
	}

	public Root ParseProgram()
	{
		return new Root();
	}
}