using System.Linq.Expressions;
using MonkeyInterpreter.AST;
using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public static class Evaluator
{
	public static IObject? Evaluate(INode node)
	{
		switch (node)
		{
			case AbstractSyntaxTree ast:
				return EvaluateStatements(ast.Statements);
			
			case ExpressionStatement expressionStatement:
				return Evaluate(expressionStatement.Expression);
			
			case IntegerLiteral integerLiteral:
				return new Integer(integerLiteral.Value);
		}

		return null;
	}

	private static IObject? EvaluateStatements(List<IStatement> statements)
	{
		IObject? result = null;
		
		foreach (IStatement statement in statements)
		{
			result = Evaluate(statement);
		}

		return result;
	}
}