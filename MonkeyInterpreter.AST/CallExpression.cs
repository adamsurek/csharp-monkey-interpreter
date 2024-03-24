using System.Text;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class CallExpression : IExpression
{
	public Token Token;
	public IExpression Function;
	public List<IExpression>? Arguments;

	public CallExpression(Token token, IExpression function, List<IExpression> arguments)
	{
		Token = token;
		Function = function;
		Arguments = arguments;
	}

	public string TokenLiteral()
	{
		return Token.Literal;
	}

	public string String()
	{
		StringBuilder stringBuilder = new();

		List<string> arguments = new();

		foreach (IExpression argument in Arguments)
		{
			arguments.Add(argument.String());
		}

		stringBuilder.Append(Function.String());
		stringBuilder.Append('(');
		stringBuilder.Append(string.Join(", ", arguments));
		stringBuilder.Append(')');

		return stringBuilder.ToString();
	}
}