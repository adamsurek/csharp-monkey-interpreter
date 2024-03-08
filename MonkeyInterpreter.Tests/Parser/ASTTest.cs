using MonkeyInterpreter.AST;
using MonkeyInterpreter.Core;
using Xunit.Abstractions;

namespace MonkeyInterpreter.Tests.Parser;

public class ASTTest
{
	[Fact]
	public void ASTToStringTest()
	{
		AbstractSyntaxTree ast = new()
		{
			Statements = new List<IStatement>
			{
				new LetStatement(new Token(Token.Let, "let"))
				{
					Name = new Identifier
					{
						Token = new Token(Token.Ident, "x"),
						Value = "x"
					},
					Value = new Identifier
					{
						Token = new Token(Token.Ident, "y"),
						Value = "y"
					},
				}
			}
		};
		
		Assert.Equal("let x = y;", ast.String());
	}
}



