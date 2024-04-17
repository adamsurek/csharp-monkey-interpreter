using System.Text;
using MonkeyInterpreter.Core.AbstractSyntaxTree;

namespace MonkeyInterpreter.Core.Evaluator;

public class FunctionObject : IObject
{
	public List<Identifier> Parameters = new();
	public BlockStatement Body;
	public VariableEnvironment VariableEnvironment;

	public FunctionObject(List<Identifier> parameters, BlockStatement body, VariableEnvironment env)
	{
		Parameters = parameters;
		Body = body;
		VariableEnvironment = env;
	}

	public ObjectTypeEnum Type()
	{
		return ObjectTypeEnum.Function;
	}

	public string Inspect()
	{
        StringBuilder stringBuilder = new();
        List<string> parameters = new();

        foreach(Identifier parameter in Parameters)
        {
	        parameters.Add(parameter.String());
        }
            
        stringBuilder.Append("fn");
        stringBuilder.Append('(');
        stringBuilder.Append(string.Join(", ", parameters));
        stringBuilder.Append(") {\n");
        stringBuilder.Append(Body?.String());
        stringBuilder.Append("\n}");

        return stringBuilder.ToString();
	}
}