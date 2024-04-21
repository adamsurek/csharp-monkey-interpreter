namespace MonkeyInterpreter.Core.Parser;

public class Token(string type, string literal)
{
	public const string Illegal = "ILLEGAL";
	public const string Eof = "\0";

	// Identifiers
	public const string Ident = "IDENT";
	public const string Int = "INT";
	public const string String = "STRING";

	// Operators
	public const string Assign = "=";
	public const string Plus = "+";
	public const string Minus = "-";
	public const string Slash = "/";
	public const string Asterisk = "*";
	public const string GThan = ">";
	public const string LThan = "<";
	public const string Bang = "!";
	
	// Comparison
	public const string Equal = "==";
	public const string NEqual = "!=";

	// Delimiters
	public const string Comma = ",";
	public const string Semicolon = ";";
	
	public const string LParen = "(";
	public const string RParen = ")";
	public const string LBrace = "{";
	public const string RBrace = "}";
	public const string LBracket = "[";
	public const string RBracket = "]";

	// Keywords
	public const string Function = "FUNCTION";
	public const string Let = "LET";
	public const string True = "TRUE";
	public const string False = "FALSE";
	public const string If = "IF";
	public const string Else = "ELSE";
	public const string Return = "Return";
		
	public string Type = type;
	public string Literal = literal;

	public Dictionary<string, string> Keywords = new Dictionary<string, string>()
	{
		{ "fn", Function },
		{ "let", Let },
		{ "true", True },
		{ "false", False },
		{ "if", If },
		{ "else", Else },
		{ "return", Return },
	};

	public string LookupIdentifier(string identifier)
	{
		return Keywords.GetValueOrDefault(identifier, Ident);
	}
}