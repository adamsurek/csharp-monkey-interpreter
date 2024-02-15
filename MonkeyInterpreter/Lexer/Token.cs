namespace MonkeyInterpreterDotNet.Lexer;

public class Token(string type, string literal)
{
	public const string ILLEGAL = "ILLEGAL";
	public const string EOF = "\0";

	// Identifiers
	public const string IDENT = "IDENT";
	public const string INT = "INT";

	// Operators
	public const string ASSIGN = "=";
	public const string PLUS = "+";

	// Delimiters
	public const string COMMA = ",";
	public const string SEMICOLON = ";";
	
	public const string LPAREN = "(";
	public const string RPAREN = ")";
	public const string LBRACE = "{";
	public const string RBRACE = "}";

	// Keywords
	public const string FUNCTION = "FUNCTION";
	public const string LET = "LET";
		
	public string Type = type;
	public string Literal = literal;

	public Dictionary<string, string> Keywords = new Dictionary<string, string>()
	{
		{ "fn", FUNCTION },
		{ "let", LET }
	};

	public string LookupIdentifier(string identifier)
	{
		return Keywords.GetValueOrDefault(identifier, IDENT);
	}
}