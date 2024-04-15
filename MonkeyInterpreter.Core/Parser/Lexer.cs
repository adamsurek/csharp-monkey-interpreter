namespace MonkeyInterpreter.Core.Parser;

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
	
	public Token NextToken()
	{
		Token token = new(Token.Illegal, "\0");
		
		SkipWhitespace();
		
		switch (Character)
		{
			case '=':
				if (PeekCharacter() == '=')
				{
					char currentCharacter = Character;
					ReadCharacter();
					string literal = currentCharacter.ToString() + Character;
					token = new Token(Token.Equal, literal);
				}
				else
				{
					token = new Token(Token.Assign, Character.ToString());
				}
				break;
			
			case '+':
				token = new Token(Token.Plus, Character.ToString());
				break;
			
			case '-':
				token = new Token(Token.Minus, Character.ToString());
				break;
			
			case '/':
				token = new Token(Token.Slash, Character.ToString());
				break;
			
			case '*':
				token = new Token(Token.Asterisk, Character.ToString());
				break;
			
			case '>':
				token = new Token(Token.GThan, Character.ToString());
				break;
			
			case '<':
				token = new Token(Token.LThan, Character.ToString());
				break;
			
			case '!':
				if (PeekCharacter() == '=')
				{
					char currentCharacter = Character;
					ReadCharacter();
					string literal = currentCharacter.ToString() + Character;
					token = new Token(Token.NEqual, literal);
				}
				else
				{
					token = new Token(Token.Bang, Character.ToString());
				}
				break;
			
			case ';':
				token = new Token(Token.Semicolon, Character.ToString());
				break;
			
			case '(':
				token = new Token(Token.LParen, Character.ToString());
				break;
				
			case ')':
				token = new Token(Token.RParen, Character.ToString());
				break;
			
			case ',':
				token = new Token(Token.Comma, Character.ToString());
				break;
			
			case '{':
				token = new Token(Token.LBrace, Character.ToString());
				break;
			
			case '}':
				token = new Token(Token.RBrace, Character.ToString());
				break;
			
			case '\0':
				token = new Token(Token.Eof, '\0'.ToString());
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
					token.Type = Token.Int;
					token.Literal = ReadNumber();
					return token;
				}
				else
				{
					token = new Token(Token.Illegal, Character.ToString());
				}
				break;
		}
		
		ReadCharacter();
		
		return token;
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
	
	private string ReadIdentifier()
	{
		int startPosition = Position;
		while (IsLetter(Character))
		{
			ReadCharacter();
		}
		return Input.Substring(startPosition, Position - startPosition);
	}

	private string ReadNumber()
	{
		int startPosition = Position;
		while (char.IsDigit(Character))
		{
			ReadCharacter();
		}
		return Input.Substring(startPosition, Position - startPosition);
	}

	private bool IsLetter(char character)
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

	private char PeekCharacter()
	{
		if (ReadPosition >= Input.Length)
		{
			return '\0';
		}
		else
		{
			return Input[ReadPosition];
		}
	}
}