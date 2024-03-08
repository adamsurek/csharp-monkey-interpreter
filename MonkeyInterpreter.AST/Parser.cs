using MonkeyInterpreter.AST;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class Parser
{
	private Token _currentToken;
	private Token _peekToken;
	private Lexer _lexer;
	private List<string> _errors;

	private delegate IExpression ParsePrefixFunc();
	private delegate IExpression ParseInfixFunc(IExpression expression);

	private Dictionary<Token, ParsePrefixFunc> PrefixParsers;
	private Dictionary<Token, ParseInfixFunc> InfixParsers;

	public Parser(Lexer lexer)
	{
		_lexer = lexer;
		_errors = new();
		NextToken();
		NextToken();
	}

	public AbstractSyntaxTree ParseProgram()
	{
		AbstractSyntaxTree abstractSyntaxTree = new();
		abstractSyntaxTree.Statements = new List<IStatement>();
		
		while (_currentToken.Type != Token.Eof)
		{
			IStatement? statement = ParseStatement();
			if (statement is not null)
			{
				abstractSyntaxTree.Statements.Add(statement);
			}

			NextToken();
		}

		return abstractSyntaxTree;
	}
	
	private void NextToken()
	{
		_currentToken = _peekToken;
		_peekToken = _lexer.NextToken();
	}

	private IStatement? ParseStatement()
	{
		switch (_currentToken.Type)
		{
			case Token.Let:
				return ParseLetStatement();
			case Token.Return:
				return ParseReturnStatement();
			default:
				return null;
		}
	}

	private LetStatement? ParseLetStatement()
	{
		LetStatement letStatement = new(_currentToken);

		if (!ExpectPeek(Token.Ident))
		{
			return null;
		}

		letStatement.Name = new Identifier()
		{
			Token = _currentToken,
			Value = _currentToken.Literal
		};

		if (!ExpectPeek(Token.Assign))
		{
			return null;
		}

		while (!IsCurrentToken(Token.Semicolon))
		{
			NextToken();
		}

		return letStatement;
	}

	private ReturnStatement ParseReturnStatement()
	{
		ReturnStatement returnStatement = new(_currentToken);
		NextToken();

		while (!IsCurrentToken(Token.Semicolon))
		{
			NextToken();
		}

		return returnStatement;
	}
	
	private bool IsCurrentToken(string token)
	{
		return _currentToken?.Type == token;
	}

	private bool IsPeekToken(string token)
	{
		return _peekToken?.Type == token;
	}

	private bool ExpectPeek(string token)
	{
		if (IsPeekToken(token))
		{
			NextToken();
			return true;
		}
		
		PeekError(token);
		return false;
	}

	public List<string> Errors()
	{
		return _errors;
	}

	private void PeekError(string token)
	{
		_errors.Add($"Expected next token to be {token}, got {_peekToken.Type} instead");
	}

	private void RegisterPrefixFunction(Token token, ParsePrefixFunc function)
	{
		PrefixParsers[token] = function;
	}
	
	private void RegisterInfixFunction(Token token, ParseInfixFunc function)
	{
		InfixParsers[token] = function;
	}
}