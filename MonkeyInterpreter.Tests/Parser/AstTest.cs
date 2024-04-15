using MonkeyInterpreter.Core;
using MonkeyInterpreter.Core.AbstractSyntaxTree;
using MonkeyInterpreter.Core.Parser;

namespace MonkeyInterpreter.Tests.Parser;

public class AstTest
{
	[Fact]
	public void AstToStringTest()
	{
		Identifier statementName = new(new Token(Token.Ident, "x"), "x");
		Identifier statementValue = new(new Token(Token.Ident, "y"), "y");
		
		AbstractSyntaxTree ast = new()
		{
			Statements = new List<IStatement>
			{
				new LetStatement(new Token(Token.Let, "let"), statementName, statementValue)
			}
		};
		
		Assert.Equal("let x = y;", ast.String());
	}
}



