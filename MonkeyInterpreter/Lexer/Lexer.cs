namespace MonkeyInterpreterDotNet.Lexer;

using MonkeyInterpreterDotNet.Lexer;

public class Lexer
{
	public string Input;
	public int Position;
	public int ReadPosition;
	public char Character;

	public Lexer(string input)
	{
		Input = input;
		ReadCharacter();
	}
	
	public void ReadCharacter()
	{
		if (ReadPosition >= Input.Length)
		{
			Character = '\0';
		}
		else
		{
			Character = Input[ReadPosition];
		}

		Position = ReadPosition;
		ReadPosition += 1;
	}
		

	public Token? NextToken()
	{
		Token token = new Token(Token.ILLEGAL, "\0");
		
		SkipWhitespace();
		
		switch (Character)
		{
			case '=':
				token = new Token(Token.ASSIGN, Character.ToString());
				break;
			
			case ';':
				token = new Token(Token.SEMICOLON, Character.ToString());
				break;
			
			case '(':
				token = new Token(Token.LPAREN, Character.ToString());
				break;
				
			case ')':
				token = new Token(Token.RPAREN, Character.ToString());
				break;
			
			case ',':
				token = new Token(Token.COMMA, Character.ToString());
				break;
			
			case '+':
				token = new Token(Token.PLUS, Character.ToString());
				break;
			
			case '{':
				token = new Token(Token.LBRACE, Character.ToString());
				break;
			
			case '}':
				token = new Token(Token.RBRACE, Character.ToString());
				break;
			
			case '\0':
				token = new Token(Token.EOF, '\0'.ToString());
				break;
			
			default:
				if (IsLetter(Character))
				{
					token.Literal = ReadIdentifier();
					token.Type = token.LookupIdentifier(token.Literal);
					return token;
				}
				else if (char.IsDigit(Character))
				{
					token.Type = Token.INT;
					token.Literal = ReadNumber();
					return token;
				}
				else
				{
					token = new Token(Token.ILLEGAL, Character.ToString());
				}
				break;
		}
		
		ReadCharacter();
		
		return token;
	}

	public string ReadIdentifier()
	{
		int startPosition = Position;
		while (IsLetter(Character))
		{
			ReadCharacter();
		}
		return Input.Substring(startPosition, Position - startPosition);
	}

	public string ReadNumber()
	{
		int startPosition = Position;
		while (char.IsDigit(Character))
		{
			ReadCharacter();
		}
		return Input.Substring(startPosition, Position - startPosition);
	}

	public bool IsLetter(char character)
	{
		return character is >= 'a' or >= 'A' and >= 'Z';
	}

	private void SkipWhitespace()
	{
		while (char.IsWhiteSpace(Character))
		{
			ReadCharacter();
		}
	}
}