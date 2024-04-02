using System.Text;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class CallExpression : IExpression
{
	private readonly Token _token;
	
	public readonly IExpression Function;
	public readonly List<IExpression?> Arguments;

	public CallExpression(Token token, IExpression function, List<IExpression?> arguments)
	{
		_token = token;
		Function = function;
		Arguments = arguments;
	}

	public string TokenLiteral()
	{
		return _token.Literal;
	}

	public string String()
	{
		StringBuilder stringBuilder = new();

		List<string> arguments = new();

		foreach (IExpression? argument in Arguments)
		{
			if (argument is not null)
			{
				arguments.Add(argument.String());
			}
		}

		stringBuilder.Append(Function.String());
		stringBuilder.Append('(');
		stringBuilder.Append(string.Join(", ", arguments));
		stringBuilder.Append(')');

		return stringBuilder.ToString();
	}
}