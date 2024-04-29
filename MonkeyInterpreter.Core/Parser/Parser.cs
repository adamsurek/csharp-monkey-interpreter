using MonkeyInterpreter.Core.AbstractSyntaxTree;

namespace MonkeyInterpreter.Core.Parser;

// TODO: Add method XML comments

public enum TokenPrecedence
{
	Lowest = 1,
	Equals,
	LessGreater,
	Sum,
	Product,
	Prefix,
	Call,
	Index
}

public class Parser
{
	private Token _currentToken;
	private Token _peekToken;
	private readonly Lexer _lexer;
	private readonly List<string> _errors;

	private delegate IExpression? ParsePrefixFunc();
	private delegate IExpression? ParseInfixFunc(IExpression expression);

	private readonly Dictionary<string, ParsePrefixFunc> _prefixParsers;
	private readonly Dictionary<string, ParseInfixFunc> _infixParsers;

	private readonly Dictionary<string, TokenPrecedence> _tokenPrecedenceCategoryMap = new()
	{
		{ Token.Equal, TokenPrecedence.Equals },
		{ Token.NEqual, TokenPrecedence.Equals },
		{ Token.LThan, TokenPrecedence.LessGreater },
		{ Token.GThan, TokenPrecedence.LessGreater },
		{ Token.Plus, TokenPrecedence.Sum },
		{ Token.Minus, TokenPrecedence.Sum },
		{ Token.Slash, TokenPrecedence.Product },
		{ Token.Asterisk, TokenPrecedence.Product },
		{ Token.LParen, TokenPrecedence.Call },
		{ Token.LBracket, TokenPrecedence.Index}
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
		RegisterPrefixFunction(Token.String, ParseStringLiteral);
		RegisterPrefixFunction(Token.Bang, ParsePrefixExpression);
		RegisterPrefixFunction(Token.Minus, ParsePrefixExpression);
		RegisterPrefixFunction(Token.True, ParseBooleanLiteral);
		RegisterPrefixFunction(Token.False, ParseBooleanLiteral);
		RegisterPrefixFunction(Token.LParen, ParseGroupedExpression);
		RegisterPrefixFunction(Token.LBracket, ParseArrayLiteral);
		RegisterPrefixFunction(Token.LBrace, ParseHashLiteral);
		RegisterPrefixFunction(Token.If, ParseIfExpression);
		RegisterPrefixFunction(Token.Function,  ParseFunctionLiteral);

		_infixParsers = new Dictionary<string, ParseInfixFunc>();
		RegisterInfixFunction(Token.Plus, ParseInfixExpression);
		RegisterInfixFunction(Token.Minus, ParseInfixExpression);
		RegisterInfixFunction(Token.Slash, ParseInfixExpression);
		RegisterInfixFunction(Token.Asterisk, ParseInfixExpression);
		RegisterInfixFunction(Token.Equal, ParseInfixExpression);
		RegisterInfixFunction(Token.NEqual, ParseInfixExpression);
		RegisterInfixFunction(Token.LThan, ParseInfixExpression);
		RegisterInfixFunction(Token.GThan, ParseInfixExpression);
		RegisterInfixFunction(Token.LParen, ParseCallExpression);
		RegisterInfixFunction(Token.LBracket, ParseIndexExpression);
	}

	public AbstractSyntaxTree.AbstractSyntaxTree ParseProgram()
	{
		AbstractSyntaxTree.AbstractSyntaxTree abstractSyntaxTree = new();
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

		IExpression? leftExpression = prefixFunc.Invoke();

		if (leftExpression is null)
		{
			return null;
		}

		while (!IsPeekToken(Token.Semicolon) && precedence < PeekPrecedence())
		{
			ParseInfixFunc? infixFunc = _infixParsers.GetValueOrDefault(_peekToken.Type);

			if (infixFunc is null)
			{
				return leftExpression;
			}
			
			NextToken();
			leftExpression = infixFunc.Invoke(leftExpression!);
		}

		return leftExpression;
	}

	private LetStatement? ParseLetStatement()
	{
		Token currentToken = _currentToken;

		if (!ExpectPeek(Token.Ident))
		{
			return null;
		}

		Identifier name = new(_currentToken, _currentToken.Literal);

		if (!ExpectPeek(Token.Assign))
		{
			return null;
		}

		NextToken();
		
		IExpression? value = ParseExpression(TokenPrecedence.Lowest);

		if (value is null)
		{
			return null;
		}

		if (IsPeekToken(Token.Semicolon))
		{
			NextToken();
		}

		return new LetStatement(currentToken, name, value);
	}

	private ReturnStatement? ParseReturnStatement()
	{
		Token currentToken = _currentToken;
		
		NextToken();
		
		IExpression? value = ParseExpression(TokenPrecedence.Lowest);

		if (value is null)
		{
			return null;
		}

		if (IsPeekToken(Token.Semicolon))
		{
			NextToken();
		}

		return new ReturnStatement(currentToken, value);
	}
	
	private ExpressionStatement? ParseExpressionStatement()
	{
		ExpressionStatement expressionStatement = new(_currentToken);
		
		IExpression? expression = ParseExpression(TokenPrecedence.Lowest);

		if (expression is null)
		{
			return null;
		}
		
		expressionStatement.Expression = expression;

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
		IntegerLiteral integerLiteral = new(_currentToken);

		if (!int.TryParse(_currentToken.Literal, out integerLiteral.Value))
		{
			_errors.Add($"Could not parse {_currentToken.Literal} as integer");
			return null;
		}

		return integerLiteral;
	}

	private IExpression ParseStringLiteral()
	{
		return new StringLiteral(_currentToken, _currentToken.Literal);
	}

	private IExpression ParseBooleanLiteral()
	{
		return new BooleanLiteral(token: _currentToken, value: IsCurrentToken(Token.True));
	}

	private IExpression? ParseGroupedExpression()
	{
		NextToken();
		IExpression? expression = ParseExpression(TokenPrecedence.Lowest);

		if (!ExpectPeek(Token.RParen))
		{
			return null;
		}

		return expression;
	}

	private IExpression ParseArrayLiteral()
	{
		Token currentToken = _currentToken;
		
		return new ArrayLiteral(currentToken, ParseExpressionList(Token.RBracket));
	}

	private List<IExpression>? ParseExpressionList(string end)
	{
		List<IExpression> expressions = new();

		if (IsPeekToken(end))
		{
			NextToken();
			return expressions;
		}
		
		NextToken();
		
		IExpression? expression = ParseExpression(TokenPrecedence.Lowest);
		if (expression is not null)
		{
			expressions.Add(expression);
		}

		while (IsPeekToken(Token.Comma))
		{
			NextToken();
			NextToken();

			expression = ParseExpression(TokenPrecedence.Lowest);
			if (expression is not null)
			{
				expressions.Add(expression);
			}
		}

		if (!ExpectPeek(end))
		{
			return null;
		}

		return expressions;
	}

	private IExpression? ParseIndexExpression(IExpression left)
	{
		Token currentToken = _currentToken;
		NextToken();

		IExpression? index = ParseExpression(TokenPrecedence.Lowest);
		if (!ExpectPeek(Token.RBracket))
		{
			return null;
		}

		return new IndexExpression(currentToken, left, index);
	}

	private IExpression? ParseHashLiteral()
	{
		HashLiteral hashLiteral = new(_currentToken, new Dictionary<IExpression, IExpression?>());

		while (!IsPeekToken(Token.RBrace))
		{
			NextToken();

			IExpression? hashKey = ParseExpression(TokenPrecedence.Lowest);

			if (hashKey is null || !ExpectPeek(Token.Colon))
			{
				return null;
			}

			NextToken();

			IExpression? hashValue = ParseExpression(TokenPrecedence.Lowest);

			if (!IsPeekToken(Token.RBrace) && !ExpectPeek(Token.Comma))
			{
				return null;
			}
			
			hashLiteral.Pairs.Add(hashKey, hashValue);
		}

		if (!ExpectPeek(Token.RBrace))
		{
			return null;
		}

		return hashLiteral;
	}

	private IExpression? ParseIfExpression()
	{
		Token currentToken = _currentToken;
		
		if (!ExpectPeek(Token.LParen))
		{
			return null;
		}
		
		NextToken();
		
		IExpression? condition = ParseExpression(TokenPrecedence.Lowest);

		if (condition is null)
		{
			return null;
		}
		
		if (!ExpectPeek(Token.RParen))
		{
			return null;
		}
		
		if (!ExpectPeek(Token.LBrace))
		{
			return null;
		}
		
		BlockStatement consequence = ParseBlockStatement();
		BlockStatement? alternative = null;

		if (IsPeekToken(Token.Else))
		{
			NextToken();

			if (!ExpectPeek(Token.LBrace))
			{
				return null;
			}

			alternative = ParseBlockStatement();
		}
		
		
		IfExpression expression = new (currentToken, condition, consequence, alternative);

		return expression;
	}

	private IExpression? ParseFunctionLiteral()
	{
		Token currentToken = _currentToken;

		if (!ExpectPeek(Token.LParen))
		{
			return null;
		}

		List<Identifier>? parameters = ParseFunctionParameters();

		if (parameters is null)
		{
			parameters = new List<Identifier>();
		}

		if (!ExpectPeek(Token.LBrace))
		{
			return null;
		}

		return new FunctionLiteral(currentToken, parameters, ParseBlockStatement());
	}

	private BlockStatement ParseBlockStatement()
	{
		BlockStatement blockStatement = new BlockStatement(_currentToken, new List<IStatement>());
		
		NextToken();

		while (!IsCurrentToken(Token.RBrace) && !IsCurrentToken(Token.Eof))
		{
			IStatement? statement = ParseStatement();

			if (statement is not null)
			{
				blockStatement.Statements.Add(statement);
			}
			
			NextToken();
		}

		return blockStatement;
	}

	private List<Identifier>? ParseFunctionParameters()
	{
		List<Identifier> identifiers = new();

		if (IsPeekToken(Token.RParen))
		{
			NextToken();
			return identifiers;
		}
		
		NextToken();

		Identifier identifier = new Identifier(_currentToken, _currentToken.Literal);
		identifiers.Add(identifier);

		while (IsPeekToken(Token.Comma))
		{
			NextToken();
			NextToken();

			identifier = new Identifier(_currentToken, _currentToken.Literal);
			identifiers.Add(identifier);
		}

		if (!ExpectPeek(Token.RParen))
		{
			return null;
		}

		return identifiers;
	}

	private IExpression? ParseCallExpression(IExpression function)
	{
		Token currentToken = _currentToken;
		List<IExpression>? arguments = ParseExpressionList(Token.RParen);

		if (arguments is null)
		{
			return null;
		}
		
		return new CallExpression(currentToken, function, arguments);
	}

	private IExpression? ParsePrefixExpression()
	{
		Token currentToken = _currentToken;
		string @operator = _currentToken.Literal;
		
		NextToken();
		
		IExpression? right = ParseExpression(TokenPrecedence.Prefix);

		if (right is null)
		{
			return null;
		}

		return new PrefixExpression(currentToken, @operator, right);
	}

	private IExpression? ParseInfixExpression(IExpression left)
	{
		Token currentToken = _currentToken;
		string @operator = _currentToken.Literal;

		TokenPrecedence precedence = CurrentPrecedence();
		NextToken();
		
		IExpression? right = ParseExpression(precedence);

		if (right is null)
		{
			return null;
		}

		return new InfixExpression(currentToken, left, @operator, right);
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
			return _tokenPrecedenceCategoryMap.GetValueOrDefault(_peekToken.Type, TokenPrecedence.Lowest);
	}
	
	private TokenPrecedence CurrentPrecedence()
	{
			return _tokenPrecedenceCategoryMap.GetValueOrDefault(_currentToken.Type, TokenPrecedence.Lowest);
	}
}