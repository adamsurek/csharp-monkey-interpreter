using System.Runtime.InteropServices;
using MonkeyInterpreter.Core.Parser;
using Xunit.Abstractions;

namespace MonkeyInterpreter.Tests.Lexer;

using System.Collections.Generic;

using Core;

public class LexerTest
{
	private readonly ITestOutputHelper _output;
	public LexerTest(ITestOutputHelper output)
	{
		this._output = output;
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
		            		!-/*5;
		            		5 < 10 > 5;
		            		
		            		if (5  < 10) {
		            			return true;
		            		} else {
		            			return false;	
		            		}
		            		
		            		10 == 10;
		            		10 != 9;
		            """;
		var tests = new List<List<string>>()
		{
			new() { Token.Let, "let" }, 
			new() { Token.Ident, "five" }, 
			new() { Token.Assign, "=" }, 
			new() { Token.Int, "5" }, 
			new() { Token.Semicolon, ";" }, 
			new() { Token.Let, "let" }, 
			new() { Token.Ident, "ten" }, 
			new() { Token.Assign, "=" }, 
			new() { Token.Int, "10" }, 
			new() { Token.Semicolon, ";" }, 
			new() { Token.Let, "let" }, 
			new() { Token.Ident, "add" }, 
			new() { Token.Assign, "=" }, 
			new() { Token.Function, "fn" }, 
			new() { Token.LParen, "(" }, 
			new() { Token.Ident, "x" },
			new() { Token.Comma, "," }, 
			new() { Token.Ident, "y" }, 
			new() { Token.RParen, ")" }, 
			new() { Token.LBrace, "{" }, 
			new() { Token.Ident, "x" }, 
			new() { Token.Plus, "+" }, 
			new() { Token.Ident, "y" }, 
			new() { Token.Semicolon, ";" }, 
			new() { Token.RBrace, "}" }, 
			new() { Token.Semicolon, ";" }, 
			new() { Token.Let, "let" }, 
			new() { Token.Ident, "result" }, 
			new() { Token.Assign, "=" }, 
			new() { Token.Ident, "add" }, 
			new() { Token.LParen, "(" }, 
			new() { Token.Ident, "five" }, 
			new() { Token.Comma, "," }, 
			new() { Token.Ident, "ten" }, 
			new() { Token.RParen, ")" }, 
			new() { Token.Semicolon, ";" }, 
			new() { Token.Bang, "!" },
			new() { Token.Minus, "-" },
			new() { Token.Slash, "/" },
			new() { Token.Asterisk, "*" },
			new() { Token.Int, "5" },
			new() { Token.Semicolon, ";" },
			new() { Token.Int, "5" },
			new() { Token.LThan, "<" },
			new() { Token.Int, "10" },
			new() { Token.GThan, ">" },
			new() { Token.Int, "5" },
			new() { Token.Semicolon, ";" },
			new() { Token.If, "if" },
			new() { Token.LParen, "(" },
			new() { Token.Int, "5" },
			new() { Token.LThan, "<" },
			new() { Token.Int, "10" },
			new() { Token.RParen, ")" },
			new() { Token.LBrace, "{" },
			new() { Token.Return, "return" },
			new() { Token.True, "true" },
			new() { Token.Semicolon, ";" },
			new() { Token.RBrace, "}" },
			new() { Token.Else, "else" },
			new() { Token.LBrace, "{" },
			new() { Token.Return, "return" },
			new() { Token.False, "false" },
			new() { Token.Semicolon, ";" },
			new() { Token.RBrace, "}" },
			new() { Token.Int, "10" },
			new() { Token.Equal, "==" },
			new() { Token.Int, "10" },
			new() { Token.Semicolon, ";" },
			new() { Token.Int, "10" },
			new() { Token.NEqual, "!=" },
			new() { Token.Int, "9" },
			new() { Token.Semicolon, ";" },
			new() { Token.Eof, "" },
		};
		
		Core.Parser.Lexer lexer = new Core.Parser.Lexer(input);

		for (int x = 0; x < tests.Count; x++)
		{
			var token = lexer.NextToken();
			Assert.Equal(token?.Type, tests[x][0] );
		}
	}
}