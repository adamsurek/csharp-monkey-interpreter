using System.Runtime.InteropServices;
using Xunit.Abstractions;

namespace MonkeyInterpreter.Tests.Lexer;

using System.Collections.Generic;

using MonkeyInterpreterDotNet.Lexer;

public class LexerTest
{
	private readonly ITestOutputHelper output;
	public LexerTest(ITestOutputHelper output)
	{
		this.output = output;
	}
	
	[Fact]
	public void TestNextToken()
	{
		var input = """
		            		let five = 5;
		            		let ten = 10;
		            
		            		let add = fn(x, y)
		            		{
		            			x + y;
		            		};
		            
		            		let result = add(five, ten);
		            """;
		var tests = new List<List<string>>()
		{
			new List<string> { Token.LET, "let" }, 
			new List<string> { Token.IDENT, "five" }, 
			new List<string> { Token.ASSIGN, "=" }, 
			new List<string> { Token.INT, "5" }, 
			new List<string> { Token.SEMICOLON, ";" }, 
			new List<string> { Token.LET, "let" }, 
			new List<string> { Token.IDENT, "ten" }, 
			new List<string> { Token.ASSIGN, "=" }, 
			new List<string> { Token.INT, "10" }, 
			new List<string> { Token.SEMICOLON, ";" }, 
			new List<string> { Token.LET, "let" }, 
			new List<string> { Token.IDENT, "add" }, 
			new List<string> { Token.ASSIGN, "=" }, 
			new List<string> { Token.FUNCTION, "fn" }, 
			new List<string> { Token.LPAREN, "(" }, 
			new List<string> { Token.IDENT, "x" },
			new List<string> { Token.COMMA, "," }, 
			new List<string> { Token.IDENT, "y" }, 
			new List<string> { Token.RPAREN, ")" }, 
			new List<string> { Token.LBRACE, "{" }, 
			new List<string> { Token.IDENT, "x" }, 
			new List<string> { Token.PLUS, "+" }, 
			new List<string> { Token.IDENT, "y" }, 
			new List<string> { Token.SEMICOLON, ";" }, 
			new List<string> { Token.RBRACE, "}" }, 
			new List<string> { Token.SEMICOLON, ";" }, 
			new List<string> { Token.LET, "let" }, 
			new List<string> { Token.IDENT, "result" }, 
			new List<string> { Token.ASSIGN, "=" }, 
			new List<string> { Token.IDENT, "add" }, 
			new List<string> { Token.LPAREN, "(" }, 
			new List<string> { Token.IDENT, "five" }, 
			new List<string> { Token.COMMA, "," }, 
			new List<string> { Token.IDENT, "ten" }, 
			new List<string> { Token.RPAREN, ")" }, 
			new List<string> { Token.SEMICOLON, ";" }, 
			new List<string> { Token.EOF, "" },
		};
		
		Lexer lexer = new Lexer(input);

		for (int x = 0; x < tests.Count; x++)
		{
			var token = lexer.NextToken();
			Assert.Equal(token?.Type, tests[x][0] );
		}
	}
}