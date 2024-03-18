using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

// TODO: Handle nulls
// TODO: Add method XML comments

public enum TokenPrecedence
{
	Lowest = 1,
	Equals,
	LessGreater,
	Sum,
	Product,
	Prefix,
	Call
}

public class Parser
{
	private Token _currentToken;
	private Token _peekToken;
	private Lexer _lexer;
	private List<string> _errors;

	private delegate IExpression ParsePrefixFunc();
	private delegate IExpression ParseInfixFunc(IExpression expression);

	private Dictionary<string, ParsePrefixFunc> _prefixParsers;
	private Dictionary<string, ParseInfixFunc> _infixParsers;

	public Dictionary<string, TokenPrecedence> TokenPrecedenceCategoryMap = new()
	{
		{ Token.Equal, TokenPrecedence.Equals },
		{ Token.NEqual, TokenPrecedence.Equals },
		{ Token.LThan, TokenPrecedence.LessGreater },
		{ Token.GThan, TokenPrecedence.LessGreater },
		{ Token.Plus, TokenPrecedence.Sum },
		{ Token.Minus, TokenPrecedence.Sum },
		{ Token.Slash, TokenPrecedence.Product },
		{ Token.Asterisk, TokenPrecedence.Product },
	};

	public Parser(Lexer lexer)
	{
		_lexer = lexer;
		_errors = new();
		NextToken();
		NextToken();

		_prefixParsers = new Dictionary<string, ParsePrefixFunc>();
		RegisterPrefixFunction(Token.Ident, ParseIdentifier);
		RegisterPrefixFunction(Token.Int, ParseIntegerLiteral);
		RegisterPrefixFunction(Token.Bang, ParsePrefixExpression);
		RegisterPrefixFunction(Token.Minus, ParsePrefixExpression);
		RegisterPrefixFunction(Token.True, ParseBooleanLiteral);
		RegisterPrefixFunction(Token.False, ParseBooleanLiteral);

		_infixParsers = new Dictionary<string, ParseInfixFunc>();
		RegisterInfixFunction(Token.Plus, ParseInfixExpression);
		RegisterInfixFunction(Token.Minus, ParseInfixExpression);
		RegisterInfixFunction(Token.Slash, ParseInfixExpression);
		RegisterInfixFunction(Token.Asterisk, ParseInfixExpression);
		RegisterInfixFunction(Token.Equal, ParseInfixExpression);
		RegisterInfixFunction(Token.NEqual, ParseInfixExpression);
		RegisterInfixFunction(Token.LThan, ParseInfixExpression);
		RegisterInfixFunction(Token.GThan, ParseInfixExpression);
		
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
				return ParseExpressionStatement();
		}
	}

	private IExpression? ParseExpression(TokenPrecedence precedence)
	{
		ParsePrefixFunc? prefixFunc = _prefixParsers.GetValueOrDefault(_currentToken.Type);

		if (prefixFunc is null)
		{
			NoPrefixParseFunctionError(_currentToken.Type);
			return null;
		}

		IExpression leftExpression = prefixFunc.Invoke();

		while (!IsPeekToken(Token.Semicolon) && precedence < PeekPrecedence())
		{
			ParseInfixFunc? infixFunc = _infixParsers.GetValueOrDefault(_peekToken.Type);

			if (infixFunc is null)
			{
				return leftExpression;
			}
			
			NextToken();
			leftExpression = infixFunc.Invoke(leftExpression);
		}

		return leftExpression;
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
	
	private ExpressionStatement? ParseExpressionStatement()
	{
		ExpressionStatement expressionStatement = new(_currentToken);
		expressionStatement.Expression = ParseExpression(TokenPrecedence.Lowest);

		if (IsPeekToken(Token.Semicolon))
		{
			NextToken();
		}

		return expressionStatement;
	}

	private IExpression ParseIdentifier()
	{
		return new Identifier(token: _currentToken, value: _currentToken.Literal);
	}

	private IExpression? ParseIntegerLiteral()
	{
		IntegerLiteral integerLiteral = new()
		{
			Token = _currentToken
		};

		if (!int.TryParse(_currentToken.Literal, out integerLiteral.Value))
		{
			_errors.Add($"Could not parse {_currentToken.Literal} as integer");
			return null;
		}

		return integerLiteral;
	}

	private IExpression ParseBooleanLiteral()
	{
		return new BooleanLiteral(token: _currentToken, value: IsCurrentToken(Token.True));
	}

	private IExpression ParsePrefixExpression()
	{
		PrefixExpression expression = new()
		{
			Token = _currentToken,
			Operator = _currentToken.Literal
		};
		
		NextToken();

		expression.Right = ParseExpression(TokenPrecedence.Prefix);
		return expression;
	}

	private IExpression ParseInfixExpression(IExpression left)
	{
		InfixExpression expression = new()
		{
			Token = _currentToken,
			Operator = _currentToken.Literal,
			Left = left
		};

		TokenPrecedence precedence = CurrentPrecedence();
		NextToken();
		expression.Right = ParseExpression(precedence);

		return expression;
	}
	
	private bool IsCurrentToken(string token)
	{
		return _currentToken.Type == token;
	}

	private bool IsPeekToken(string token)
	{
		return _peekToken.Type == token;
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

	private void NoPrefixParseFunctionError(string token)
	{
		_errors.Add($"No prefix parse function for {token} found");
	}

	private void RegisterPrefixFunction(string token, ParsePrefixFunc function)
	{
		_prefixParsers[token] = function;
	}
	
	private void RegisterInfixFunction(string token, ParseInfixFunc function)
	{
		_infixParsers[token] = function;
	}

	private TokenPrecedence PeekPrecedence()
	{
			return TokenPrecedenceCategoryMap.GetValueOrDefault(_peekToken.Type, TokenPrecedence.Lowest);
	}
	
	private TokenPrecedence CurrentPrecedence()
	{
			return TokenPrecedenceCategoryMap.GetValueOrDefault(_currentToken.Type, TokenPrecedence.Lowest);
	}
}