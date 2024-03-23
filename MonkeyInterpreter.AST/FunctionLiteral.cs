using System.Text;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public class FunctionLiteral : IExpression
{
    public Token Token;
    public List<Identifier>? Parameters = new();
    public BlockStatement? Body;

    public FunctionLiteral(Token token)
    {
        Token = token;
    }

    public string TokenLiteral()
    {
        return Token.Literal;
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