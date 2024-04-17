using System.Text;
using MonkeyInterpreter.Core.Parser;

namespace MonkeyInterpreter.Core.AbstractSyntaxTree;

public class FunctionLiteral : IExpression
{
    private readonly Token _token;

    public List<Identifier> Parameters;
    public BlockStatement Body;

    public FunctionLiteral(Token token, List<Identifier> parameters, BlockStatement body)
    {
        _token = token;
        Parameters = parameters;
        Body = body;
    }

    public string TokenLiteral()
    {
        return _token.Literal;
    }

    public string String()
    {
        StringBuilder stringBuilder = new();
        List<string> parameters = new();

        foreach(Identifier parameter in Parameters)
        {
            parameters.Add(parameter.String());
        }
            
        stringBuilder.Append(TokenLiteral());
        stringBuilder.Append('(');
        stringBuilder.Append(string.Join(", ", parameters));
        stringBuilder.Append(") ");
        stringBuilder.Append(Body?.String());

        return stringBuilder.ToString();
    }
}