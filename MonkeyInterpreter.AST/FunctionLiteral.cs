using System.Text;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class FunctionLiteral : IExpression
{
    private readonly Token _token;
    
    public List<Identifier>? Parameters = new();
    public BlockStatement? Body;

    public FunctionLiteral(Token token)
    {
        _token = token;
    }

    public string TokenLiteral()
    {
        return _token.Literal;
    }

    public string String()
    {
        StringBuilder stringBuilder = new();
        List<string> parameters = new();

        if (Parameters is not null)
        {
            foreach(Identifier parameter in Parameters)
            {
                parameters.Add(parameter.String());
            }
        }
            
        stringBuilder.Append(TokenLiteral());
        stringBuilder.Append('(');
        stringBuilder.Append(string.Join(", ", parameters));
        stringBuilder.Append(") ");
        stringBuilder.Append(Body?.String());

        return stringBuilder.ToString();
    }
}